namespace Edlite
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Script.Serialization;

    public class AdminHandler : IHttpHandler
    {
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
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;

            if (Request.RequestType == "POST")
            {
                var jc = new JavaScriptSerializer();
                StringBuilder sb = new StringBuilder();

                if (Request.Params.AllKeys.Contains("query"))
                {
                    var res = this.Run(Request.Params["query"]);
                    jc.Serialize(res, sb);
                }

                Response.Write(sb.ToString());
                Response.ContentType = "application/json";
            }
            else if (Request.RequestType == "GET")
            {
                Response.StatusCode = 404;
                Response.Status = "Not supported";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        object Run(string query)
        {
            IList<object> results = new List<object>();
            string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string message = null;
            string error = null;

            try
            {
                using (DbConnection conn = this.GetConnection(connString))
                {
                    conn.Open();

                    DbCommand command = this.GetCommand(query, conn);
                    var reader = command.ExecuteReader();
                    bool loopResult = true;

                    while (loopResult)
                    {
                        List<object> table = new List<object>();

                        while (reader.Read())
                        {
                            int fields = reader.FieldCount;
                            List<object> row = new List<object>();

                            for (int i = 0; i < fields; i++)
                            {
                                row.Add(reader[i]);
                            }

                            table.Add(row);
                        }

                        loopResult = reader.NextResult();

                        results.Add(table);
                    }

                    message = "ok";
                }
            }
            catch(Exception e)
            {
                error = e.Message;
            }

            return new { results = results, error=error, message=message};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        DbConnection GetConnection(string connString)
        {
            return Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings["edliteDbConnectionType"]), connString) as DbConnection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        DbCommand GetCommand(string query, DbConnection conn)
        {
            return Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings["edliteDbCommandType"]), query, conn) as DbCommand;
        }
    }
}
