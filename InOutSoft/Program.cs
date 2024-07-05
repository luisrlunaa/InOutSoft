using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InOutSoft
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HomeForm());
        }

        public static int Id;
        public static string descripcion;
        public static string ImpresonaPeq;
        public static string connectionString;

        public static SqlConnection conection()
        {
            connectionString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            return connection;
        }

        public static void Conectar()
        {
            if (conection().State == ConnectionState.Closed)
                conection().Open();
        }

        public static void Desconectar()
        {
            if (conection().State == ConnectionState.Open)
                conection().Close();
        }
    }
}
