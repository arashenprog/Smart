using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Utility.Importer.Contracts.Contracts
{
    public interface IUploadContext
    {
        MemoryStream UploadBase64(UploadBase upload);
        string UploadFile();
    }
}
