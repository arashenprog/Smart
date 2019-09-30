using Smart.Security.Autentication.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Security.Autentication.Abstractions.Contracts
{
    public interface IAutenticationContext
    {
        SignInOutputModel SignIn();
       
    }
}
