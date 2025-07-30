using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Benner.Backend.Application.UseCases.Product.Commands;
using Benner.Backend.Application.UseCases.Product.Queries;
using Benner.Backend.Domain.Entities;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;
using Benner.Backend.Shared.Queries;
using Benner.Backend.WPF.App.Commands;
using ICommand = System.Windows.Input.ICommand;

namespace Benner.Backend.WPF.App.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly ICommandHandler<CreateProductCommand, Result<Product>> _createProductHandler;
        private readonly ICommandHandler<DeleteProductCommand, Result<bool>> _deleteProductHandler;
        private readonly IQueryHandler<GetAllProductsQuery, Result<IEnumerable<Product>>> _getAllProductsHandler;
        private readonly ICommandHandler<UpdateProductCommand, Result<Product>> _updateProductHandler;
        private readonly ICommandHandler<UpdateStockCommand, Result<Product>> _updateStockHandler;
        private Product _currentProduct;
        private string _errorMessage;
        private ObservableCollection<Product> _filteredProducts;
        private bool _isEditing;
        private bool _isLoading;

        private ObservableCollection<Product> _products;
        private string _searchText;
        private Product _selectedProduct;
        private string _successMessage;

        public ProductViewModel(
            IQueryHandler<GetAllProductsQuery, Result<IEnumerable<Product>>> getAllProductsHandler,
            ICommandHandler<CreateProductCommand, Result<Product>> createProductHandler,
            ICommandHandler<UpdateProductCommand, Result<Product>> updateProductHandler,
            ICommandHandler<DeleteProductCommand, Result<bool>> deleteProductHandler,
            ICommandHandler<UpdateStockCommand, Result<Product>> updateStockHandler)
        {
            _getAllProductsHandler = getAllProductsHandler;
            _createProductHandler = createProductHandler;
            _updateProductHandler = updateProductHandler;
            _deleteProductHandler = deleteProductHandler;
            _updateStockHandler = updateStockHandler;

            Products = new ObservableCollection<Product>();
            FilteredProducts = new ObservableCollection<Product>();

            LoadProductsCommand = new AsyncRelayCommand(LoadProductsAsync);
            SaveProductCommand = new AsyncRelayCommand(SaveProductAsync, CanSaveProduct);
            DeleteProductCommand = new AsyncRelayCommand(DeleteProductAsync, CanDeleteProduct);
            UpdateStockCommand = new AsyncRelayCommand(UpdateStockAsync, CanUpdateStock);
            NewProductCommand = new RelayCommand(NewProduct);
            EditProductCommand = new RelayCommand(EditProduct, CanEditProduct);
            CancelCommand = new RelayCommand(Cancel);
            SearchCommand = new RelayCommand(PerformSearch);

            CurrentProduct = Product.CreateEmpty();

            _ = LoadProductsAsync();
        }

        #region Properties

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set => SetProperty(ref _filteredProducts, value);
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    if (EditProductCommand is RelayCommand editCommand)
                        editCommand.RaiseCanExecuteChanged();
                    if (DeleteProductCommand is AsyncRelayCommand deleteCommand)
                        deleteCommand.RaiseCanExecuteChanged();
                    if (UpdateStockCommand is AsyncRelayCommand updateStockCommand)
                        updateStockCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public Product CurrentProduct
        {
            get => _currentProduct;
            set
            {
                if (SetProperty(ref _currentProduct, value))
                {
                    if (SaveProductCommand is AsyncRelayCommand saveCommand)
                    {
                        saveCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (SetProperty(ref _isEditing, value))
                {
                    if (EditProductCommand is RelayCommand editCommand)
                        editCommand.RaiseCanExecuteChanged();
                    if (DeleteProductCommand is AsyncRelayCommand deleteCommand)
                        deleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    PerformSearch();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand LoadProductsCommand { get; }
        public ICommand SaveProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand UpdateStockCommand { get; }
        public ICommand NewProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SearchCommand { get; }

        #endregion

        #region Command Methods

        private async Task LoadProductsAsync()
        {
            try
            {
                IsLoading = true;
                ClearMessages();

                var result = await _getAllProductsHandler.HandleAsync(new GetAllProductsQuery());
                if (result.IsSuccess && result.Data != null)
                {
                    Products.Clear();
                    foreach (var product in result.Data)
                    {
                        Products.Add(product);
                    }

                    PerformSearch();
                }
                else
                {
                    ErrorMessage = result.Error ?? "Erro ao carregar produtos";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao carregar produtos: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveProductAsync()
        {
            try
            {
                IsLoading = true;
                ClearMessages();

                Result<Product> result;

                if (IsEditing)
                {
                    var command = new UpdateProductCommand(
                        CurrentProduct.Id,
                        CurrentProduct.Name,
                        CurrentProduct.Description,
                        CurrentProduct.Price,
                        CurrentProduct.StockQuantity,
                        CurrentProduct.MinimumStock,
                        CurrentProduct.Category,
                        CurrentProduct.Brand);

                    result = await _updateProductHandler.HandleAsync(command);
                }
                else
                {
                    var command = new CreateProductCommand(
                        CurrentProduct.Name,
                        CurrentProduct.Description,
                        CurrentProduct.Code,
                        CurrentProduct.Price,
                        CurrentProduct.StockQuantity,
                        CurrentProduct.MinimumStock,
                        CurrentProduct.Category,
                        CurrentProduct.Brand);

                    result = await _createProductHandler.HandleAsync(command);
                }

                if (result.IsSuccess)
                {
                    SuccessMessage = IsEditing ? "Produto atualizado com sucesso!" : "Produto criado com sucesso!";
                    await LoadProductsAsync();
                    Cancel();
                }
                else
                {
                    ErrorMessage = result.Error;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao salvar produto: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteProductAsync()
        {
            if (SelectedProduct == null) return;

            try
            {
                IsLoading = true;
                ClearMessages();

                var command = new DeleteProductCommand(SelectedProduct.Id);
                var result = await _deleteProductHandler.HandleAsync(command);

                if (result.IsSuccess)
                {
                    SuccessMessage = "Produto excluído com sucesso!";
                    await LoadProductsAsync();
                    SelectedProduct = null;
                }
                else
                {
                    ErrorMessage = result.Error;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao excluir produto: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task UpdateStockAsync()
        {
            if (SelectedProduct == null) return;

            try
            {
                IsLoading = true;
                ClearMessages();

                var newQuantity = SelectedProduct.StockQuantity + 10;
                var command = new UpdateStockCommand(SelectedProduct.Id, newQuantity);
                var result = await _updateStockHandler.HandleAsync(command);

                if (result.IsSuccess)
                {
                    SuccessMessage = "Estoque atualizado com sucesso!";
                    await LoadProductsAsync();
                }
                else
                {
                    ErrorMessage = result.Error;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao atualizar estoque: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void NewProduct()
        {
            CurrentProduct = Product.CreateEmpty();
            IsEditing = false;
            ClearMessages();
        }

        private void EditProduct()
        {
            if (SelectedProduct == null) return;

            CurrentProduct = new Product(
                SelectedProduct.Name,
                SelectedProduct.Description,
                SelectedProduct.Code,
                SelectedProduct.Price,
                SelectedProduct.StockQuantity,
                SelectedProduct.MinimumStock,
                SelectedProduct.Category,
                SelectedProduct.Brand);

            typeof(Product).GetProperty("Id")?.SetValue(CurrentProduct, SelectedProduct.Id);

            IsEditing = true;
            ClearMessages();
        }

        private void Cancel()
        {
            CurrentProduct = Product.CreateEmpty();
            IsEditing = false;
            ClearMessages();
        }

        private void PerformSearch()
        {
            FilteredProducts.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var product in Products)
                {
                    FilteredProducts.Add(product);
                }
            }
            else
            {
                var searchLower = SearchText.ToLower();
                var filtered = Products.Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    p.Code.ToLower().Contains(searchLower) ||
                    p.Category.ToLower().Contains(searchLower) ||
                    p.Brand.ToLower().Contains(searchLower));

                foreach (var product in filtered)
                {
                    FilteredProducts.Add(product);
                }
            }
        }

        #endregion

        #region Helper Methods

        private bool CanSaveProduct()
        {
            return CurrentProduct != null &&
                   !string.IsNullOrWhiteSpace(CurrentProduct.Name) &&
                   !string.IsNullOrWhiteSpace(CurrentProduct.Code) &&
                   CurrentProduct.Price > 0 &&
                   !IsLoading;
        }

        private bool CanDeleteProduct()
        {
            return SelectedProduct != null && !IsEditing && !IsLoading;
        }

        private bool CanUpdateStock()
        {
            return SelectedProduct != null && !IsEditing && !IsLoading;
        }

        private bool CanEditProduct()
        {
            return SelectedProduct != null && !IsEditing && !IsLoading;
        }

        private void ClearMessages()
        {
            ErrorMessage = null;
            SuccessMessage = null;
        }

        #endregion
    }
}