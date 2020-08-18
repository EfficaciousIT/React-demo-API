using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace NewDemo.Models
{
    public class CustomUploadMultipartFormProvider : MultipartFormDataStreamProvider
    {
        string dtr2;
        public CustomUploadMultipartFormProvider(string path, string extension) : base(path)
        {
            dtr2 = extension;
        }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            if (headers != null && headers.ContentDisposition != null)
            {
                string filename = dtr2;

                return filename;
            }

            return base.GetLocalFileName(headers);
        }
    }
}