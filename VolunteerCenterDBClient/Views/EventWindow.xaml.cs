using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VolunteerCenterDBClient.Models;

namespace VolunteerCenterDBClient.Views
{
    /// <summary>
    /// Interaction logic for EventWindow.xaml
    /// </summary>
    public partial class EventWindow : Window
    {
        public bool IsDeleting { get; private set; }
        private List<Volunteer> _vols = new List<Volunteer>();
        private List<Volunteer> _requestedVols = new List<Volunteer>();
        private List<Volunteer> _requestsFromParent = new List<Volunteer>();
        private Event _event;

        public Event Event
        {
            get
            {
                Event evt = new Event();
                evt.name = name.Text;
                evt.shortName = shortName.Text;
                evt.dateStart = beginDate.SelectedDate.GetValueOrDefault();
                evt.dateEnd = endDate.SelectedDate.GetValueOrDefault();
                evt.volunteers = new List<Volunteer>();
                if (_requestedVols.Count != 0)
                    evt.volunteers.AddRange(_requestedVols);
                if (_vols.Count != 0)
                    evt.volunteers.AddRange(_vols);

                if (orgs.SelectedIndex != -1)
                {
                    evt.orgName = orgs.SelectedItem.ToString();
                }

                if (_event != null)
                    evt.id = _event.id;

                return evt;
            }
        }

        public EventWindow(List<string> orgsList)
        {
            InitializeComponent();
            addButton.Visibility = Visibility.Visible;
            orgs.ItemsSource = orgsList;
        }

        public EventWindow(Event _event, List<string> orgsList, ObservableCollection<Volunteer> vols, ObservableCollection<Volunteer> requests)
        {
            InitializeComponent();
            this._event = _event;
            saveButton.Visibility = Visibility.Visible;
            deleteButton.Visibility = Visibility.Visible;
            volsGrid.IsEnabled = true;
            volsGrid.ItemsSource = vols;
            this._vols.AddRange(vols);
            requestsGrid.Visibility = Visibility.Visible;
            requestsGrid.ItemsSource = requests;
            _requestsFromParent.AddRange(requests);
            orgs.ItemsSource = orgsList;
            if (_event.org != null)
            {
                orgs.SelectedItem = _event.org.name;
                orgs.IsEnabled = false;
            }
            name.Text = _event.name;
            shortName.Text = _event.shortName;
            beginDate.SelectedDate = _event.dateStart;
            endDate.SelectedDate = _event.dateEnd;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            IsDeleting = true;
        }

        private void Accept(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            _requestedVols.Add(_requestsFromParent[requestsGrid.SelectedIndex]);
        }
    }
}
