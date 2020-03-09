using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace vacationManagement
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            LoginForm loginform = new LoginForm();
            if (loginform.ShowDialog() == true)
            {
                // Lanza la ventana principal
                MainWindow mainwindow = new MainWindow();
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                mainwindow.Show();
            }
        }
    }
}
