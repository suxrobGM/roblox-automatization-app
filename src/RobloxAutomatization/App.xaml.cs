using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using RobloxAutomatization.Data;
using RobloxAutomatization.Services;
using RobloxAutomatization.Views;

namespace RobloxAutomatization
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<MainWindow>();
            containerRegistry.Register<ApplicationDbContext>();
            containerRegistry.Register<IRobloxAutomatizationService, RobloxAutomatizationService>();
        }
    }
}
