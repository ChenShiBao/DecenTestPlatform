
using Decen.Client.TestPlatform.Views;
using Decen.Client.ToolkitMvvm.ViewModel;
using Decen.Core.Laboratory.Service.Impl;
using Decen.Core.Laboratory.Service.IRepository;
using Decen.Core.Laboratory.Service.IService;
using Decen.Core.Laboratory.Service.RepositoryImpl;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Decen.Client.TestPlatform
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; set; }

        public App()
        {
            Services = ConfigureServices();
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ICompanyService, CompanyServiceImpl>();
            services.AddSingleton<ICompanyRepository, CompanyRepository>();

            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<MainWindow>(sp => new MainWindow { DataContext = sp.GetService<MainWindowViewModel>() });

            return services.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = Services.GetService<MainWindow>();
            mainWindow?.Show();

        }
    }
}
