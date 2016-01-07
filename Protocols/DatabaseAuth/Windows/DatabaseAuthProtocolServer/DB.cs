using System;
using System.Collections.Generic;
using System.Data;
using US.OpenServer.Data.SqlServer;

namespace US.OpenServer.Protocols.DatabaseAuth
{
    public class DB : SqlServerBase
    {
        public DB(string name) : base (name)
        {
        }

        public int Authenticate(string userName, string password)
        {
            string cmd = string.Format(Properties.Resources.SQL_AUTHENTICATE, userName);
            DataTable dt = ExecuteQuery(cmd);
            if (dt.Rows.Count < 1)
                throw new Exception("Username not found.");

            DataRow dr = dt.Rows[0];
            string actualPassword = GetString(dr, "password");
            if (actualPassword != password)
                throw new Exception("Access Denied");

            return GetInt(dr, "id");
        }

        public bool IsInRole(int accountId, string role)
        {
            List<string> retVal = new List<string>();

            string cmd = string.Format(Properties.Resources.SQL_IS_IN_ROLE, accountId, role);
            return ExecuteQueryIdentity(cmd) == 0 ?  false : true;
        }
    }
}
