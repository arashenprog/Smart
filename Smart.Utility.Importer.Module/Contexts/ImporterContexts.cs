﻿using ACoreX.Data.Abstractions;
using ACoreX.WebAPI;
using CRM.Data.Entities;
using OfficeOpenXml;
using Smart.Utility.Importer.Contracts.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smart.Utility.Importer.Module.Contexts
{
    public class ImporterContexts : IImporterContext
    {

        private IData _data;

        public ImporterContexts(IData data)
        {
            _data = data;
        }

        [WebApi(Route = "/api/ie")]
        public string ImportExcel()
        {
            var filePath = @"D:/test.xlsx";
            FileInfo file = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                var rawText = string.Empty;
                for (int row = 2; row <= rowCount; row++)
                {
                    //for (int col = 1; col <= ColCount; col++)
                    //{
                        // This is just for demo purposes
                       // rawText += worksheet.Cells[row, col].Value.ToString() + "\t";
                        var result = _data.Query<Product>("INSERT INTO CRM.CRM_PRODUCT (PRDT_NAME,	PRDT_TYPE,	PRDT_BRAND) VALUES(@P1,@P2,@P3); ",
                new DBParam { Name = "@P1", Value = worksheet.Cells[row, 1].Value.ToString() },
                new DBParam { Name = "@P2", Value = worksheet.Cells[row, 2].Value.ToString() },
                new DBParam { Name = "@P3", Value = worksheet.Cells[row, 3].Value.ToString() }
                );
                    //}
                    rawText += "\r\n";
                }
                return rawText;
            }
        }
    }
}