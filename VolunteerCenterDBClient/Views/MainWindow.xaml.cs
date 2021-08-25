using System.Windows;
using System.Windows.Input;
using VolunteerCenterDBClient.Navigation;

namespace VolunteerCenterDBClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            NavigationManager.NavigationService = _navigationFrame.NavigationService;
        }
    }
}
