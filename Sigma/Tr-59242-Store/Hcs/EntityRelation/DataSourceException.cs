using System;
using System.Collections.Generic;
using System.Text;

namespace Hcs.Stores
{
    public class DataStoreException : CommonException
    {
        private static readonly string defaultMessage = "Ошибка при работе с хранилищем данных.";
        private static readonly string defaultCode = "STR_GEN_00000";

        public DataStoreException(string code)
            : base(defaultMessage, code)
        {
        }
        public DataStoreException(string code, Exception innerException)
            : base(defaultMessage, code, innerException)
        {
        }
        public DataStoreException()
            : base(defaultMessage, defaultCode)
        {
        }
        public DataStoreException(Exception innerException)
            : base(defaultMessage, defaultCode, innerException)
        {
        }

        //$$$
        //public DataSourceException(System.Data.SqlClient.SqlException sqlException)
        //    : base(defaultMessage, String.Format("STR_SQL_{0:00000}", sqlException.Number), sqlException)
        //{
        //}
    }
}
