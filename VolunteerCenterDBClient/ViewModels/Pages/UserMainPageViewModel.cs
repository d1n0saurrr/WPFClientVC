using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VolunteerCenterDBClient.Commands;
using VolunteerCenterDBClient.Models;
using VolunteerCenterDBClient.Navigation;

namespace VolunteerCenterDBClient.ViewModels.Pages
{
    class UserMainPageViewModel : BaseViewModel
    {
        private string _token;
        private long _id;

        public ObservableCollection<Event> _events;
        public ObservableCollection<Event> ItemsSourceEvents
        {
            get =>
                _events;

            set
            {
                _events = value;
                OnPropertyChanged(nameof(ItemsSourceEvents));
            }
        }

        public ObservableCollection<Event> _userEvents;
        public ObservableCollection<Event> ItemsSourceUserEvents
        {
            get =>
                _userEvents;

            set
            {
                _userEvents = value;
                OnPropertyChanged(nameof(ItemsSourceUserEvents));
            }
        }

        public ObservableCollection<Event> _requestedEvents;
        public ObservableCollection<Event> ItemsSourceRequestedEvents
        {
            get =>
                _requestedEvents;

            set
            {
                _requestedEvents = value;
                OnPropertyChanged(nameof(ItemsSourceRequestedEvents));
            }
        }

        public User _user;
        public User User
        {
            get =>
                _user;

            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }

        private int _selected;
        public int Selected
        {
            get =>
                _selected;

            set
            {
                _selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }

        public UserMainPageViewModel(string token, long id) { _token = token; _id = id; AllEventsCommand.Execute(null); }

        #region Commands

        private ICommand _allEventsCommand;
        private ICommand _myEventsCommand;
        private ICommand _profileCommand;
        private ICommand _profileUpdateCommand;
        private ICommand _goBackCommand;
        private ICommand _chooseEventCommand;

        public ICommand AllEventsCommand =>
            _allEventsCommand ?? (_allEventsCommand = new RelayCommand(async _ => await GetAllEvents()));

        public ICommand MyEventsCommand =>
            _myEventsCommand ?? (_myEventsCommand = new RelayCommand(async _ => await GetUserEvents()));

        public ICommand ProfileCommand =>
            _profileCommand ?? (_profileCommand = new RelayCommand(async _ => await GetProfile()));

        public ICommand ProfileUpdateCommand =>
            _profileUpdateCommand ?? (_profileUpdateCommand = new RelayCommand(async _ => await UpdateProfile()));

        public ICommand GoBackCommand =>
            _goBackCommand ?? (_goBackCommand = new RelayCommand(_ => NavigationManager.GoBack()));

        public ICommand ChooseEventCommand =>
            _chooseEventCommand ?? (_chooseEventCommand = new RelayCommand(async obj => await RequestEvent(obj)));

        #endregion

        #region Tasks

        private async Task GetAllEvents()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/user/events/all?id=" + _id);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = await streamReader.ReadToEndAsync();
                    ItemsSourceEvents = JsonSerializer.Deserialize<ObservableCollection<Event>>(result);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось прочитать данные с сервера!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
                GoBackCommand.Execute(null);
            }
        }

        private async Task GetUserEvents()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/user/events/my_events?id=" + _id);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = await streamReader.ReadToEndAsync();
                    var response = JsonSerializer.Deserialize<Dictionary<string, List<Event>>>(result);
                    ItemsSourceUserEvents = new ObservableCollection<Event>(response["events"]);
                    ItemsSourceRequestedEvents = new ObservableCollection<Event>(response["requestedEvents"]);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось прочитать данные с сервера!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
                GoBackCommand.Execute(null);
            }
        }

        private async Task GetProfile()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/user/profile?id=" + _id);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = await streamReader.ReadToEndAsync();

                    User = JsonSerializer.Deserialize<User>(result);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось прочитать данные с сервера!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
                GoBackCommand.Execute(null);
            }
        }

        private async Task UpdateProfile()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/user/profile/update");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    User.firstName = User.volunteer.firstName;
                    User.lastName = User.volunteer.lastName;
                    User.patronymic = User.volunteer.patronymic;

                    string json = JsonSerializer.Serialize<User>(User);

                    await streamWriter.WriteAsync(json);
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    if ((await streamReader.ReadToEndAsync()).Equals("OK"))
                    {
                        MessageBox.Show("Данные успешно изменены!");
                        await GetProfile();
                    }
                    else
                    {
                        MessageBox.Show("Не получилось изменить данные!");
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
                GoBackCommand.Execute(null);
            }
        }

        private async Task RequestEvent(Object obj)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/user/events/request?user_id="
                + _id + "&event_id=" + ItemsSourceEvents[(int)obj].id);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    if ((await streamReader.ReadToEndAsync()).Equals("OK"))
                    {
                        MessageBox.Show("Заявка на мероприятие \"" + ItemsSourceEvents[(int)obj].name +  "\" подана!");
                        await GetAllEvents();
                    }
                    else
                    {
                        MessageBox.Show("Не получилось подать заявку!");
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
                GoBackCommand.Execute(null);
            }
        }

        #endregion
    }
}
