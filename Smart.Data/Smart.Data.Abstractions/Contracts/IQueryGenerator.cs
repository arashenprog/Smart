using Smart.Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Abstractions.Contracts
{
    public interface IQueryGenerator
    {
        QueryGeneratedCommand Generate(QueryInputParamater input);
    }
}
