namespace Edlite
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// 
    /// </summary>
    public class EdliteHandlerFactory : IHttpHandlerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestType"></param>
        /// <param name="url"></param>
        /// <param name="pathTranslated"></param>
        /// <returns></returns>
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            string resource = context.Request.PathInfo.Length == 0 ? string.Empty :
               context.Request.PathInfo.Substring(1).ToLower(CultureInfo.InvariantCulture);

            IHttpHandler handler = FindHandler(resource);

            if (handler == null)
                throw new HttpException(404, "Resource not found.");

            return handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public void ReleaseHandler(IHttpHandler handler)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static IHttpHandler FindHandler(string name)
        {
            Debug.Assert(name != null);

            switch (name)
            {
                case "style":
                    return new ResourceHandler("style");
                case "script":
                    return new ResourceHandler("script");
                case "db":
                    return new AdminHandler();
                default:
                    return name.Length == 0 ? new ResourceHandler("admin") : null;
            }
        }
    }
}
