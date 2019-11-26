using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Utility.Importer.Contracts.Contracts
{
    public interface IImporterContext
    {
        string ImportProduct(ImportInputParamater input);
        //string ImportProductWithFile(QueryInputParamaterWithFile input);
        string ImportProductForSP(ImportInputParamater input);
    }
}
