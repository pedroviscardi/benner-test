using System;
using Benner.Backend.Shared.Commands;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Application.UseCases.Product.Commands
{
    public class UpdateStockCommand : ICommand<Result<Domain.Entities.Product>>
    {
        public UpdateStockCommand(Guid id, int newQuantity)
        {
            Id = id;
            NewQuantity = newQuantity;
        }

        public Guid Id { get; }
        public int NewQuantity { get; }
    }
}