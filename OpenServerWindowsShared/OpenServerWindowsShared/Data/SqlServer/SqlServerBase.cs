using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;

namespace US.OpenServer.Data.SqlServer
{
    public class SqlServerBase : DBBase, IDisposable
    {
        public const int DUPLICATE_KEY_VIOLATION = 2627;
        public const int STRING_OR_BINARY_OVERFLOW = 8152;

        public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
        public const string DATE_FORMAT = "yyyy-MM-dd";
        
        public const int MAX_STRING_LENGTH = 8000;
        public const int MAX_UNICODE_STRING_LENGTH = 4000;
        
        public const string NULL_VALUE = "null";

        #region Variables
        private SqlConnection c;
        private SqlTransaction t;        
        #endregion

        #region Constructor
        public SqlServerBase(string name = null) 
        {
            if (name == null)
                name = "DB";

            this.name = name;
        }

        public void Dispose()
        {
            Close();
        }
        #endregion

        #region Open/Close
        public override void Open()
        {
            ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[name];
            if (css == null)
                throw new Exception(string.Format("Missing database connection string.  Name: {0}", name));

            c = new SqlConnection(css.ConnectionString);
            c.Open();
        }      

        public void CheckConnection()
        {
            ExecuteNonQuery("select count(*) FROM dbo.sysobjects");            
        }
        public override void Close()
        {
            Rollback();
            if (c != null)
            {
                c.Close();
                c = null;
            }
        }
        #endregion

        #region Transactions
        public bool IsOpen { get { return c != null; } }

        public bool IsTransacted { get { return t != null; } }

        public override void BeginTransaction()
        {
            if (c == null)
                Open();

            t = c.BeginTransaction();
        }

        public override void CommitTransaction()
        {
            t.Commit();
            t = null;
        }

        public override void Rollback()
        {
            if (t != null)
            {
                try
                {
                    t.Rollback();
                }
                catch (Exception)
                {
                }
                t = null;
            }
        }
        #endregion

        #region Execute
        public override void Execute(string sqlcmd)
        {
            bool bCloseConnection = false;
            try
            {
                if (c == null)
                {
                    Open();
                    bCloseConnection = true;
                }

                SqlCommand cmd = new SqlCommand(sqlcmd, c);
                //cmd.CommandTimeout = commandTimeout;
                if (t != null)
                    cmd.Transaction = t;

                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (bCloseConnection)
                    Close();
            }            
        }

        public int ExecuteNonQuery(string sqlcmd)
        {
            bool bCloseConnection = false;
            try
            {
                if (c == null)
                {
                    Open();
                    bCloseConnection = true;
                }

                SqlCommand cmd = new SqlCommand(sqlcmd, c);
                //cmd.CommandTimeout = commandTimeout;
                if (t != null)
                    cmd.Transaction = t;

                return cmd.ExecuteNonQuery();
            }
            finally
            {
                if (bCloseConnection)
                    Close();
            }
        }

        public override DataTable ExecuteQuery(string sqlcmd)
        {
            bool bCloseConnection = false;
            try
            {
                if (c == null)
                {
                    Open();
                    bCloseConnection = true;
                }

                SqlCommand cmd = new SqlCommand(sqlcmd, c);
                //cmd.CommandTimeout = commandTimeout;
                if (t != null)
                    cmd.Transaction = t;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("tmp");
                adapter.Fill(dt);

                return dt;
            }
            finally
            {
                if (bCloseConnection)
                    Close();
            }
        }

        public override int ExecuteQueryIdentity(string sqlcmd)
        {
            int retVal = -1;
            DataTable dt = ExecuteQuery(sqlcmd);
            if (dt.Rows.Count > 0)
                retVal = Convert.ToInt32(dt.Rows[0][0]);

            return retVal;
        }

        public override DbDataReader ExecuteReader(string sqlcmd)
        {
            SqlCommand cmd = new SqlCommand(sqlcmd, c);
            //cmd.CommandTimeout = commandTimeout;
            if (t != null)
                cmd.Transaction = t;
            return cmd.ExecuteReader();
        }
        #endregion

        #region DateTime
        public static string FormatDate(DateTime val)
        {
            return val.ToString(SqlServerBase.DATE_FORMAT, CultureInfo.InvariantCulture);
        }

        public static string Format(DateTime val)
        {
            return val.ToString(SqlServerBase.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        }

        public string Format(DateTime? val)
        {
            return val == null || !val.HasValue ? "null" : "'" + Format(val.Value) + "'";
        }
        #endregion

        #region TimeSpan
        public static string Format(TimeSpan val)
        {
            return val.ToString();
        }

        public string Format(TimeSpan? val)
        {
            return val == null || !val.HasValue ? "null" : "'" + Format(val.Value) + "'";
        }
        #endregion

        #region Strings
        public override string EscapeString(string val)
        {
            if (val == null || val.Length <= 0)
                return val;
            else
                return val.Replace("'", "''");
        }

        public string TruncateString(string val, int maxSingleByteCharacters)
        {
            if (val == null || val.Length <= 0)
                return val;

            int maxCharacters = Math.Min(maxSingleByteCharacters, MAX_UNICODE_STRING_LENGTH);

            return (val.Length > maxCharacters) ?
                Trim(val.Substring(0, maxCharacters)) : val;
        }

        public string TruncateAsciiString(string val, int maxSingleByteCharacters)
        {
            if (val == null || val.Length <= 0)
                return val;

            int maxCharacters = Math.Min(maxSingleByteCharacters, MAX_STRING_LENGTH);

            return (val.Length > maxCharacters) ?
                Trim(val.Substring(0, maxCharacters)) : val;
        }

        public string TruncateUnicodeString(string val, int maxCharacters)
        {
            if (val == null || val.Length <= 0)
                return val;

            return (val.Length > maxCharacters) ?
                Trim(val.Substring(0, maxCharacters)) : val;
        }

        public string ParameterizeString(string val)
        {
            return (string.IsNullOrEmpty(val)) ?
                    "null" :
                    string.Format("N'{0}'", val);
        }

        public string ParameterizeAsciiString(string val)
        {
            return (string.IsNullOrEmpty(val)) ?
                    "null" :
                    string.Format("'{0}'", val);
        }

        public string FormatString(string val, int maxSingleByteCharacters)
        {
            val = EscapeString(val);
            val = TruncateString(val, maxSingleByteCharacters);
            return ParameterizeString(val);
        }

        public string FormatAsciiString(string val, int maxSingleByteCharacters)
        {
            val = EscapeString(val);
            val = TruncateAsciiString(val, maxSingleByteCharacters);
            return ParameterizeAsciiString(val);
        }

        public string FormatUnicodeString(string val, int maxCharacters)
        {
            val = EscapeString(val);
            val = TruncateUnicodeString(val, maxCharacters);
            return ParameterizeString(val);
        }
        #endregion
    }
}
