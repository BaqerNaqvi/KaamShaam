using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;
using KaamShaam.LocalModels;

namespace KaamShaam.Services
{
    public static class CategoryService
    {
        public static List<LocalCategory> GetAllCategories()
        {
            using (var dbContext = new KaamShaamEntities())
            {
                var dbCats = dbContext.Categories.
                    Where(cat => cat.Status && cat.IsApproved).ToList().Select( pbj => pbj.Mapper()).ToList();
                return dbCats;
            }
        }
       

        public static LocalCategory GetCategoryById(long? id)
        {
            if (id != null)
            {
                using (var dbContext = new KaamShaamEntities())
                {
                    var dbCats = dbContext.Categories.FirstOrDefault(obj => obj.Id == id).Mapper();
                    return dbCats;
                }
            }
            return null;
        }
    }
}