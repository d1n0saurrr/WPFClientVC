using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VolunteerCenterDBClient.Commands;
using VolunteerCenterDBClient.Navigation;
using VolunteerCenterDBClient.Models;
using VolunteerCenterDBClient.Views;

namespace VolunteerCenterDBClient.ViewModels.Pages
{
    class AdminPageViewModel : BaseViewModel
    {
        private string _token;

        private Volunteer _selectedVol;
        public Volunteer SelectedVol
        {
            get =>
                _selectedVol;

            set
            {
                _selectedVol = value;
                OnPropertyChanged(nameof(SelectedVol));
            }
        }

        private Event _selectedEvent;
        public Event SelectedEvent
        {
            get =>
                _selectedEvent;

            set
            {
                _selectedEvent = value;
                OnPropertyChanged(nameof(SelectedEvent));
            }
        }

        private Organization _selectedOrg;
        public Organization SelectedOrg
        {
            get =>
                _selectedOrg;

            set
            {
                _selectedOrg = value;
                OnPropertyChanged(nameof(SelectedOrg));
            }
        }

        private bool _lastUserModified;
        private User _selectedUser;
        public User SelectedUser
        {
            get =>
                _selectedUser;

            set
            {
                if (_selectedUser != null && !_lastUserModified)
                {
                    _selectedUser.role = new Role(_oldUserRole.Equals("ROLE_USER") ? 2 : 1, _oldUserRole.ToString());
                    OnPropertiesChanged(nameof(SelectedUser));
                }

                _lastUserModified = false;
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        private string _oldUserRole = string.Empty;
        private string _selectedRole = string.Empty;
        public Object SelectedRole
        {
            get =>
                _selectedRole;

            set
            {
                _oldUserRole = SelectedUser.Role;
                _selectedRole = ((TextBlock)value).Text;
                SelectedUser.role = new Role(_selectedRole.Equals("ROLE_USER") ? 2 : 1, _selectedRole.ToString());
                OnPropertyChanged(nameof(SelectedRole));
            }
        }

        public AdminPageViewModel(string token) { _token = token; GetAllVolunteersAsync(); }

        #region Observes

        private ObservableCollection<Volunteer> _volunteers;
        private ObservableCollection<Event> _events;
        private ObservableCollection<Organization> _orgs;
        private ObservableCollection<User> _users;

        public IEnumerable<string> UserRoles => new[] { "ROLE_USER", "ROLE_ADMIN" };

        public ObservableCollection<Volunteer> ItemsSourceVols
        {
            get =>
                _volunteers;

            set
            {
                _volunteers = value;
                OnPropertyChanged(nameof(ItemsSourceVols));
            }
        }

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

        public ObservableCollection<Organization> ItemsSourceOrgs
        {
            get =>
                _orgs;

            set
            {
                _orgs = value;
                OnPropertyChanged(nameof(ItemsSourceOrgs));
            }
        }

        public ObservableCollection<User> ItemsSourceUsers
        {
            get =>
                _users;

            set
            {
                _users = value;
                OnPropertyChanged(nameof(ItemsSourceUsers));
            }
        }

        #endregion

        #region Commands

        public ICommand _getAllVolunteersCommand;
        public ICommand _getAllEventsCommand;
        public ICommand _getAllOrgsCommand;
        public ICommand _getAllUsersCommand;
        public ICommand _addVolCommand;
        public ICommand _addEvenCommandt;
        public ICommand _addOrgCommand;
        public ICommand _selectVolCommand;
        public ICommand _selectEventCommand;
        public ICommand _selectOrgCommand;
        public ICommand _selectUserSaveCommand;
        public ICommand _findByFirstNameVolCommand;
        public ICommand _findByEventNameCommand;
        public ICommand _findByOrgNameCommand;
        private ICommand _goBackCommand;

        public ICommand GetAllVolunteersCommand =>
            _getAllVolunteersCommand ?? (_getAllVolunteersCommand = new RelayCommand(async _ => await GetAllVolunteersAsync()));

        public ICommand GetAllEventsCommand =>
            _getAllEventsCommand ?? (_getAllEventsCommand = new RelayCommand(async _ => await GetAllEventsAsync()));

        public ICommand GetAllOrgsCommand =>
            _getAllOrgsCommand ?? (_getAllOrgsCommand = new RelayCommand(async _ => await GetAllOrgsAsync()));

        public ICommand GetAllUsersCommand =>
            _getAllUsersCommand ?? (_getAllUsersCommand = new RelayCommand(async _ => await GetAllUsersAsync()));

        public ICommand AddVolCommand =>
           _addVolCommand ?? (_addVolCommand = new RelayCommand(async _ => await AddVolunteerAsync()));

        public ICommand AddEventCommand =>
           _addEvenCommandt ?? (_addEvenCommandt = new RelayCommand(async _ => await AddEventAsync()));

        public ICommand AddOrgCommand =>
           _addOrgCommand ?? (_addOrgCommand = new RelayCommand(async _ => await AddOrgAsync()));

        public ICommand SelectVolCommand =>
            _selectVolCommand ?? (_selectVolCommand = new RelayCommand(async obj => await SelectVolAsync(obj)));

        public ICommand SelectEventCommand =>
            _selectEventCommand ?? (_selectEventCommand = new RelayCommand(async obj => await SelectEventAsync(obj)));

        public ICommand SelectOrgCommand =>
            _selectOrgCommand ?? (_selectOrgCommand = new RelayCommand(async obj => await SelectOrgAsync(obj)));

        public ICommand SelectUserSaveCommand =>
            _selectUserSaveCommand ?? (_selectUserSaveCommand = new RelayCommand(async _ => await SelectUserSaveAsync()));

        public ICommand FindByFirstNameVolCommand =>
            _findByFirstNameVolCommand ?? (_findByFirstNameVolCommand = new RelayCommand(async obj => await FindByFirstNameVolunteer(obj)));

        public ICommand FindByEventNameCommand =>
            _findByEventNameCommand ?? (_findByEventNameCommand = new RelayCommand(async obj => await FindByEventName(obj)));

        public ICommand FindByOrgNameCommand =>
            _findByOrgNameCommand ?? (_findByOrgNameCommand = new RelayCommand(async obj => await FindByOrgName(obj)));

        public ICommand GoBackCommand =>
            _goBackCommand ?? (_goBackCommand = new RelayCommand(_ => NavigationManager.GoBack()));

        #endregion

        #region Tasks(HttpRequests)

        #region Volunteers

        private async Task GetAllVolunteersAsync()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/admin/volunteers/get_views");
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                string result;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = await streamReader.ReadToEndAsync();
                    ItemsSourceVols = JsonSerializer.Deserialize<ObservableCollection<Volunteer>>(result);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось прочитать данные с сервера!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
            }
        }

