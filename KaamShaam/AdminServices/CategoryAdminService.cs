using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;
using KaamShaam.LocalModels;

namespace KaamShaam.AdminServices
{
    public static class CategoryAdminService
    {
        public static List<LocalCategory> GetCategories()
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbCats = dbContext.Categories.ToList().Select(pbj => pbj.Mapper()).ToList();
                return dbCats;
            }
        }
        public static long UpdateCategory(Category obj)
        {
            using (var context = new KaamShaamEntities())
            {
                var data = context.Categories.FirstOrDefault(x => x.Id == obj.Id);
                if (data == null)
                {
                    data = new Category { Name = obj.Name, Status = true };
                }
                else
                {
                    data.Name = obj.Name;
                }
                data.IsApproved = false;
                data.EditedAt = DateTime.Now;
                data.Feedback = null;
                data.EditedAt = DateTime.Now;
                context.Categories.AddOrUpdate(data);
                context.SaveChanges();
                return data.Id;
            }
        }
        public static void UpdateStatus(Category obj)
        {
            using (var context = new KaamShaamEntities())
            {
                var data = context.Categories.FirstOrDefault(x => x.Id == obj.Id);
                if (data != null)
                {
                    data.Status = obj.Status;
                }
                context.Categories.AddOrUpdate(data);
                context.SaveChanges();
            }
        }
        public static void DeleteCat(Category obj)
        {
            using (var context = new KaamShaamEntities())
            {
                var data = context.Categories.FirstOrDefault(x => x.Id == obj.Id);
                if (data != null)
                {
                    context.Categories.Remove(data);
                    context.SaveChanges();
                }

            }
        }

        public static List<LocalCategory> GetNotApprovedCategories()
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbCats = dbContext.Categories.Where( cat => cat.Status && !cat.IsApproved && string.IsNullOrEmpty(cat.Feedback)).ToList().Select(pbj => pbj.Mapper()).ToList();
                return dbCats;
            }
        }

        public static void ApprovalStatus(LocalCategory obj)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbJob = dbcontext.Categories.FirstOrDefault(l => l.Id == obj.Id);
                if (dbJob != null)
                {
                    dbJob.IsApproved = obj.IsApproved;
                    if (!obj.IsApproved)
                    {
                        dbJob.Feedback = obj.Feedback;
                    }
                    dbcontext.SaveChanges();
                }
            }
        }
    }
}