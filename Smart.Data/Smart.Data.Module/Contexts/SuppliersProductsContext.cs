using ACoreX.Data.Abstractions;
using ACoreX.WebAPI;
using Newtonsoft.Json;
using Smart.Data.Abstractions.Contracts;
using Smart.Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Smart.Data.Module.Contexts
{
    public class SuppliersProductsContext: ISuppliersProductsContext
    {
        private IData _data;
        public SuppliersProductsContext(IData input)
        {
            _data = input;
        }

        [WebApi(Route = "api/CRM/SuppliersProducts/Delete", Authorized = false, Method = WebApiMethod.Post)]
        public int delete(ProductsSuppliersInputModel data)
        {

            using (SqlConnection cnn = _data.OpenConnection())
            {
                try
                {
                    DBParam p0 = new DBParam();
                    p0.Name = "@productsId";
                    p0.Value = Newtonsoft.Json.JsonConvert.SerializeObject(data.ProductsId);

                    DBParam p1 = new DBParam();
                    p1.Name = "@suppliersId";
                    p1.Value = Newtonsoft.Json.JsonConvert.SerializeObject(data.SuppliersId);


                    var result = _data.Execute("[CRM].[CRM_SP_PARTNERS_PRODUCTS_DELETE]", System.Data.CommandType.StoredProcedure, p0, p1);


                    _data.Dispose();
                    return result;
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    _data.Dispose();
                }
            }
        }

        [WebApi(Route = "api/CRM/SuppliersProducts/Insert", Authorized = false, Method = WebApiMethod.Post)]
        public int insert(ProductsSuppliersInputModel data)
        {
            using (SqlConnection cnn = _data.OpenConnection())
            {
                try
                {
                    DBParam p0 = new DBParam();
                    p0.Name = "@productsId";
                    p0.Value =  Newtonsoft.Json.JsonConvert.SerializeObject(data.ProductsId);


                    DBParam p1 = new DBParam();
                    p1.Name = "@suppliersId";
                    p1.Value =  Newtonsoft.Json.JsonConvert.SerializeObject(data.SuppliersId);


                    var result = _data.Execute("[CRM].[CRM_SP_PARTNERS_PRODUCTS_INSERT]", System.Data.CommandType.StoredProcedure, p0, p1);


                    _data.Dispose();
                    return result;
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    _data.Dispose();
                }
            }



        }

       
    }
}
