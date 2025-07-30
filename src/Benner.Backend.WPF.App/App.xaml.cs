using System.Windows;
using Benner.Backend.WPF.App.Services;

namespace Benner.Backend.WPF.App
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceLocator.Initialize();

            var mainViewModel = ViewModelFactory.CreateMainViewModel();
            var mainWindow = new MainWindow(mainViewModel);
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}