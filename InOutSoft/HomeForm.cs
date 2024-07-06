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
        ConnectionCx connectionCx = new ConnectionCx();
        public static HomeForm Instance;
        public int idreparacion;
        public string descripcion;
        public string cliente;
        public string averia;
        public string tipoDeTrabajo;
        public string datos;
        public string marca;
        public string fecha;
        public string precio;
        public string nota;

        public HomeForm()
        {
            InitializeComponent();
            connectionCx.connection();
            Instance = this;
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
            llenar();
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
            try
            {
                if (string.IsNullOrWhiteSpace(txtCliente.Text)) { MessageBox.Show("Es importante y obligatorio introducir el nombre del cliente"); return; }
                if (string.IsNullOrWhiteSpace(txtTelefono.Text)) { MessageBox.Show("Es importante y obligatorio introducir el numero de telefono del cliente"); return; }
                if (string.IsNullOrWhiteSpace(txtMarca.Text)) { MessageBox.Show("Es importante y obligatorio introducir la marca del equipo"); return; }
                if (string.IsNullOrWhiteSpace(txtImei.Text)) { MessageBox.Show("Es importante y obligatorio introducir el iemi del equipo para evitar reclamos de confusion"); return; }
                if (string.IsNullOrWhiteSpace(txtModelo.Text)) { MessageBox.Show("Es importante y obligatorio introducir el modelo del equipo"); return; }
                if (string.IsNullOrWhiteSpace(txtAveria.Text)) { MessageBox.Show("Es importante y obligatorio introducir el tipo de problema con que llego el equipo a la tienda"); return; }
                if (string.IsNullOrWhiteSpace(txtprecio.Text)) { MessageBox.Show("Es importante y obligatorio introducir el precio de la reparacion"); return; }

                connectionCx.Disconnect();
                if (idreparacion > 0 && descripcion.ToLower() == "salida")
                {
                    tickEstiloP();
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand("Registrartaller", connectionCx.sqlConnection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = idreparacion;
                        cmd.Parameters.Add("@cliente", SqlDbType.NVarChar).Value = (txtCliente.Text + '.' + txtTelefono.Text).ToLower();
                        cmd.Parameters.Add("@averia", SqlDbType.NVarChar).Value = txtAveria.Text.ToLower();
                        cmd.Parameters.Add("@tipoDeTrabajo", SqlDbType.VarChar).Value = lblType.Text.ToUpper();
                        cmd.Parameters.Add("@Datos", SqlDbType.NVarChar).Value = (txtModelo.Text + '.' + txtImei.Text).ToUpper();
                        cmd.Parameters.Add("@Marca", SqlDbType.NVarChar).Value = txtMarca.Text;
                        cmd.Parameters.Add("@fecha", SqlDbType.DateTime).Value = Convert.ToDateTime(dtpFecha.Text).Date;
                        cmd.Parameters.Add("@precio", SqlDbType.Decimal).Value = Convert.ToDecimal(txtprecio.Text);
                        cmd.Parameters.Add("@nota", SqlDbType.NVarChar).Value = txtnota.Text;

                        connectionCx.Connect();
                        cmd.ExecuteNonQuery();
                        connectionCx.Disconnect();

                        if (lblType.Text.ToLower() == "salida")
                        {
                            using (SqlCommand cmd2 = new SqlCommand("pagos_re", connectionCx.sqlConnection))
                            {
                                cmd2.CommandType = CommandType.StoredProcedure;

                                //Tabla de pago
                                cmd2.Parameters.Add("@id", SqlDbType.Int).Value = idreparacion;
                                cmd2.Parameters.Add("@monto", SqlDbType.Decimal).Value = Convert.ToDecimal(txtprecio.Text);

                                connectionCx.Connect();
                                cmd2.ExecuteNonQuery();
                                connectionCx.Disconnect();

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

                    limpiar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tickEstiloP()
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
            ticket.ImprimirTicket(Program.ImpressionPeq);//NOMBRE DE LA IMPRESORA
        }

        private void limpiar()
        {
            txtTelefono.Clear();
            txtModelo.Clear();
            txtprecio.Clear();
            txtnota.Clear();
            txtMarca.Clear();
            txtImei.Clear();
            txtCliente.Clear();
            txtAveria.Clear();
            idreparacion = 0;
        }

        private void llenar()
        {
            connectionCx.Disconnect();
            string cadSql = "select * from NomEmp";
            SqlCommand comando = new SqlCommand(cadSql, connectionCx.sqlConnection);
            connectionCx.Connect();
            SqlDataReader leer = comando.ExecuteReader();
            if (leer.Read())
            {
                lblDir.Text = leer["DirEmp"].ToString() ?? string.Empty;
                lblLogo.Text = leer["NombreEmp"].ToString() ?? "InOutSoft System";
                lblTel1.Text = leer["Tel1"].ToString() ?? string.Empty;
                lblTel2.Text = leer["Tel2"].ToString() ?? string.Empty;
                lblrnc.Text = leer["RNC"].ToString() ?? string.Empty;
                var impresoraPeq = leer["ImpresoraPeq"].ToString();
                Program.ImpressionPeq = string.IsNullOrWhiteSpace(impresoraPeq) ? "POS80 Printer" : impresoraPeq;
            }
            connectionCx.Disconnect();
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            this.Size = new Size(332, 610);
            PayListForm F = new PayListForm();
            F.lblLogo.Text = lblLogo.Text;
            F.lblDir.Text = lblDir.Text;
            F.Show();
        }

        private void txtprecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsPunctuation(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsPunctuation(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            limpiar();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PrintForm print = new PrintForm();
            print.Show();
        }

        public void Refresh()
        {
            if (idreparacion > 0)
            {
                this.Size = new Size(1130, 670);
                lblTypeTitle.Text = "Generar Salida";
                lblType.Text = "Salida";
                button1.Text = "Registrar";
                button1.BackColor = Color.SpringGreen;
                button1.ForeColor = Color.Black;

                if (!string.IsNullOrWhiteSpace(cliente))
                {
                    txtCliente.Text = cliente.Split('.')[0];
                    txtTelefono.Text = cliente.Split('.')[1];
                }

                txtAveria.Text = averia;
                if (!string.IsNullOrWhiteSpace(datos))
                {
                    txtModelo.Text = datos.Split('.')[0];
                    txtImei.Text = datos.Split('.')[1];
                }

                txtMarca.Text = marca;
                dtpFecha.Text = fecha;
                txtprecio.Text = precio;
                txtnota.Text = nota;


                // Perform any additional updates needed to refresh the form
                this.Update();
            }
        }
    }
}
