using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace ClearbytesBridge
{
    public class CSQLite
    {
        public SQLiteConnection Connection;
        public CSQLite(string db)
        {
            if (!File.Exists(db)) throw new FileNotFoundException();

            Connection = new SQLiteConnection("Data Source=" + db + ";Version=3;");
            try
            {
                Connection.Open();
            }
            finally
            {
                if (IsOpen())
                    Connection.Close();
            }
        }

        public bool IsOpen()
        {
            return (Connection.State == System.Data.ConnectionState.Open || Connection.State == System.Data.ConnectionState.Fetching || Connection.State == System.Data.ConnectionState.Executing);
        }


        public QueryResult NonQuery(string q)
        {
            SQLiteCommand cmd = null;
            try
            {
                cmd = new SQLiteCommand(q, Connection);
                Connection.Open();
                return new QueryResult(cmd.ExecuteNonQuery(), 0, null, null);
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                if (IsOpen())
                    Connection.Close();
            }
        }

        public QueryResult QuickQuery(string q)
        {
            SQLiteDataReader r = null;
            SQLiteCommand cmd = null;
            try
            {
                List<object> cols = new List<object>();
                List<object[]> rows = new List<object[]>();

                Connection.Open();
                cmd = new SQLiteCommand(q, Connection);
                r = cmd.ExecuteReader();

                int affected = r.RecordsAffected;
                int returned = 0;
                bool readcols = false;
                while (r.Read())
                {
                    if (!readcols)
                    {
                        for (int i = 0; i < r.VisibleFieldCount; i++) cols.Add(r.GetName(i));

                        readcols = true;
                    }

                    List<Object> row = new List<object>();
                    for (int i = 0; i < r.VisibleFieldCount; i++)
                        row.Add(r.GetValue(i));
                    returned++;
                    rows.Add(row.ToArray());
                }

                r.Close();
                r.Dispose();
                r = null;

                return new QueryResult(affected, returned, cols.ToArray(), rows.ToArray());
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();

                if (IsOpen())
                    Connection.Close();
                if (r != null && !r.IsClosed)
                {
                    r.Close();
                    r.Dispose();
                    r = null;
                }
            }
        }
    }

    public class QueryResult
    {
        public readonly int Affected = -1;
        public readonly int Returned = -1;

        public readonly object[] Columns;
        public readonly object[][] Rows;

        public QueryResult(int a, int r, object[] c, object[][] i)
        {
            Affected = a;
            Returned = r;
            Columns = c;
            Rows = i;
        }
    }
}
