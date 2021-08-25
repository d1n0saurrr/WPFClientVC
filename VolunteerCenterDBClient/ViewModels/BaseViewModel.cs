using System.ComponentModel;

namespace VolunteerCenterDBClient.ViewModels
{
    internal class BaseViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertiesChanged(params string[] propertiesNames)
        {
            foreach (var propertyName in propertiesNames)
            {
                OnPropertyChanged(propertyName);
            }
        }
    }
}
