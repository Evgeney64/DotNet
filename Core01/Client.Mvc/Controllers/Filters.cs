using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Web.UI;
using Microsoft.AspNetCore.Http.Features;

namespace Home.Controllers
{
    #region ActionLogAttribute


    public class ActionLogAttribute : ActionFilterAttribute
    {
        private Stream stream;
        private MemoryStream responseBody;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            this.responseBody = new MemoryStream();
            // hijack the real stream with our own memory stream 
            filterContext.HttpContext.Response.Body = responseBody;

            //stream = filterContext.HttpContext.Response.Body;
            //stream = new MemoryStream();
            //filterContext.HttpContext.Response.Body = stream;

            //return;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            //filterContext.HttpContext.Response.Body.CopyToAsync(responseBody);
            responseBody.CopyToAsync(filterContext.HttpContext.Response.Body);
            return;

            HttpContext context = filterContext.HttpContext;

            using (StreamReader sr = new StreamReader(responseBody))
            {
                var actionResult = sr.ReadToEnd();
                Console.WriteLine(actionResult);
                // create new stream and assign it to body 
                // context.HttpContext.Response.Body = ;
            }

            // no ERROR on the next line!

            //string str = (string)context.Items["body"];
            //context.Features.Set<IHttpSendFileFeature>(bodyWrapperStream);
            { }
            StreamReader responsereader = new StreamReader(context.Response.Body);  //empty stream? why?
            responsereader.BaseStream.Position = 0;
            string response = responsereader.ReadToEnd();
            context.Response.Body.CopyToAsync(stream);

            Stream originalBody = context.Response.Body;
            try
            {
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    //await next(context);

                    memStream.Position = 0;
                    string responseBody = new StreamReader(memStream).ReadToEnd();

                    memStream.Position = 0;
                    memStream.CopyToAsync(originalBody);
                }

            }
            finally
            {
                context.Response.Body = originalBody;
            }
            return;
            //string str = filterContext.HttpContext.Request.BodyReader.ToString();
            { }
            //StreamReader responsereader = new StreamReader(filterContext.HttpContext.Response.Body);  //empty stream? why?
            //responsereader.BaseStream.Position = 0;
            //string response = responsereader.ReadToEnd();
            //IOHelper.SaveViewResult(response, filterContext.HttpContext);

