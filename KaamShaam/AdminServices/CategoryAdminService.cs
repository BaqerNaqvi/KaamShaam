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
        public static void UpdateCategory(Category obj)
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
                context.Categories.AddOrUpdate(data);
                context.SaveChanges();
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
    }
}