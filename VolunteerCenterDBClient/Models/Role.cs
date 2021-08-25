using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolunteerCenterDBClient.Models
{
    public class Role
    {
        public long id { get; set; }
        public string name { get; set; }

        public Role() { }

        public Role(int id, string name) { this.id = id; this.name = name; }
    }
}
