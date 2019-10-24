using ACoreX.Data.Abstractions;
using ACoreX.WebAPI;
using Crm.Data.Entities;
using CRM.Data.Entities;
using OfficeOpenXml;
using Smart.Utility.Importer.Contracts.Contracts;
using Smart.Data.Module.Contexts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Smart.Data.Abstractions.Models;

namespace Smart.Utility.Importer.Module.Contexts
{
    public class ImporterContexts : IImporterContext
    {

        private IData _data;

        public ImporterContexts(IData data)
        {
            _data = data;
        }


        [WebApi(Route = "/api/utility/import", Method = WebApiMethod.Post)]
        public string ImportProduct(QueryInputParamater input)
        {
            //var getProductFields = _data.Query<EntityField>("select * from ADM.ADM_ENTITY_FIELDS where ETFD_ENTT_ID = @P1",
            //                    new DBParam { Name = "@P1", Value = "e4350589-45eb-e911-b511-d850e641f96f" });
            var importProfileFields = new QueryGeneratorContext(_data).Generate(input);



            string filePath = @"D:/test.xlsx";
            FileInfo file = new FileInfo(filePath);



            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                var rawText = string.Empty;
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("INSERT INTO {0} (", importProfileFields[0].EntitySource);
                //foreach (var item in importProfileFields)
                //{
                //    sb.AppendFormat("{0},", item.EntityFieldName);

                //}

                for (int col = 1; col <= ColCount; col++)
                {
                    foreach (var item in importProfileFields)
                    {
                        if (item.EntityImportFieldExcelColName == worksheet.Cells[1, col].Value.ToString())
                        {
                            sb.AppendFormat("{0},", item.EntityImportFieldExcelColName);
                        }
                    }
                }
                sb.Length--;


                sb.Length--;
                sb.AppendFormat(" )");
                sb.AppendFormat(" VALUES ");

                StringBuilder valuesStr = new StringBuilder();

                for (int row = 2; row <= rowCount; row++)
                {
                    valuesStr.AppendFormat("(");
                    for (int col = 1; col <= ColCount; col++)
                    {
                        //if (importProfileFields[col - 1].EntityImportFieldExcelColName == worksheet.Cells[1, col].Value.ToString())
                        valuesStr.AppendFormat("{0},", worksheet.Cells[row, col].Value.ToString());
                    }
                    valuesStr.Length--;
                    valuesStr.AppendFormat("),");
                }
                valuesStr.Length--;
                //var Value = "Values (";
                //for (int col = 1; col <= ColCount; col++)
                //{
                //    Value = worksheet.Cells[1, col].Value.ToString();

                //foreach (var item in importProfileFields)
                //{
                //    if (Value == item.EntityImportFieldExcelColName)
                //    {

                //    }
                //    sb.AppendFormat(" {0}, ", item.EntityFieldName);

                //}


                //}

                //for (int col = 1; col <= ColCount; col++)
                //{
                // This is just for demo purposes
                // rawText += worksheet.Cells[row, col].Value.ToString() + "\t";



                string res = sb.Append(valuesStr).ToString();


                //        var result = _data.Query<Products>(" {EntitySource} (PRDT_NAME,	PRDT_TYPE,	PRDT_BRAND) VALUES(@P1,@P2,@P3); ",
                //                    new DBParam { Name = "@P1", Value = worksheet.Cells[row, 1].Value.ToString() },
                //                    new DBParam { Name = "@P2", Value = worksheet.Cells[row, 2].Value.ToString() },
                //                    new DBParam { Name = "@P3", Value = worksheet.Cells[row, 3].Value.ToString() }
                //);
                //}
                rawText += "\r\n";

                return rawText;
            }
        }
    }
}
