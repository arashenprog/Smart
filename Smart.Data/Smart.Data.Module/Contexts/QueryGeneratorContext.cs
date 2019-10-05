using ACoreX.Data.Abstractions;
using ACoreX.WebAPI;
using Smart.Data.Abstractions.Contracts;
using Smart.Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smart.Data.Module.Contexts
{
    public class QueryGeneratorContext : IQueryGenerator
    {
        IData _data;
        public QueryGeneratorContext(IData data)
        {
            _data = data;
        }
        [WebApi(Route = "api/getDS", Authorized = false, Method = WebApiMethod.Post)]
        public QueryGeneratedCommand Generate(QueryInputParamater input)
        {

            var result = _data.Query<DataSourceInput>("SELECT Top 1 * FROM ADM.ADM_DATA_SOURCES WHERE DSRC_COD=@P1",
                new DBParam { Name = "@P1", Value = input.Code }).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select * from {0}", result.DSRC_SRC);
            return new QueryGeneratedCommand
            {
                Command = "Hi"
            };
        }
    }
}
