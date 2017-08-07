using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KaamShaam.Models;

namespace KaamShaam.LocalModels
{
    public class RegisterPageWraper
    {
        public RegisterViewModel RegisterViewModel { get; set; }

        public LoginViewModel LoginViewModel { get; set; }

        public List<LocalUser> Vendors { get; set; }

        public List<LocalCategory> Categories { get; set; }
    }
}