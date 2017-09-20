using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.LocalModels;

namespace KaamShaam.Models
{
    public class AppointmentEventModel
    {
        public string title { get; set; }

        public string start { get; set; }

        public string appointmentWithId { get; set; }

        public string appointmentWithName { get; set; }

        public string appointmentWithPhone { get; set; }

        public DateTime startDateTime { get; set; }
    }
}