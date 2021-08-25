using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolunteerCenterDBClient.Models
{
    public class Organization
    {
        public long id { get; set; }
        public string name { get; set; }
        public string contactPerson { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
    }
}