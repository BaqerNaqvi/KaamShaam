using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaamShaam.AdminModels
{
    public class MakeAdminModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string Role { get; set; }

        public bool Status { get; set; }
    }
}