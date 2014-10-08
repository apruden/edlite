namespace Edlite
{
    using System.Web.UI;

    /// <summary>
    /// 
    /// </summary>
    public class AdminPage : Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("this is a test");
        }
    }
}
