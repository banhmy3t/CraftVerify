//dotnet add package MySql.Data
using MySql.Data.MySqlClient;
using System.Reflection.Metadata;

namespace DataAccessLibraryCraftVerify
{
    public class MySQLDAO : IReadOnlyDAO, IWriteOnlyDAO
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
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    using (MySqlCommand command = new MySqlCommand(sqlcommand, connection, transaction))
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
            MySqlDataReader read = null;
            string attributevalue = null;
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    using (MySqlCommand command = new MySqlCommand(sqlcommand, connection, transaction))
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
