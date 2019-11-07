using ACoreX.Data.Abstractions;
using ACoreX.WebAPI;
using ACoreX.Data.Abstractions;
using ACoreX.WebAPI;
using Newtonsoft.Json;
using Smart.Data.Abstractions.Contracts;
using Smart.Data.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Smart.Data.Module.Contexts
{
    public class EntitiesNoteContext : IEntitiesNoteContext
    {
        private IData _data;
        public EntitiesNoteContext(IData input)
        {
            _data = input;
        }
        [WebApi(Route = "api/CRM/EntitiesNote/Insert", Authorized = false, Method = WebApiMethod.Post)]

        public int InsertNote(EntitiesNoteInputModel data)
        {
            
            using (SqlConnection cnn = _data.OpenConnection())
            {
                try
                {
                    DBParam p0 = new DBParam();
                    p0.Name = "@refId";
                    p0.Value = data.RefId;

                    DBParam p1 = new DBParam();
                    p1.Name = "@date";
                    p1.Value = Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); ;

                    DBParam p2 = new DBParam();
                    p2.Name = "@note";
                    p2.Value = data.Note;

                    DBParam p3 = new DBParam();
                    p3.Name = "@userId";
                    p3.Value = data.UserId;
                    p3.IsNullable = true;

                    var result = _data.Execute("[CRM].[CRM_SP_ENTITIES_NOTE_INSERT]", p0,p1,p2,p3);


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
