using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace InOutSoft
{
    public class ConnectionCx
    {
        public string connectionString;
        public SqlConnection sqlConnection;

        public void connection()
        {
            connectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
        }

        public void Connect()
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                connection();

            if (sqlConnection.State == ConnectionState.Open)
                return;

            if (sqlConnection.State == ConnectionState.Closed)
                sqlConnection.Open();
        }

        public void Disconnect()
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                connection();

            if (sqlConnection.State == ConnectionState.Closed)
                return;

            if (sqlConnection.State == ConnectionState.Open)
                sqlConnection.Close();
        }
    }
}
