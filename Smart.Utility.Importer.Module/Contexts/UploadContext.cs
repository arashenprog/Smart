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
        [WebApi(Route = "/api/utility/UploadBase", Method = WebApiMethod.Post)]
        public MemoryStream UploadBase64(UploadBase upload)
        {
            if (upload.Base64String == null || upload.Base64String.Length == 0)
                throw new Exception("Please select the File");

            var fileDataByteArray = Convert.FromBase64String(upload.Base64String);
            var fileDataStream = new MemoryStream(fileDataByteArray);

            fileDataStream.CopyTo(fileDataStream, 100);
            return fileDataStream;
        }
        //[WebApi(Route = "/api/utility/UploadFile", Method = WebApiMethod.Post)]
        //public MemoryStream UploadFile(UploadBase upload)
        //{
        //    if (upload.Base64String == null || upload.Base64String.Length == 0)
        //        throw new Exception("Please select the File");

        //    var fileDataByteArray = Convert.FromBase64String(upload.Base64String);
        //    var fileDataStream = new MemoryStream(fileDataByteArray);

        //    fileDataStream.CopyTo(fileDataStream, 100);
        //    return fileDataStream;
        //}

        [WebApi(Route = "/api/utility/test", Method = WebApiMethod.Get)]
        public string UploadFile()
        {
            return "";
        }
    }
}
