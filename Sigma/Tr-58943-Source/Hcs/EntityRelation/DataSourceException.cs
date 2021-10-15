using System;
using System.Collections.Generic;
using System.Text;

namespace Hcs.DataSource
{
    public class DataSourceException : CommonException
    {
        private static readonly string defaultMessage = "Ошибка при работе с источником данных.";
        private static readonly string defaultCode = "STR_GEN_00000";

        public DataSourceException(string code)
            : base(defaultMessage, code)
        {
        }
        public DataSourceException(string code, Exception innerException)
            : base(defaultMessage, code, innerException)
        {
        }
        public DataSourceException()
            : base(defaultMessage, defaultCode)
        {
        }
        public DataSourceException(Exception innerException)
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
