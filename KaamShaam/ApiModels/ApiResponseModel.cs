using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace KaamShaam.ApiModels
{
    public class ApiResponseModel
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public string JToken { get; set; }
    }
}