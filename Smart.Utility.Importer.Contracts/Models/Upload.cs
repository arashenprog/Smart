using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Utility.Importer.Contracts.Models
{
    public class UploadBase
    {
        public string Name { get; set; }
        public string Base64String { get; set; }
    }
    public class UploadFile
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}
