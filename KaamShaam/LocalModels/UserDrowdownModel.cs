using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.DbEntities;

namespace KaamShaam.LocalModels
{
    public class UserDrowdownModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }
    }

    public static class Mapper
    {
        public static UserDrowdownModel MappDD(this AspNetUser source)
        {
            return new UserDrowdownModel
            {
                UserId = source.Id,
                UserName = source.FullName
            };
        }
    }
}