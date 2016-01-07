using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace US.OpenServer.Data
{
    public abstract class DBBase
    {
        protected string name;

        public const int DEFAULT_CONNECTION_TIMEOUT = 30;//30 seconds
        public const int DEFAULT_COMMAND_TIMEOUT = 300;//5 minutes        
        private const int MAX_DISPLAY_ROWS = 10;
        
        public abstract void Open();
        public abstract void Close();        
        public abstract void BeginTransaction();
        public abstract void CommitTransaction();
        public abstract void Rollback();

        public abstract void Execute(string sqlcmd);
        public abstract DataTable ExecuteQuery(string sqlcmd);
        public abstract int ExecuteQueryIdentity(string sqlcmd);
        public abstract DbDataReader ExecuteReader(string sqlcmd);

        public abstract string EscapeString(string val);

        public virtual string ByteArrayAsSqlString(byte[] data, int maxLength)
        {
            if (data == null || data.Length == 0)
                return "null";
            
            StringBuilder hexString = new StringBuilder(2 + 2 * data.Length);
            hexString.Append("0x");
            for (int counter = 0; counter < data.Length; counter++)
            {
                //do not overflow the database column
                if (counter >= maxLength)
                    break;

                hexString.Append(data[counter].ToString("X2"));
            }
            return hexString.ToString();
        }

        public static byte GetByte(DbDataReader dr, int col)
        {
            if (dr.IsDBNull(col))
                return 0;

            if (typeof(Int16) == dr.GetFieldType(col))//oracle
                return Convert.ToByte(dr.GetInt16(col));
            
            return dr.GetByte(col);
        }

        public static byte[] GetByteArray(DbDataReader sdr, int col)
        {
            if (!sdr.IsDBNull(col))
            {
                long datalen = sdr.GetBytes(col, 0, null, 0, 0);
                byte[] data = new byte[datalen];
                sdr.GetBytes(col, 0, data, 0, data.Length);
                return data;
            }
            else
                return null;
        }

        public static byte GetByte(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj == null || obj is DBNull)
                return 0x00;
            
            if (obj is int || obj is short)
                return Convert.ToByte(obj);
                
            return (byte)obj;
        }

        public static byte[] GetByteArray(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj == null || obj is DBNull)
                return null;
            else
                return (byte[])obj;
        }

        public static bool GetBool(DbDataReader sdr, int col)
        {
            if (!sdr.IsDBNull(col))
                return sdr.GetBoolean(col);
            else
                return false;
        }

        public static bool GetBool(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj == null || obj is DBNull)
                return false;
            else if (obj is ulong)//mysql 56
                return (ulong)obj == 0 ? false : true;
            else if (obj is short)//oracle 
                return (short)obj == 0 ? false : true;
            else
                return (bool)obj;
        }

        public static bool GetBool(DataRow dr, string col)
        {
            object obj = dr[col];
            if (obj == null || obj is DBNull)
                return false;
            else
                return (bool)obj;
        }

        public static DateTime GetDateTime(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj is DateTime)
                return (DateTime)obj;

            return new DateTime(0);
        }

        public static DateTime GetDateTime(DataRow dr, string col)
        {
            object obj = dr[col];
            if (obj is DateTime)
                return (DateTime)obj;

            return new DateTime(0);
        }

        public static DateTime? GetNullableDateTime(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj is DBNull)
                return null;

            return GetDateTime(dr, col);
        }

        public static TimeSpan GetTimeSpan(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj is TimeSpan)
                return (TimeSpan)obj;

            return new TimeSpan(0);
        }

        public static TimeSpan? GetNullableTimeSpan(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj is DBNull)
                return null;

            return GetTimeSpan(dr, col);
        }

        public static short GetShort(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj == null || obj is DBNull)
                return 0;
            else
                return Convert.ToInt16(obj);
        }

        public static ushort GetUShort(DbDataReader sdr, int col)
        {
            if (sdr.IsDBNull(col))
                return 0;

            if (typeof(long) == sdr.GetFieldType(col))//oracle
                return Convert.ToUInt16(sdr.GetInt64(col));

            return (ushort)sdr.GetInt32(col);            
        }

        public static ushort GetUShort(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj == null || obj is DBNull)
                return 0;
            else
                return Convert.ToUInt16(obj);
        }

        public static int GetInt(DbDataReader sdr, int col)
        {
            if (sdr.IsDBNull(col))
                return 0;
            
            if (typeof(Int64) == sdr.GetFieldType(col))//oracle
                return Convert.ToInt32(sdr.GetInt64(col));
                
            return sdr.GetInt32(col);
        }

        public static int GetInt(DataRow dr, int col)
        {
            return GetInt(dr[col]);
        }

        public static int? GetNullableInt(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj is DBNull)
                return null;

            return GetInt(dr, col);
        }

        public static int GetInt(DataRow dr, string col)
        {
            return GetInt(dr[col]);
        }

        private static int GetInt(object obj)
        {
            if (obj == null || obj is DBNull)
                return 0;
            
            if (typeof(Int64) == obj.GetType())//oracle, MySQL count(*)
                return Convert.ToInt32(obj);

            if (typeof(decimal) == obj.GetType())//oracle
                return Convert.ToInt32(obj);
                
            return (int)obj;
        }

        public static uint GetUInt(DbDataReader sdr, int col)
        {
            if (sdr.IsDBNull(col))
                return 0;

            if (typeof(decimal) == sdr.GetFieldType(col))//1st oracle release of Log Manager
                return Convert.ToUInt32(sdr.GetDecimal(col));

            return (uint)sdr.GetInt64(col);
        }

        public static uint GetUInt(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj == null || obj is DBNull)
                return 0;
            else
                return Convert.ToUInt32(obj);
        }

        public static UInt64 GetUInt64(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj == null || obj is DBNull)
                return 0;
            else
                return Convert.ToUInt64(obj);
        }

        public static long GetLong(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj == null || obj is DBNull)
                return 0;
            else
                return Convert.ToInt64(obj);
        }

        public static string GetString(DbDataReader sdr, int col)
        {
            if (!sdr.IsDBNull(col))
                return sdr.GetString(col);
            else
                return string.Empty;
        }

        public static string GetString(DataRow dr, int col)
        {
            object obj = dr[col];
            if (obj == null || obj is DBNull)
                return string.Empty;
            else if (obj is byte[])
                return Encoding.ASCII.GetString((byte[])obj);//MySQL Log Repository Notes/Get Top 10000
            else
                return (string)obj;
        }

        public static string GetString(DataRow dr, string col)
        {
            object obj = dr[col];
            if (obj == null || obj is DBNull)
                return string.Empty;
            else
                return (string)obj;
        }

        public static string[] GetStringArray(DataRow dr, int col)
        {
            return ReadStrings(GetByteArray(dr, col));
        }

        public static string[] GetStringArray(DbDataReader dr, int col)
        {
            return ReadStrings(GetByteArray(dr, col));
        }

        private static string[] ReadStrings(byte[] stringsAsBytes)
        {
            if (stringsAsBytes == null)
                return new string[0];

            List<string> retVal = new List<string>();

            StringBuilder strbldrU = new StringBuilder();
            for (int i = 0; i + 1 < stringsAsBytes.Length; i += 2)
            {
                byte[] bt = { stringsAsBytes[i], stringsAsBytes[i + 1] };
                string str = UnicodeEncoding.Unicode.GetString(bt);
                if (str == "\0")
                {
                    retVal.Add(strbldrU.ToString());
                    strbldrU = new StringBuilder();
                }
                else
                    strbldrU.Append(str);
            }
            return retVal.ToArray();
        }

        public string EscapeString(string sqlCmd, int maxLength)
        {
            string retVal = EscapeString(sqlCmd);
            if (!string.IsNullOrEmpty(retVal))
            {
                //truncate message
                if (retVal.Length > maxLength)
                    retVal = retVal.Substring(0, maxLength);
            }
            retVal = (retVal != null && retVal.Length > 0) ? "'" + retVal + "'" : "null";
            return retVal;
        }

        protected string Trim(string val)
        {
            if (val.EndsWith("'"))
            {
                int i = val.LastIndexOf('\'');
                while (i == val.Length - 1 && i > 0)
                {
                    val = val.Substring(0, val.Length - 1);
                    i = val.LastIndexOf('\'');
                }
            }
            return val;
        }
    }
}
