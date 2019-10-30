using ACoreX.WebAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Smart.Utility.Importer.Contracts.Contracts;
using Smart.Utility.Importer.Contracts.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Utility.Importer.Module.Contexts
{
    public class UploadContext : IUploadContext

    {
        [WebApi(Route = "/api/utility/upload", Method = WebApiMethod.Post)]
        public MemoryStream UploadBase64(Upload upload)
        {
            if (upload.base64String == null || upload.base64String.Length == 0)
                throw new Exception("Please select the File");

            var fileDataByteArray = Convert.FromBase64String(upload.base64String);
            var fileDataStream = new MemoryStream(fileDataByteArray);

            return fileDataStream;
        }
    }
}
