using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;
using KaamShaam.Models;

namespace KaamShaam.Services
{
    public static class AppointmentService
    {
        public static void AddAppointment(CustomAppoinmentModel source)
        {
            using (var db= new KaamShaamEntities())
            {
                db.Appointments.Add(new Appointment
                {
                    DateTime = source.DateTime,
                    CreatedBy = source.CreatedBy,
                    Title = source.Title,
                    Type = source.Type,
                    WithId = source.WithId,
                    IsAttended = false
                });
                db.SaveChanges();
            }
        }

        public static void EditAppointment(CustomAppoinmentModel source)
        {
            using (var db = new KaamShaamEntities())
            {
                var dbObj = db.Appointments.FirstOrDefault(app => app.Id == source.Id);
                if (dbObj != null)
                {
                    dbObj.DateTime = source.DateTime;
                    dbObj.Title = source.Title;
                    dbObj.Type = source.Type;
                    dbObj.WithId = source.WithId;
                    dbObj.IsAttended = source.IsAttended;
                }               
                db.SaveChanges();
            }
        }

        public static void DeleteAppointment(CustomAppoinmentModel source)
        {
            using (var db = new KaamShaamEntities())
            {
                var dbObj = db.Appointments.FirstOrDefault(app => app.Id == source.Id);
                if (dbObj != null)
                {
                    db.Appointments.Remove(dbObj);
                }
                db.SaveChanges();
            }
        }

        public static List<CustomAppoinmentModel> GetDaAppoinments(DateTime date, string postedById)
        {
            using (var db = new KaamShaamEntities())
            {
                var apps =db.Appointments.Where(app => DbFunctions.TruncateTime(app.DateTime) == date.Date
                && (app.WithId == postedById || app.CreatedBy == postedById) && !app.IsAttended).ToList();
                if (apps.Any())
                {
                    return apps.Select(hmm => hmm.Mapper()).ToList();
                }
            }
            return null;
        }

        public static List<AppointmentEventModel> GetUserAppoinments(string postedById)
        {
            using (var db = new KaamShaamEntities())
            {
                var apps = db.Appointments.Where(app=>(app.WithId == postedById || app.CreatedBy == postedById) && !app.IsAttended).ToList();
                if (apps.Any())
                {
                    return apps.Select(hmm => hmm.MapperEvent()).ToList();
                }
            }
            return null;
        }
    }
}