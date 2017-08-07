using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;

namespace KaamShaam.LocalModels
{
    public class LocalCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
    }

    public static class CategoryMapper
    {
        public static LocalCategory Mapper(this Category source)
        {
            return new LocalCategory
            {
                Id = source.Id,
                Name = source.Name,
                Status = source.Status
            };
        }
    }

}