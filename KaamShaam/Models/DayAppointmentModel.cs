using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaamShaam.Models
{
    public class DayAppointmentModel
    {
        public List<CustomAppoinmentModel> Appoinments { get; set; }
        public string DateName { get; set; }
    }
}