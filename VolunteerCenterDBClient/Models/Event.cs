using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VolunteerCenterDBClient.Models
{
    public class Event
    {
        public long id { get; set; }
        public string name { get; set; }
        public string shortName { get; set; }
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public Organization org { get; set; }
        public List<Volunteer> volunteers { get; set; }
        public List<Volunteer> requestedVols { get; set; }

        [JsonIgnore]
        public string orgName { get; set; }

        [JsonIgnore]
        public string StartDay { get { return dateStart.ToString("dd.MM.yyyy"); } }

        [JsonIgnore]
        public string EndDay { get { return dateEnd.ToString("dd.MM.yyyy"); } }
    }
}
