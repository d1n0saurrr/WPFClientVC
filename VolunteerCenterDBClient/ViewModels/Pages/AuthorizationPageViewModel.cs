using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VolunteerCenterDBClient.Commands;
using VolunteerCenterDBClient.Models;
using VolunteerCenterDBClient.Navigation;
using VolunteerCenterDBClient.Views.Pages;

namespace VolunteerCenterDBClient.ViewModels.Pages
{
    internal sealed class AuthorizationPageViewModel : BaseViewModel
    {
        private string _passwordConfirmation = string.Empty;
        private ICommand _authorizeCommand;
        private ICommand _registerCommand;
        User user = new User(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

        public string Username
        {
            private get =>
                user.username;

            set
            {
                user.username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            private get =>
                user.password;

            set
            {
                user.password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string FirstName
        {
            private get =>
                user.firstName;

            set
            {
                user.firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            private get =>
                user.lastName;

            set
            {
                user.lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string Patronymic
        {
            private get =>
                user.patronymic;

            set
            {
                user.patronymic = value;
                OnPropertyChanged(nameof(Patronymic));
            }
        }

        public string Email
        {
            private get =>
                user.email;

            set
            {
                user.email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string PasswordConfirmation
        {
            get =>
                _passwordConfirmation;

            set
            {
                _passwordConfirmation = value;
                OnPropertyChanged(nameof(PasswordConfirmation));
            }
        }

        public ICommand AuthorizeCommand =>
            _authorizeCommand ?? (_authorizeCommand = new RelayCommand(async _ => await AuthorizeAsync(),
                _ => CanAuthorize()));

        public ICommand RegisterCommand =>
            _registerCommand ?? (_registerCommand = new RelayCommand(async _ => await RegisterAsync(),
                _ => CanRegister()));

        private async Task AuthorizeAsync()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/auth");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonSerializer.Serialize(new
                    {
                        username = user.username,
                        password = user.password
                    });

                    await streamWriter.WriteAsync(json);
                }

                var httpResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = await streamReader.ReadToEndAsync();
                    Dictionary<string, string> jsonToken = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
                    
                    if (jsonToken["role"] == "ROLE_ADMIN")
                        NavigationManager.Navigate<AdminMainPage>(new AdminPageViewModel(jsonToken["token"]));
                    else
                        NavigationManager.Navigate<UserMainPage>(new UserMainPageViewModel(jsonToken["token"], long.Parse(jsonToken["id"])));
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

        private async Task RegisterAsync()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/register");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonSerializer.Serialize<User>(user);

                    await streamWriter.WriteAsync(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string result;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = await streamReader.ReadToEndAsync();
                    if (result.Equals("OK"))
                    {
                        await FindVolunteerForUser();
                        MessageBox.Show("Вы были зарегистрированы, пожалуйста, авторизуйтесь!");
                    }
                    else
                        MessageBox.Show("Такой пользователь уже существует!");
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

        private async Task FindVolunteerForUser()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/auth");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonSerializer.Serialize(new
                {
                    username = user.username,
                    password = user.password
                });

                await streamWriter.WriteAsync(json);
            }

            var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = await streamReader.ReadToEndAsync();
                Dictionary<string, string> jsonToken = JsonSerializer.Deserialize<Dictionary<string, string>>(result);

                HttpWebRequest httpWebRequest1 = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/user/volunteer/set?id="
                        + jsonToken["id"]);
                httpWebRequest1.ContentType = "application/json";
                httpWebRequest1.Method = "POST";
                httpWebRequest1.Headers.Add("Authorization", "Bearer_" + jsonToken["token"]);

                var result1 = (HttpWebResponse)await httpWebRequest1.GetResponseAsync();
            }
        }

        private bool CanAuthorize()
        {
            return Username.Length > 0 && Password.Length > 0;
        }

        private bool CanRegister()
        {
            return !string.IsNullOrEmpty(FirstName)
                && !string.IsNullOrEmpty(LastName)
                && !string.IsNullOrEmpty(Patronymic)
                && !string.IsNullOrEmpty(Email)
                && !string.IsNullOrEmpty(Username)
                && !string.IsNullOrEmpty(Password)
                && Password.Equals(PasswordConfirmation);
        }

    }
}
