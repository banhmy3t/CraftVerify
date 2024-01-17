using Microsoft.Data.SqlClient;

namespace DataAccessLibraryCraftVerify
{
    public class SQLServerDAO : IReadOnlyDAO, IWriteOnlyDAO
    {
        public int InsertAttribute(string connString, string sqlcommand)
        {
            if (connString == null)
            {
                throw new ArgumentNullException();
            }
            if (sqlcommand == null)
            {
                throw new ArgumentNullException();
            }

            int rowsaffected = 0;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    using (SqlCommand command = new SqlCommand(sqlcommand, connection, transaction))
                    {
                        try
                        {
                            rowsaffected += command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

            }
            return rowsaffected;
        }

        public string GetAttribute(string connString, string sqlcommand)
        {
            #region Validate arguments
            if (connString == null)
            {
                throw new ArgumentNullException();
            }
            if (sqlcommand == null)
            {
                throw new ArgumentNullException();
            }
            #endregion
            SqlDataReader read = null;
            string attributevalue = null;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    using (SqlCommand command = new SqlCommand(sqlcommand, connection, transaction))
                    {
                        using (read = command.ExecuteReader())
                        {
                            if (read.Read())
                            {
                                attributevalue = read["YourAttribute"].ToString();
                            }
                        }
                    }
                }
            }
            return attributevalue;
        }
    }
}
