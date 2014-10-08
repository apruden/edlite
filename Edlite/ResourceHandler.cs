namespace Edlite
{
    using System;
    using System.IO;
    using System.Resources;
    using System.Text;
    using System.Web;

    /// <summary>
    /// 
    /// </summary>
    public class ResourceHandler :  IHttpHandler
    {
        private string name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public ResourceHandler(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            HttpResponse response = context.Response;

            switch (name)
            {
                case "script":
                    response.ContentType = "application/javascript";
                    break;
                case "style":
                    response.ContentType = "text/css";
                    break;
                default:
                    response.ContentType = "text/html";
                    break;
            }
            
            this.WriteResourceToStream(response.OutputStream, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputStream"></param>
        /// <param name="resourceName"></param>
        public void WriteResourceToStream(Stream outputStream, string resourceName)
        {
            //using (Stream inputStream = typeof(ResourceHandler).Assembly.GetManifestResourceStream(
            //    typeof(ResourceHandler), "Edlite.assets." + resourceName))
            string res = new ResourceManager(typeof(WebResource)).GetObject(resourceName) as string;

            using(Stream inputStream = new MemoryStream(Encoding.UTF8.GetBytes(res)))
            {
                byte[] buffer = new byte[Math.Min(inputStream.Length, 4096)];
                int readLength = inputStream.Read(buffer, 0, buffer.Length);

                while (readLength > 0)
                {
                    outputStream.Write(buffer, 0, readLength);
                    readLength = inputStream.Read(buffer, 0, buffer.Length);
                }
            }
        }
    }
}
