using Capa_de_Presentacion;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace InOutSoft
{
    public partial class HomeForm : Form
    {
        public HomeForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Size = new Size(332, 610);
            ListForm F = new ListForm();
            F.lblLogo.Text = lblLogo.Text;
            F.lblDir.Text = lblDir.Text;
            F.Show();
        }

        private void btnList_Click(object sender, System.EventArgs e)
        {
            this.Size = new Size(332, 610);
            ListForm F = new ListForm();
            F.lblLogo.Text = lblLogo.Text;
            F.lblDir.Text = lblDir.Text;
            F.Show();
        }

        private void btnIn_Click(object sender, System.EventArgs e)
        {
            this.Size = new Size(1130, 670);
            lblTypeTitle.Text = "Generar Entrada";
            lblType.Text = "Entrada";
            button1.Text = "Guardar";
            button1.BackColor = Color.RoyalBlue;
            button1.ForeColor = Color.White;
        }

        private void btnOut_Click(object sender, System.EventArgs e)
        {
            this.Size = new Size(1130, 670);
            lblTypeTitle.Text = "Generar Salida";
            lblType.Text = "Salida";
            button1.Text = "Registrar";
            button1.BackColor = Color.SpringGreen;
            button1.ForeColor = Color.Black;
        }

        private void HomeForm_Load(object sender, System.EventArgs e)
        {
            //llenar();
            this.Size = new Size(332, 610);
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            var fecha = DateTime.Today.ToString("dd/MM/yyyy");
            var hora = DateTime.Now.ToString("hh:mm:ss tt");

            lblFecha.Text = fecha;
            lblHora.Text = hora;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.Desconectar();

            if (Program.Id > 0 && Program.descripcion.ToLower() == "salida")
            {
                tickEstiloP();
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("Registrartaller", Program.conection()))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(Program.Id);
                    cmd.Parameters.Add("@cliente", SqlDbType.NVarChar).Value = (txtCliente.Text + "." + txtTelefono.Text).ToLower();
                    cmd.Parameters.Add("@averia", SqlDbType.NVarChar).Value = txtAveria.Text.ToLower();
                    cmd.Parameters.Add("@tipoDeTrabajo", SqlDbType.VarChar).Value = lblType.Text;
                    cmd.Parameters.Add("@Datos", SqlDbType.NVarChar).Value = (txtModelo.Text + "." + txtImei.Text).ToUpper();
                    cmd.Parameters.Add("@Marca", SqlDbType.NVarChar).Value = txtMarca.Text;
                    cmd.Parameters.Add("@fecha", SqlDbType.DateTime).Value = Convert.ToDateTime(dtpFecha.Text);
                    cmd.Parameters.Add("@precio", SqlDbType.Decimal).Value = Convert.ToDecimal(txtprecio.Text);
                    cmd.Parameters.Add("@nota", SqlDbType.NVarChar).Value = txtnota.Text;

                    Program.Conectar();
                    cmd.ExecuteNonQuery();
                    Program.Desconectar();

                    if (lblType.Text.ToLower() == "salida")
                    {
                        using (SqlCommand cmd2 = new SqlCommand("pagos_re", Program.conection()))
                        {
                            cmd2.CommandType = CommandType.StoredProcedure;

                            //Tabla de pago
                            cmd2.Parameters.Add("@monto", SqlDbType.Decimal).Value = Convert.ToDecimal(txtprecio.Text);
                            cmd2.Parameters.Add("@fecha", SqlDbType.DateTime).Value = Convert.ToDateTime(dtpFecha.Text);

                            Program.Conectar(); ;
                            cmd2.ExecuteNonQuery();
                            Program.Desconectar();

                            tickEstiloP();
                            MessageBox.Show("Salida Registrada y Pago Confirmado");
                        }
                    }
                    else
                    {
                        tickEstiloP();
                        MessageBox.Show("Guardado en el Taller");
                    }
                }

                clean();
            }
        }

        public void tickEstiloP()
        {
            CrearTiket ticket = new CrearTiket();

            //cabecera del ticket.
            ticket.TextoCentro(lblLogo.Text);
            ticket.TextoIzquierda(lblDir.Text);
            ticket.TextoIzquierda("TELEFONOS:" + lblTel1.Text + "/" + lblTel2.Text);
            ticket.TextoIzquierda("RNC: " + lblrnc.Text);
            ticket.lineasGuio();

            //SUB CABECERA.
            ticket.TextoIzquierda("FECHA: " + DateTime.Now.ToShortDateString());
            ticket.TextoIzquierda("HORA: " + DateTime.Now.ToShortTimeString());

            //ARTICULOS A VENDER.
            ticket.lineasGuio();
            ticket.TextoIzquierda("TIPO DE TRABAJO: " + lblType.Text.ToUpper());
            ticket.TextoIzquierda("CLIENTE: " + txtCliente.Text);
            ticket.TextoIzquierda("NUMERO: " + txtTelefono.Text);
            ticket.TextoIzquierda("MARCA: " + txtMarca.Text);
            ticket.TextoIzquierda("IMEI: " + txtImei.Text);
            ticket.TextoIzquierda("MODELO: " + txtModelo.Text);
            ticket.TextoIzquierda("NOTA: " + txtnota.Text);
            ticket.TextoIzquierda("");

            //resumen de la venta
            ticket.AgregarTotales("COSTO TOTAL DEL SERVICIO : ", decimal.Parse(txtprecio.Text));

            //TEXTO FINAL DEL TICKET
            ticket.TextoIzquierda("EXTRA");
            ticket.TextoIzquierda("-FAVOR REVISE MUY BIEN EL TRABAJO AL RECIBIRLO");
            ticket.TextoIzquierda("-SOLO GARANTIZAMOS EL TRABAJO REALIZADO POR NOSOTROS");
            ticket.TextoCentro("!GRACIAS POR VISITARNOS");

            ticket.TextoIzquierda("");
            ticket.TextoIzquierda("");
            ticket.TextoIzquierda("");
            ticket.TextoIzquierda("");
            ticket.TextoIzquierda("");
            ticket.TextoIzquierda("");
            ticket.TextoIzquierda("");
            ticket.TextoIzquierda("");
            ticket.TextoIzquierda("");
            ticket.TextoIzquierda("");
            ticket.CortaTicket();//CORTAR TICKET
            ticket.ImprimirTicket(Program.ImpresonaPeq);//NOMBRE DE LA IMPRESORA
        }

        public void clean()
        {
            txtTelefono.Clear();
            txtModelo.Clear();
            txtprecio.Clear();
            txtnota.Clear();
            txtMarca.Clear();
            txtImei.Clear();
            txtCliente.Clear();
            txtAveria.Clear();
        }

        public void llenar()
        {
            Program.Desconectar();
            string cadSql = "select * from NomEmp";
            SqlCommand comando = new SqlCommand(cadSql, Program.conection());
            Program.Conectar();
            SqlDataReader leer = comando.ExecuteReader();
            if (leer.Read())
            {
                lblDir.Text = leer["DirEmp"].ToString();
                lblLogo.Text = leer["NombreEmp"].ToString();
                lblTel1.Text = leer["Tel1"].ToString();
                lblTel2.Text = leer["Tel2"].ToString();
                lblrnc.Text = leer["RNC"].ToString();
                var impresoraPeq = leer["ImpresoraPeq"].ToString();
                Program.ImpresonaPeq = string.IsNullOrWhiteSpace(impresoraPeq) ? "POS80 Printer" : impresoraPeq;
            }
            Program.Desconectar();
        }
    }
}