        private async Task FindByFirstNameVolunteer(Object obj)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/admin/volunteers/find?first_name=" + (string)obj);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = await streamReader.ReadToEndAsync();
                    var list = JsonSerializer.Deserialize<ObservableCollection<Volunteer>>(result);

                    if (list.Count == 0)
                        MessageBox.Show("Волонтера с фамилией \"" + (string)obj + "\" нет");
                    else
                        ItemsSourceVols = list;
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось прочитать данные с сервера!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
            }
        }

        private async Task<ObservableCollection<Volunteer>> GetVolunteersByEventAsync(long id)
        {
            string address = "http://localhost:8080/api/admin/volunteers/get_by_event?event_id=" + id;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(address);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = await streamReader.ReadToEndAsync();
                    return JsonSerializer.Deserialize<ObservableCollection<Volunteer>>(result);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось прочитать данные с сервера!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
            }

            return null;
        }

        private async Task AddVolunteerAsync()
        {
            VolunteerWindow addWindow = new VolunteerWindow();

            if (addWindow.ShowDialog() == true)
            {
                await AddVolunteerAsync(addWindow.Volunteer);
            }
            else
            {
                MessageBox.Show("Не удалось добавить волонтера!");
            }
        }

        private async Task AddVolunteerAsync(Volunteer volunteer)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/admin/volunteers/adding");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonSerializer.Serialize<Volunteer>(volunteer);

                    await streamWriter.WriteAsync(json);
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    if ((await streamReader.ReadToEndAsync()).Equals("OK"))
                    {
                        MessageBox.Show("Волонтер успешно добавлен!");
                        await GetAllVolunteersAsync();
                    }
                    else
                    {
                        MessageBox.Show("Волонтер не добавлен!");
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
            }
        }

        private async Task SelectVolAsync(Object obj)
        {
            VolunteerWindow volWindow = new VolunteerWindow(obj as Volunteer);
            bool? result = volWindow.ShowDialog();

            if (result == true)
            {
                await AddVolunteerAsync(volWindow.Volunteer);
            }
            else if (result == false && volWindow.IsDeleting)
            {
                await SelectedVolunteerDeleteAsync((obj as Volunteer).id);
            }
        }

        private async Task SelectedVolunteerDeleteAsync(long id)
        {
            string address = "http://localhost:8080/api/admin/volunteers/delete/{" + id + "}";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(address);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);
            byte[] dataStream = Encoding.UTF8.GetBytes("{" + id + "}");
            httpWebRequest.ContentLength = dataStream.Length;

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    await streamWriter.WriteAsync("{" + id + "}");
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = await streamReader.ReadToEndAsync();
                    if (result.Equals("\"OK\""))
                    {
                        MessageBox.Show("Волонтер успешно удален!");
                        await GetAllVolunteersAsync();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении волонтера!");
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
            }
        }

        #endregion

        #region Events

        private async Task GetAllEventsAsync()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/admin/events/get");
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                string result;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = await streamReader.ReadToEndAsync();
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
            }
        }

        private async Task AddEventAsync()
        {
            await GetAllOrgsAsync();
            List<string> orgs = new List<string>(ItemsSourceOrgs.Count);

            foreach (Organization org in ItemsSourceOrgs)
                orgs.Add(org.name);

            EventWindow addWindow = new EventWindow(orgs);

            if (addWindow.ShowDialog() == true)
            {
                await AddEventAsync(addWindow.Event);
            }
            else
            {
                MessageBox.Show("Не удалось добавить мероприятие!");
            }
        }


        private async Task FindByEventName(Object obj)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/admin/events/find?name=" + (string)obj);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = await streamReader.ReadToEndAsync();
                    var list = JsonSerializer.Deserialize<ObservableCollection<Event>>(result);

                    if (list.Count == 0)
                        MessageBox.Show("Мероприятия/й " + (string)obj + " нет");
                    else
                        ItemsSourceEvents = list;
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось прочитать данные с сервера!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
            }
        }

        private async Task AddEventAsync(Event @event)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/admin/events/adding");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonSerializer.Serialize<Event>(@event);

                    await streamWriter.WriteAsync(json);
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    if ((await streamReader.ReadToEndAsync()).Equals("OK"))
                    {
                        MessageBox.Show("Мероприятие успешно добавлено!");
                        await GetAllEventsAsync();
                    }
                    else
                    {
                        MessageBox.Show("Мероприятие не добавлено!");
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
            }
        }

        private async Task SelectEventAsync(Object obj)
        {
            await GetAllOrgsAsync();
            List<string> orgs = new List<string>(ItemsSourceOrgs.Count);

            foreach (Organization org in ItemsSourceOrgs)
                orgs.Add(org.name);

            EventWindow evtWindow = new EventWindow(obj as Event, orgs, new ObservableCollection<Volunteer>((obj as Event).volunteers),
                            new ObservableCollection<Volunteer>((obj as Event).requestedVols));
            bool? result = evtWindow.ShowDialog();

            if (result == true)
            {
                Event @event = evtWindow.Event;
                @event.org = ItemsSourceOrgs.First<Organization>(_ => _.name.Equals(evtWindow.Event.orgName));
                await AddEventAsync(@event);
            }
            else if (result == false && evtWindow.IsDeleting)
            {
                await SelectedEventDeleteAsync((obj as Event).id);
            }
        }

        private async Task SelectedEventDeleteAsync(long id)
        {
            string address = "http://localhost:8080/api/admin/events/delete/{" + id + "}";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(address);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            byte[] dataStream = Encoding.UTF8.GetBytes("{" + id + "}");
            httpWebRequest.ContentLength = dataStream.Length;

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    await streamWriter.WriteAsync("{" + id + "}");
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String result = await streamReader.ReadToEndAsync();
                    if (result.Equals("\"OK\""))
                    {
                        MessageBox.Show("Мероприятие успешно удалено!");
                        await GetAllVolunteersAsync();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении мероприятия!");
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
            }
        }

        private async Task<ObservableCollection<Event>> FindEventsByOrgIdAsync(long id)
        {
            string address = "http://localhost:8080/api/admin/orgs/get_events?id=" + id;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(address);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = await streamReader.ReadToEndAsync();
                    return JsonSerializer.Deserialize<ObservableCollection<Event>>(result);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось прочитать данные с сервера!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
            }

            return null;
        }

        #endregion

        #region Orgs

        private async Task GetAllOrgsAsync()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/admin/orgs/get");
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                string result;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = await streamReader.ReadToEndAsync();
                    ItemsSourceOrgs = JsonSerializer.Deserialize<ObservableCollection<Organization>>(result);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось прочитать данные с сервера!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
            }
        }

        private async Task FindByOrgName(Object obj)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/admin/orgs/find?name=" + (string)obj);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = await streamReader.ReadToEndAsync();
                    var list = JsonSerializer.Deserialize<ObservableCollection<Organization>>(result);

                    if (list.Count == 0)
                        MessageBox.Show("Организатора \"" + (string)obj + "\" нет");
                    else
                        ItemsSourceOrgs = list;
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось прочитать данные с сервера!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
            }
        }

        private async Task AddOrgAsync()
        {
            OrgWindow addWindow = new OrgWindow();

            if (addWindow.ShowDialog() == true)
            {
                await AddOrgAsync(addWindow.Org);
            }
            else
            {
                MessageBox.Show("Не удалось добавить волонтера!");
            }
        }

        private async Task AddOrgAsync(Organization org)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/admin/orgs/adding");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonSerializer.Serialize<Organization>(org);

                    await streamWriter.WriteAsync(json);
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    if ((await streamReader.ReadToEndAsync()).Equals("OK"))
                    {
                        MessageBox.Show("Организатор успешно добавлен!");
                        await GetAllOrgsAsync();
                    }
                    else
                    {
                        MessageBox.Show("Организатор не добавлен!");
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
            }
        }

        private async Task SelectOrgAsync(Object obj)
        {
            OrgWindow orgWindow = new OrgWindow(obj as Organization, (await FindEventsByOrgIdAsync((obj as Organization).id)));
            bool? result = orgWindow.ShowDialog();

            if (result == true)
            {
                await AddOrgAsync(orgWindow.Org);
            }
            else if (result == false && orgWindow.IsDeleting)
            {
                await SelectedOrgDeleteAsync((obj as Organization).id);
            }
        }

        private async Task SelectedOrgDeleteAsync(long id)
        {
            string address = "http://localhost:8080/api/admin/orgs/delete/{" + id + "}";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(address);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);
            byte[] dataStream = Encoding.UTF8.GetBytes("{" + id + "}");
            httpWebRequest.ContentLength = dataStream.Length;

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    await streamWriter.WriteAsync("{" + id + "}");
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String result = await streamReader.ReadToEndAsync();
                    if (result.Equals("\"OK\""))
                    {
                        MessageBox.Show("Организатор успешно удален!");
                        await GetAllVolunteersAsync();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении организатора!");
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
            }
        }

        #endregion

        #region Users

        private async Task GetAllUsersAsync()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/admin/users/get");
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = await streamReader.ReadToEndAsync();
                    ItemsSourceUsers = JsonSerializer.Deserialize<ObservableCollection<User>>(result);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось прочитать данные с сервера!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
            }
        }

        private async Task SelectUserSaveAsync()
        {
            string address = "http://localhost:8080/api/admin/users/set_role?id=" + SelectedUser.id + "&role=" + SelectedUser.Role;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(address);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "Post";
            httpWebRequest.Headers.Add("Authorization", "Bearer_" + _token);

            byte[] dataStream = Encoding.UTF8.GetBytes("?id=" + SelectedUser.id + "&role=" + SelectedUser.Role);
            httpWebRequest.ContentLength = dataStream.Length;

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    await streamWriter.WriteAsync("?id=" + SelectedUser.id + "&role=" + SelectedUser.Role);
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    String result = await streamReader.ReadToEndAsync();
                    if (result.Equals("OK"))
                    {
                        MessageBox.Show("Роль пользователя изменена!");
                        _lastUserModified = true;
                        await GetAllVolunteersAsync();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при изменении роли пользователя!");
                    }
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Не удалось прочитать данные с сервера!");
            }
            catch (WebException)
            {
                MessageBox.Show("Не удалось связаться с сервером!");
            }
        }

        #endregion

        #endregion
    }
}