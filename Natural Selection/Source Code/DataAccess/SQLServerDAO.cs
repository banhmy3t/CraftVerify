using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Reflection.Metadata;

namespace DataAccessLibraryCraftVerify
{
    public class SQLServerDAO : IReadOnlyDAO, IWriteOnlyDAO
    {
        public int InsertAttribute(string connString, string sqlcommand)
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
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                using (var transaction = await connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    using (SqlCommand command = new SqlCommand(sqlcommand, connection, transaction))
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

        public ICollection<object>? GetAttribute(string connString, string sqlcommand)
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
            SqlDataReader? read = null;
            ICollection<object>? attributevalue = new List<object>();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                using (var transaction = await connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    using (SqlCommand command = new SqlCommand(sqlcommand, connection, transaction))
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
