using VolunteerCenterDBClient.Navigation;
using VolunteerCenterDBClient.ViewModels.Pages;
using VolunteerCenterDBClient.Views.Pages;

namespace VolunteerCenterDBClient.ViewModels
{
    internal sealed class MainViewModel : BaseViewModel
    {

        public MainViewModel()
        {
            NavigationManager.Navigate<AuthorizationPage>(new AuthorizationPageViewModel());
        }

    }
}
