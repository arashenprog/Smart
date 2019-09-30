using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Security.Autentication.Abstractions.Models
{
    public class SignInInputModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    public class SignInOutputModel
    {
        public string DiaplayName { get; set; }
        public DateTime LastLoggedTime { get; set; }
    }
}
