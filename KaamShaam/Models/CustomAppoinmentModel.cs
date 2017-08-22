using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;

namespace KaamShaam.Models
{
    public class CustomAppoinmentModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public System.DateTime DateTime { get; set; }
        public string WithId { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string WithName { get; set; }
        public bool IsAttended { get; set; }

    }

    public static class AppointmentMapper
    {
        public static CustomAppoinmentModel Mapper(this Appointment source)
        {
            return new CustomAppoinmentModel
            {
                Id = source.Id,
                DateTime = source.DateTime,
                CreatedBy = source.CreatedBy,
                CreatedByName = source.CreatedByUser.FullName,
                Title = source.Title,
                Type = source.Type,
                WithId = source.WithId,
                WithName = source.WithUser.FullName,
                IsAttended= source.IsAttended
            };
        }

        public static AppointmentEventModel MapperEvent(this Appointment source)
        {
          return   new AppointmentEventModel
          {
              title = source.Title+"-"+source.Type,
              start = source.DateTime.ToString("yyyy-MM-dd")
          };
        }
    }
}