using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Linq;
using System;
using Newtonsoft.Json;


namespace Tsb.WCF.Web
{
    public enum ServiceResultNewItemType_Enum
    {
        EntityObject = 0,
        Class = 1,
        Link = 2,
    }
}

namespace Tsb.WCF.Web.SystemSpace.SysTables
{
    public class ServiceResult
    {
        public ServiceResult()
        {
            Error = false;
            ErrorMessages = new List<string>();
            Messages = new List<string>();
            Items = new List<ServiceResult>();
        }

        #region static Results
        public static ServiceResult NewResultWithError(string errorMessage, string errorMessageInner = null)
        {
            ServiceResult result = new ServiceResult();
            result.ErrorMessage = errorMessage;
            result.ErrorMessageInner = errorMessageInner;
            result.Error = true;
            return result;
        }

        public static ServiceResult NewResultSuccess(string message)
        {
            ServiceResult result = new ServiceResult();
            result.Message = message;
            return result;
        }

        public static ServiceResult NewResultSuccess()
        {
            ServiceResult result = new ServiceResult();
            return result;
        }

        #endregion

        #region Count / CountErrors
        [JsonIgnore]
        public int Count
        {
            get
            {
                ServiceResult item = this;
                if (runItem != null && runItems.Count() == 1)
                    item = runItem;
                if (runItems.Count() > 1)
                    return runItems.Count();

                if (item.Items.Count() > 0)
                    return item.Items.Count();
                return 1;
            }
        }

        [JsonIgnore]
        public int CountErrors
        {
            get
            {
                ServiceResult item = this;
                if (runItem != null && runItems.Count() == 1)
                    item = runItem;
                if (runItems.Count() > 1)
                    return runItems.Where(ss => ss.Error).Count();

                if (item.Items.Count() > 0)
                    return item.Items.Where(ss => ss.Error).Count();
                else if (item.Error)
                    return 1;
                return 0;
            }
        }
        #endregion

        #region Error status
        public bool Error { get; set; }

        public int ErrorCode { get; set; }

        [JsonIgnore]
        public bool ErrorOrConfirm { get; set; }

        [JsonIgnore]
        public bool ErrorItemRun
        {
            get
            {
                if (runItem != null)
                    return runItem.Items.Where(ss => ss.Error).Count() > 0;
                return false;
            }
        }
        #endregion

        #region Message status
        [JsonIgnore]
        public bool WithoutMeassage { get; set; }
        #endregion

