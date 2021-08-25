using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VolunteerCenterDBClient.Models
{
    public class User
    {
        public long id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string patronymic { get; set; }
        public string email { get; set; }
        public Role role { get; set; }
        public string password { get; set; }
        public Volunteer volunteer { get; set; }

        [JsonIgnore]
        public string Role { get { return role.name; } set { } }

        public User() { }

        public User(string username, string firstName, string lastName, string patronymic, string email, string password)
        {
            this.username = username;
            this.firstName = firstName;
            this.lastName = lastName;
            this.patronymic = patronymic;
            this.email = email;
            this.password = password;
        }
    }
}
