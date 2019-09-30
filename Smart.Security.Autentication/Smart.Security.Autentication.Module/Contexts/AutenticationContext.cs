using ACoreX.WebAPI;
using Smart.Security.Autentication.Abstractions.Contracts;
using Smart.Security.Autentication.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Security.Autentication.Contexts
{
    public class AutenticationContext : IAutenticationContext
    {
        [WebApi(Route = "/api/auth/sign", Method = WebApiMethod.Get, Authorized = true)]
        public SignInOutputModel SignIn()
        {
            return new SignInOutputModel
            {
                DiaplayName = "Arash",
                LastLoggedTime = DateTime.Now.AddDays(-2)
            };
        }
    }
}
