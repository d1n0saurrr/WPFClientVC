using System.Windows;
using VolunteerCenterDBClient.ViewModels;
using VolunteerCenterDBClient.Views;

namespace VolunteerCenterDBClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            (App.Current.MainWindow = new MainWindow { DataContext = new MainViewModel() }).Show();
        }

    }
}