        #region ErrorMessage
        public string ErrorMessage { get; set; }
        [JsonIgnore]
        public List<string> ErrorMessages { get; set; }
        public string ErrorMessageInner { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        public string ErrorMessageFull
        {
            get
            {
                return ErrorMessage
                  + (!string.IsNullOrWhiteSpace(ErrorMessageInner) ? "\n************\n" + ErrorMessageInner : string.Empty)
                  + (!string.IsNullOrWhiteSpace(Message) ? "\n************\n" + Message : string.Empty);
            }
        }
        #endregion

        #region Message
        public string Message { get; set; }

        [JsonIgnore]
        public List<string> Messages { get; set; }
        #endregion

        #region GetMessage() / GetErrorMessage()
        public string GetMessage()
        {
            //return Message;
            #region
            if (result_message != null)
                return result_message;
            { }

            #region messages
            List<string> messages = new List<string>();
            List<ServiceResult> items = new List<ServiceResult>();
            if (Message != null)
                messages.Add(Message);
            if (Messages != null)
                messages.AddRange(Messages);
            if (Items != null)
            {
                items.AddRange(Items.Where(ss => ss.Message != null || ss.Messages.Any()
                    || ss.ErrorMessage != null || ss.ErrorMessages.Any()));
                if (runItem != null)
                {
                    items.AddRange(runItem.Items.Where(ss => ss.Message != null || ss.Messages.Any()
                        || ss.ErrorMessage != null || ss.ErrorMessages.Any()));
                }
            }
            #endregion

            #region result
            if (messages.Count + items.Count > 5)
            {
                #region ErrorMessages
                if (ErrorMessages == null)
                    ErrorMessages = new List<string>();
                if (ErrorMessages.Count > 0)
                    ErrorMessages.Add("\n---------Сообщения---------------");

                ErrorMessages.AddRange(messages);
                foreach (ServiceResult item in items)
                {
                    if (item.Messages.Count() == 0 && item.ErrorMessages.Count() == 0)
                    {
                        if (item.Error && item.ErrorMessage != null)
                            ErrorMessages.Add("<p class='is-red-colored' >" + item.ErrorMessage + "<p/>");
                        else if (item.Message != null)
                            ErrorMessages.Add("<p>" + item.Message + "<p/>");
                    }
                    else
                    {
                        foreach (var message in item.Messages)
                            ErrorMessages.Add("<p>" + message + "<p/>");
                        foreach (var message in item.ErrorMessages)
                            ErrorMessages.Add("<p class='is-red-colored' >" + message + "<p/>");
                    }
                }

                //Error = true;
                Error = (CountErrors > 0 && items.Count == CountErrors);
                result_message = "Есть список сообщений";
                #endregion
            }
            else
            {
                #region result_message
                foreach (string mess in messages)
                {
                    if (mess != null)
                    {
                        if (result_message == null)
                            result_message = "";
                        result_message += "<p>" + mess + "<p/>";
                    }
                }

                if (items.Count == 1 && items[0].Items != null && items[0].Items.Count == 1)
                {
                    ServiceResult item = items[0].Items[0];
                    if (item.Error)
                        result_message += "<p class='is-red-colored' >" + item.ErrorMessage + "<p/>";
                    else
                        result_message += "<p>" + item.Message + "<p/>";
                }
                else
                {
                    foreach (ServiceResult item_in in items)
                    {
                        ServiceResult item = item_in;
                        if (items.Count == 1 && item.Items != null && item.Items.Count == 1)
                            item = item.Items[0];

                        if (item.Error)
                            result_message += "<p class='is-red-colored' >" + item.ErrorMessage + "<p/>";
                        else
                            result_message += "<p>" + item.Message + "<p/>";
                        foreach (var message in item.Messages)
                            result_message += "<p>" + message + "<p/>";
                        foreach (var message in item.ErrorMessages)
                            result_message += "<p class='is-red-colored' >" + message + "<p/>";

                        if (items.Count == 1 && item.Items != null && item.Items.Count == 1)
                            break;
                    }
                }
                #endregion
            }
            #endregion

            #region CountErrors / DateDiff
            string _result_message = "";
            if (this.CountErrors > 1)
                _result_message += "- ошибки: " + this.CountErrors + " из " + this.Count;
            if (afterRun != null && afterRun.DateDiff != null)
                _result_message += "\n- время выполнения " + afterRun.DateDiff;
            if (_result_message != "" && result_message != null)
                result_message += "<p>" + _result_message + "</p>";
            #endregion

            return result_message;
            #endregion
        }
        private string result_message;

        public string GetErrorMessage()
        {
            //return Message;
            #region
            if (ErrorMessageInner != null)
                return ErrorMessageInner;
            if (ErrorMessages == null || ErrorMessages.Count == 0)
                return ErrorMessage;

            string messaage = "";
            foreach (string mess in ErrorMessages)
            {
                if (mess != null)
                    messaage += mess + "\n";
            }

            return messaage;
            #endregion
        }
        #endregion

        #region Items / AddResult() / runItem / afterRun
        public AlgotithmStatus_Enum AlgotithmStatus { get; set; }

        [JsonIgnore]
        public List<ServiceResult> Items { get; set; }
        public void AddResult(ServiceResult result)
        {
            Items.Add(result);
            //Error = (CountErrors > 0 && Count == CountErrors);
            //if (Error && Count == 1)
            //{
            //    ErrorMessage = result.ErrorMessage;
            //}
            //else
            //{
            //    ErrorMessage = null;
            //}
        }
        public void AddResults(List<ServiceResult> results) { Items.AddRange(results); }

        [JsonIgnore]
        private ServiceResult runItem
        {
            get
            {
                //_runItem = this;
                if (_runItem == null)
                    _runItem = Items.Where(ss => ss.AlgotithmStatus == AlgotithmStatus_Enum.RunItem).FirstOrDefault();
                return _runItem;
            }
        }
        private ServiceResult _runItem;

        [JsonIgnore]
        private List<ServiceResult> runItems
        {
            get
            {
                //_runItem = this;
                if (Items != null)
                    return Items.Where(ss => ss.AlgotithmStatus == AlgotithmStatus_Enum.RunItem).ToList();
                return new List<ServiceResult>();
            }
        }


        [JsonIgnore]
        private ServiceResult afterRun
        {
            get
            {
                if (_afterRun == null)
                    _afterRun = Items.Where(ss => ss.AlgotithmStatus == AlgotithmStatus_Enum.AfterRun).FirstOrDefault();
                return _afterRun;
            }
        }
        private ServiceResult _afterRun;
        #endregion

        #region Results
        #region NewValues
        public ServiceResultNewItemType_Enum NewItemType { get; set; }

        public long NewId { get; set; }
        public decimal NewDecimal { get; set; }
        public string NewItemUrl { get; set; }
        public XElement NewItemXml { get; set; }

        [JsonIgnore]
        public object NewItem { get; set; }
        [JsonIgnore]
        public object NewViewItem { get; set; }
        [JsonIgnore]
        public List<object> NewItems { get; set; }
        #endregion

        #region DownloadData
        // для API
        //[JsonIgnore]
        public long? Download_DocumentStorageId { get; set; }
        //[JsonIgnore]
        public byte[] DownloadableData { get; set; }
        public string DownloadableFileName { get; set; }
        #endregion

        public string StringValue { get; set; }
        public decimal? DecimalValue { get; set; }

        public string HtmlValue { get; set; }

        [JsonIgnore]
        public List<long> ListId { get; set; }

        public long? Id { get; set; }

        public XElement ResultXml { get; set; }

        [JsonIgnore]
        public List<Tuple<string, string, string>> UpdateDiff { get; set; }
        #endregion

        #region DateDiff
        [JsonIgnore]
        public DateTime? DateStart { get; set; }
        [JsonIgnore]
        public DateTime? DateEnd { get; set; }
        [JsonIgnore]
        public string DateDiff { get; set; }
        #endregion

        [JsonIgnore]
        public bool is_app_host { get; set; }

        #region ToString()
        public override string ToString()
        {
            string message = "";
            if (Error)
                message = "Ошибка - " + ErrorMessage;
            else
                message = "Ok - " + Message;

            if (Items.Count() > 0)
                message += " ; [" + CountErrors + " из " + Count + "]";

            return message;
        }
        #endregion

        public List<CheckResult> CheckItems { get; set; }

        public List<object> ChartItems { get; set; }

        [JsonIgnore]
        public Extensions.Task.TaskResult TaskResult { get; set; }

        public int? Step { get; set; }
        public bool ShowMask { get; set; }
        public List<object> ResultsList { get; set; }
        public string resultStr { get; set; } 
    }

    public enum AlgotithmStatus_Enum
    {
        None = 0,
        BeforeRun = 1,
        RunItem = 2,
        AfterRun = 3,
    }

    public class CheckResult
    {
        public int CheckId { get; set; }
        public string CheckMessage { get; set; }
        public bool isError { get; set; }
    }
}