            //ContentResult contres = new ContentResult();
            //contres.Content = response;
            //filterContext.Result = contres;

        }
    }

    public class ActionLogAttribute0 : ActionFilterAttribute//, IPageFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            IOHelper.SaveLogResult(FilterAction_enum.OnActionExecuting, filterContext.HttpContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            IOHelper.SaveLogResult(FilterAction_enum.OnActionExecuted, filterContext.HttpContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            IOHelper.SaveLogResult(FilterAction_enum.OnResultExecuting, filterContext.HttpContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var content = filterContext.HttpContext.Response;
            using (var responseWriter = new StreamWriter(content.Body, Encoding.UTF8))
            {
                responseWriter.Write("This is some text to write!");
            }
            //Stream body = filterContext.HttpContext.Response.Body;
            //Span<byte> buffer = new Span<byte>();
            //filterContext.HttpContext.Response.Body.Read(buffer);
            //ViewResult viewResult = filterContext.Result as ViewResult;

            base.OnResultExecuted(filterContext);
            IOHelper.SaveLogResult(FilterAction_enum.OnResultExecuted, filterContext.HttpContext);
        }

        //public void OnPageHandlerExecuting(PageHandlerExecutingContext context) { }
        //public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }
        //public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
    }

    public class ActionLogAttribute1 : ReadOnlyActionFilterAttribute
    {
        protected override void OnClose(Stream stream, HttpContext httpContext)
        {
            #region
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);

            MemoryStream memoryStream = new MemoryStream(buffer);
            StreamReader reader = new StreamReader(memoryStream);
            string text = reader.ReadToEnd();

            IOHelper.SaveViewResult(text, httpContext);
            #endregion
        }
    }

    public class ActionLogAttribute3 : ActionFilterAttribute
    {
        #region Define
        private HtmlTextWriter tw;
        private StringWriter sw;
        private StringBuilder sb;
        //private HttpWriter output;
        #endregion

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //return;
            sb = new StringBuilder();
            sw = new StringWriter(sb);
            tw = new HtmlTextWriter(sw);
            //output = (HttpWriter)filterContext.HttpContext.Response.Body;
            //filterContext.RequestContext.HttpContext.Response.Output = tw;

            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            #region
            string response = sb.ToString();
            //output.Write(response);

            base.OnResultExecuted(filterContext);
            return;

            //IOHelper.SaveResult(text, httpContext);

            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string path_name = base_dir + "\\log";

            string controllerName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string file_name = controllerName + "-" + actionName + ".xml";

            if (actionName == "Index")
            {
                string[] path_files = Directory.GetFiles(path_name);
                string[] output_files = path_files.Select(ss => Path.GetFileName(ss)).ToArray();
                foreach (string file in output_files)
                    File.Delete(path_name + "//" + file);
            }

            Encoding encoding = Encoding.GetEncoding("UTF-8");
            string[] arr_src = new string[1];
            arr_src[0] = response;

            File.WriteAllLines(Path.Combine(path_name, file_name), arr_src, encoding);

            //response processing
            //output.Write(response);
            #endregion
        }
    }

    public abstract class ReadOnlyActionFilterAttribute : ActionFilterAttribute
    {
        private delegate void ReadOnlyOnClose(Stream stream, HttpContext httpContext);

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Body = new OnCloseFilter(
                filterContext.HttpContext.Response.Body,
                this.OnClose,
                filterContext.HttpContext
                );
            base.OnActionExecuting(filterContext);
            IOHelper.SaveLogResult(FilterAction_enum.OnActionExecuting, filterContext.HttpContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            IOHelper.SaveLogResult(FilterAction_enum.OnActionExecuted, filterContext.HttpContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            IOHelper.SaveLogResult(FilterAction_enum.OnResultExecuting, filterContext.HttpContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            IOHelper.SaveLogResult(FilterAction_enum.OnResultExecuted, filterContext.HttpContext);
        }

        protected abstract void OnClose(Stream stream, HttpContext httpContext);

        private class OnCloseFilter : MemoryStream
        {
            private readonly Stream stream;

            private readonly HttpContext httpContext;

            private readonly ReadOnlyOnClose onClose;

            public OnCloseFilter(Stream _stream, ReadOnlyOnClose _onClose, HttpContext _httpContext)
            {
                this.stream = _stream;
                this.onClose = _onClose;
                this.httpContext = _httpContext;
            }

            public override void Close()
            {
                this.Position = 0;
                this.onClose(this, httpContext);
                this.Position = 0;
                this.CopyTo(this.stream);
                base.Close();
            }
        }
    }
    #endregion

    public static class IOHelper
    {
        #region
        private static DateTime actionExecutingTime;
        private static DateTime resultExecutingTime;

        public static void SaveViewResult(string response, HttpContext httpContext)
        {
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string path_name = base_dir + "\\log";
            path_name=@"c:\Disc_D\Prog\DotNet\Core01\Client.Mvc\Log";

            var routeData = httpContext.Request.RouteValues;
            string controllerName = routeData["controller"].ToString();
            string actionName = routeData["action"].ToString();

            string file_name = controllerName + "-" + actionName + ".xml";

            Encoding encoding = Encoding.GetEncoding("UTF-8");
            string[] arr_src = new string[1];
            arr_src[0] = response;

            File.WriteAllLines(Path.Combine(path_name, file_name), arr_src, encoding);
        }

        public static void SaveLogResult(FilterAction_enum filterAction, HttpContext httpContext)
        {
            return;
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string path_name = base_dir + "\\log";
            string log_file_name = path_name + "\\_log.txt";

            //var routeData = httpContext.Request.RequestContext.RouteData;
            //string controllerName = routeData.Values["controller"].ToString();
            //string actionName = routeData.Values["action"].ToString();

            string controllerName = "";
            string actionName = "";

            if (actionName == "Index" && filterAction == FilterAction_enum.OnActionExecuting)
            {
                string[] path_files = Directory.GetFiles(path_name);
                string[] output_files = path_files.Select(ss => Path.GetFileName(ss)).ToArray();
                foreach (string file in output_files)
                {
                    File.Delete(path_name + "//" + file);
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
