using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Home.Controllers
{
    #region ActionLogAttribute
    public class ActionLogAttribute : ActionFilterAttribute
    {
        private Stream responseBody;
        private MemoryStream memoryBody;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            responseBody = context.HttpContext.Response.Body;
            memoryBody = new MemoryStream();
            context.HttpContext.Response.Body = memoryBody;

            base.OnActionExecuting(context);
            IOHelper.SaveLogResult(FilterAction_enum.OnActionExecuting, context.HttpContext);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            IOHelper.SaveLogResult(FilterAction_enum.OnActionExecuted, context.HttpContext);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);
            IOHelper.SaveLogResult(FilterAction_enum.OnResultExecuting, context.HttpContext);
        }
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            memoryBody.Seek(0, SeekOrigin.Begin);
            using (StreamReader sr = new StreamReader(memoryBody))
            {
                string str = sr.ReadToEnd();
                IOHelper.SaveViewResult(str, context.HttpContext);

                byte[] bytes = Encoding.UTF8.GetBytes(str);
                responseBody.WriteAsync(bytes, 0, bytes.Length);
            }
            base.OnResultExecuted(context);
            IOHelper.SaveLogResult(FilterAction_enum.OnActionExecuted, context.HttpContext);
        }
    }
    #endregion


    public static class IOHelper
    {
        #region
        private static string root_dir_name = "Client.Mvc";
        private static DateTime actionExecutingTime;
        private static DateTime resultExecutingTime;

        public static string GetRootDir(string _root_dir_name)
        {
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string root_dir_path = base_dir.Substring(0, base_dir.IndexOf(_root_dir_name)) + _root_dir_name;
            return root_dir_path;
        }
        public static void SaveViewResult(string response, HttpContext httpContext)
        {
            string log_dir = GetRootDir(root_dir_name) + "\\log";

            var routeData = httpContext.Request.RouteValues;
            string controllerName = routeData["controller"].ToString();
            string actionName = routeData["action"].ToString();

            string file_name = controllerName + "-" + actionName + ".xml";

            Encoding encoding = Encoding.GetEncoding("UTF-8");
            string[] arr_src = new string[1];
            arr_src[0] = response;

            File.WriteAllLines(Path.Combine(log_dir, file_name), arr_src, encoding);
        }

        public static void SaveLogResult(FilterAction_enum filterAction, HttpContext httpContext)
        {
            string log_dir = GetRootDir(root_dir_name) + "\\log";
            string log_file_name = log_dir + "\\_log.txt";

            //var routeData = httpContext.Request.RequestContext.RouteData;
            //string controllerName = routeData.Values["controller"].ToString();
            //string actionName = routeData.Values["action"].ToString();

            string controllerName = "";
            string actionName = "";

            if (actionName == "Index" && filterAction == FilterAction_enum.OnActionExecuting)
            {
                string[] path_files = Directory.GetFiles(log_dir);
                string[] output_files = path_files.Select(ss => Path.GetFileName(ss)).ToArray();
                foreach (string file in output_files)
                {
                    File.Delete(log_dir + "//" + file);
                }
            }
            if (File.Exists(log_file_name) == false)
            {
                using (StreamWriter sw = File.CreateText(log_file_name))
                {
                    sw.WriteLine("");
                }
            }

            string span = null;
            TimeSpan sp;
            switch (filterAction)
            {
                case FilterAction_enum.OnActionExecuting:
                    actionExecutingTime = DateTime.Now;
                    break;
                case FilterAction_enum.OnActionExecuted:
                    sp = DateTime.Now - actionExecutingTime;
                    span = sp.TotalSeconds.ToString();
                    break;
                case FilterAction_enum.OnResultExecuting:
                    resultExecutingTime = DateTime.Now;
                    break;
                case FilterAction_enum.OnResultExecuted:
                    sp = DateTime.Now - resultExecutingTime;
                    span = sp.TotalSeconds.ToString();
                    break;
            }

            string str_action = controllerName + "." + actionName + " ---------------------------------";
            string str_filter = " - " + filterAction.ToString() + " [" + DateTime.Now + "]";
            if (span != null)
                str_filter += "   " + span;

            using (StreamWriter sw = File.AppendText(log_file_name))
            {
                if (filterAction == FilterAction_enum.OnActionExecuting)
                {
                    sw.WriteLine("");
                    sw.WriteLine(str_action);
                }

                sw.WriteLine(str_filter);
            }
        }
        #endregion
    }

    public enum FilterAction_enum
    {
        None = 0,
        OnActionExecuting = 1,
        OnActionExecuted = 2,
        OnResultExecuting = 3,
        OnResultExecuted = 4,
    }
}
