using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.AdminModels;
using KaamShaam.DbEntities;

namespace KaamShaam.Services
{
    public static class BannerService
    {
        public static List<Banner> GetAllBanners()
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                return dbcontext.Banners.OrderBy(banner => banner.ShowOrder).ToList();
            }
        }

        public static void DeleteBanner(long bId)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbObj = dbcontext.Banners.FirstOrDefault(b => b.Id == bId);
                if (dbObj != null)
                {
                    dbcontext.Banners.Remove(dbObj);
                }
            }
        }

        public static void AddBanner(Banner obj)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                dbcontext.Banners.Add(obj);
                dbcontext.SaveChanges();
            }
        }

        public static void EditBanner(BannerModel obj)
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                var dbObj = dbcontext.Banners.FirstOrDefault(b => b.Id == obj.Id);
                if (dbObj != null)
                {
                    dbObj.Status = obj.Status;
                    dbObj.ShowOrder = obj.ShowOrder;
                    dbcontext.SaveChanges();
                }
            }
        }

        public static List<Banner> GetActiveBanners()
        {
            using (var dbcontext = new KaamShaamEntities())
            {
                return dbcontext.Banners.Where(b => b.Status).OrderBy(banner => banner.ShowOrder).ToList();
            }
        }
    }
}