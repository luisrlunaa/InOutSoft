using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InOutSoft
{
    public partial class PrintForm : Form
    {
        ConnectionCx connectionCx = new ConnectionCx();
        public PrintForm()
        {
            InitializeComponent();
            connectionCx.connection();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("¿Está Seguro que Desea Cambiar el nombre de la Impresora?", "In/OutSoft System", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    using (SqlCommand cmd = new SqlCommand("ActualizarImpresora", connectionCx.sqlConnection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@impName", SqlDbType.NVarChar).Value = textBox1.Text;

                        connectionCx.Connect();
                        cmd.ExecuteNonQuery();
                        connectionCx.Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void PrintForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = Program.ImpressionPeq;
        }
    }
}
