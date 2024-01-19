//dotnet add package MySql.Data
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace DataAccessLibraryCraftVerify
{
    public class MySQLDAO : IReadOnlyDAO, IWriteOnlyDAO
    {
        public async Task<int> InsertAttribute(string connString, string sqlcommand)
        {
            #region Validate Arguments
            if (connString == null)
            {
                throw new ArgumentNullException();
            }
            if (sqlcommand == null)
            {
                throw new ArgumentNullException();
            }
            #endregion
            int rowsaffected = 0;
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                using (var transaction = await connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    using (MySqlCommand command = new MySqlCommand(sqlcommand, connection, transaction))
                    {
                        try
                        {
                            rowsaffected += command.ExecuteNonQueryAsync();
                            await transaction.Commit();
                        }
                        catch
                        {
                            await transaction.Rollback();
                            throw;
                        }
                    }
                }

            }
            return rowsaffected;
        }

        public async Task<ICollection<object>?> GetAttribute(string connString, string sqlcommand)
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
            MySqlDataReader? read = null;
            ICollection<object>? attributevalue = null;
            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                connection.Open();
                using (var transaction = await connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    using (MySqlCommand command = new MySqlCommand(sqlcommand, connection, transaction))
                    {
                        using (read = await command.ExecuteReaderAsync())
                        {
                            while (await read.Read())
                            {
                                var values = new object[read.FieldCount];
                                read.GetValues(values);
                                attributevalue.Add(values);
                            }
                        }
                    }
                }
            }
            return attributevalue;
        }

    }
}
