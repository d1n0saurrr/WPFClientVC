using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VolunteerCenterDBClient.Models
{
    public class Volunteer
    {
        public long id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string patronymic { get; set; }
        public Nullable<DateTime> dateOfBirth { get; set; }
        public int loyaltylevel { get; set; }
        public string loyalty
        {
            get
            {
                switch (loyaltylevel)
                {
                    case 1:
                        return "Обычный";

                    case 2:
                        return "Серебряный";

                    case 3:
                        return "Золотой";

                    default:
                        return "Неопределен";
                }
            }
        }

        public string sex { get; set; }
        public string citizenship { get; set; }
        public string institute { get; set; }
        //public string courseOfStudy { get; set; }
        //public string formOfStudy { get; set; }
        //public string budgetEducation { get; set; }
        public string studyGroup { get; set; }
        public string yearOfGraduation { get; set; }
        //public string clothingSize { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        //public bool employment { get; set; }
        //public List<string> activities { get; set; }
        public List<Event> events { get; set; }
        public List<Event> requestedEvents { get; set; }
        //public List<string> languages { get; set; }
        //public List<string> mediaSkills { get; set; }
        //public List<string> scholarships { get; set; }

        [JsonIgnore]
        public string DateOfBirth { get { return dateOfBirth.GetValueOrDefault().ToString("dd.MM.yyyy"); } }

        public Volunteer() { }

        public Volunteer(string firstName, string lastName, string patronymic, DateTime? dateOfBirth, string sex, string citizenship, string institute, string studyGroup, string yearOfGraduation, string telephone)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.patronymic = patronymic;
            this.dateOfBirth = dateOfBirth;
            this.sex = sex;
            this.citizenship = citizenship;
            this.institute = institute;
            this.studyGroup = studyGroup;
            this.yearOfGraduation = yearOfGraduation;
            this.telephone = telephone;
        }
    }
}
