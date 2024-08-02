using Capa_de_Presentacion;
using InOutSoft.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace InOutSoft
{
    public partial class HomeForm : Form
    {
        ConnectionCx connectionCx = new ConnectionCx();
        public static HomeForm Instance;
        public static List<SelectedProducts> selectedProducts;
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

        public string WindUser;
        public string SqlFolder;

        public HomeForm()
        {
            InitializeComponent();
            connectionCx.connection();
            Instance = this;
            selectedProducts = new List<SelectedProducts>();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Size = new Size(325, 600);
            ListForm F = new ListForm();
            F.lblLogo.Text = lblLogo.Text;
            F.lblDir.Text = lblDir.Text;
            F.Show();
        }

        private void btnList_Click(object sender, System.EventArgs e)
        {
            this.Size = new Size(325, 600);
            ListForm F = new ListForm();
            F.lblLogo.Text = lblLogo.Text;
            F.lblDir.Text = lblDir.Text;
            F.Show();
        }

        private void btnIn_Click(object sender, System.EventArgs e)
        {
            this.Size = new Size(900, 600);
            button4.Visible = false;
            dgvVenta.Visible = false;
            dgvVenta.Rows.Clear();
            lblTypeTitle.Text = "Generar Entrada";
            lblType.Text = "Entrada";
            button1.Text = "Guardar";
            button1.BackColor = Color.RoyalBlue;
            button1.ForeColor = Color.White;
        }

        private void btnOut_Click(object sender, System.EventArgs e)
        {
            this.Size = new Size(900, 600);
            button4.Visible = true;
            dgvVenta.Visible = true;
            lblTypeTitle.Text = "Generar Salida";
            lblType.Text = "Salida";
            button1.Text = "Registrar";
            button1.BackColor = Color.SpringGreen;
            button1.ForeColor = Color.Black;
        }

        private void HomeForm_Load(object sender, System.EventArgs e)
        {
            llenar();

            this.Size = new Size(325, 600);
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
                    using (SqlCommand cmd = new SqlCommand("RegistrarTaller", connectionCx.sqlConnection))
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

                        try
                        {
                            connectionCx.Connect();
                            cmd.ExecuteNonQuery();
                            connectionCx.Disconnect();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al guardar registro de taller. \n" + ex);
                            return;
                        }

                        if (lblType.Text.ToLower() == "salida")
                        {
                            using (SqlCommand cmd1 = new SqlCommand("RegistrarDetalleTaller", connectionCx.sqlConnection))
                            {
                                var priceProductsInList = selectedProducts;
                                foreach (DataGridViewRow row in dgvVenta.Rows)
                                {
                                    connectionCx.Disconnect();
                                    cmd1.CommandType = CommandType.StoredProcedure;

                                    int idventa = 0;
                                    int idProduct = Convert.ToInt32(row.Cells["IdProducto"].Value);
                                    var precio = Convert.ToDouble(row.Cells["PrecioU"].Value);
                                    var cantidad = Convert.ToDouble(row.Cells["Cantidad"].Value);
                                    var ganancia = (precio - priceProductsInList?.FirstOrDefault(x => x.idProduct == idProduct)?.Price) * cantidad;

                                    //Tabla detalles
                                    cmd1.Parameters.Add("@IdTaller", SqlDbType.Int).Value = idventa;
                                    cmd1.Parameters.Add("@IdProducto", SqlDbType.Int).Value = idProduct;
                                    cmd1.Parameters.Add("@NombreProducto", SqlDbType.NVarChar).Value = Convert.ToString(row.Cells["NombreProducto"].Value);
                                    cmd1.Parameters.Add("@Precio", SqlDbType.Float).Value = precio;
                                    cmd1.Parameters.Add("@Ganancia", SqlDbType.Float).Value = ganancia;
                                    cmd1.Parameters.Add("@Cantidad", SqlDbType.Float).Value = cantidad;
                                    cmd1.Parameters.Add("@SubTotal", SqlDbType.Float).Value = Convert.ToDouble(row.Cells["SubtoTal"].Value);

                                    try
                                    {
                                        connectionCx.connection();
                                        cmd1.ExecuteNonQuery();
                                        cmd1.Parameters.Clear();
                                        connectionCx.Disconnect();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Error al guardar los detalles. \n" + ex);
                                        return;
                                    }
                                }
                            }

                            using (SqlCommand cmd2 = new SqlCommand("RegistrarPago", connectionCx.sqlConnection))
                            {
                                cmd2.CommandType = CommandType.StoredProcedure;

                                //Tabla de pago
                                cmd2.Parameters.Add("@id", SqlDbType.Int).Value = idreparacion;
                                cmd2.Parameters.Add("@monto", SqlDbType.Decimal).Value = Convert.ToDecimal(txtprecio.Text);

                                try
                                {
                                    connectionCx.Connect();
                                    cmd2.ExecuteNonQuery();
                                    connectionCx.Disconnect();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error al guardar Pago. \n" + ex);
                                    return;
                                }

                                tickEstiloP();
                                MessageBox.Show("Salida Registrada y Pago Confirmado");
                                selectedProducts = new List<SelectedProducts>();
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
                WindUser = leer["WindowsUserName"].ToString() ?? string.Empty;
                SqlFolder = leer["SqlFolderName"].ToString() ?? string.Empty;
                Program.ImpressionPeq = string.IsNullOrWhiteSpace(impresoraPeq) ? "POS80 Printer" : impresoraPeq;
            }
            connectionCx.Disconnect();
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            this.Size = new Size(325, 600);
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
                this.Size = new Size(900, 600);
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
            }

            if (selectedProducts != null && selectedProducts.Any() && lblType.Text.ToLower() == "salida")
            {
                var prod = selectedProducts.LastOrDefault();
                lblNameP.Text = prod.Name;
                lblPriceP.Text = prod.PriceV.ToString();
                txtQuantity.Text = prod.Quantity.ToString();

            }

            // Perform any additional updates needed to refresh the form
            this.Update();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (selectedProducts != null && selectedProducts.Any() && !string.IsNullOrWhiteSpace(txtQuantity.Text) && Convert.ToDecimal(txtQuantity.Text) > 0)
            {
                var prod = selectedProducts.LastOrDefault();
                var quantity = Convert.ToDouble(selectedProducts?.Where(x => x.idProduct == prod.idProduct && x != prod)?.ToList()?.Sum(y => y.Quantity) + Convert.ToDouble(txtQuantity.Text));
                prod.Quantity = quantity;

                selectedProducts = selectedProducts.Where(x => x.idProduct != prod.idProduct).ToList();
                selectedProducts.Add(prod);

                dgvVenta.Rows.Clear();
                for (int i = 0; i < selectedProducts.Count; i++)
                {
                    dgvVenta.Rows.Add();
                    dgvVenta.Rows[i].Cells["IdProducto"].Value = selectedProducts[i].idProduct;
                    dgvVenta.Rows[i].Cells["NombreProducto"].Value = selectedProducts[i].Name;
                    dgvVenta.Rows[i].Cells["Cantidad"].Value = selectedProducts[i].Quantity;
                    dgvVenta.Rows[i].Cells["PrecioU"].Value = selectedProducts[i].PriceV;
                    dgvVenta.Rows[i].Cells["SubtoTal"].Value = (selectedProducts[i].Quantity * selectedProducts[i].PriceV);
                }

                lblNameP.Text = "...";
                lblPriceP.Text = "...";
                txtQuantity.Clear();
            }
            else
                MessageBox.Show("Seleccione un producto del listado para poder agregar");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ListProducts F = new ListProducts();
            F.lblLogo.Text = lblLogo.Text;
            F.lblDir.Text = lblDir.Text;
            F.Show();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            var dbName = connectionCx.sqlConnection.Database;
            var dirs = new DirectoryInfo(@"" + SqlFolder).FullName;
            string fileName = "_" + dbName + "_" + DateTime.Now.ToShortDateString().Replace("/", "-") + ".bak";
            ReintentBackup(dbName, dirs, fileName);
        }

        private void ReintentBackup(string dbName, string dirs, string fileName)
        {
            bool continueLoop = true;

            while (continueLoop)
            {
                try
                {
                    var (save, message) = MakeBackup(dirs, connectionCx.connectionString, dbName, fileName);
                    if (save)
                    {
                        var destination = @"" + WindUser + "\\" + fileName;
                        if (File.Exists(destination))
                        {
                            File.Delete(destination);
                        }

                        File.Move(dirs + "\\" + fileName, destination);
                        MessageBox.Show("Copia de seguridad de base de datos realizado y guardado");
                        continueLoop = false;
                        Application.Exit();
                    }
                    else
                    {
                        MessageBox.Show("Error al realizar la Copia de seguridad de base de datos");
                        DialogResult result = MessageBox.Show("¿Desea intentar nuevamente guardar la copia de seguridad?", "Sistema de Ventas", MessageBoxButtons.YesNo);

                        if (result == DialogResult.No)
                        {
                            continueLoop = false;
                            // Exit the application if the user chooses not to continue
                            Application.Exit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    DialogResult result = MessageBox.Show("¿Desea intentar nuevamente guardar la copia de seguridad?", "Sistema de Ventas", MessageBoxButtons.YesNo);

                    if (result == DialogResult.No)
                    {
                        continueLoop = false;
                        // Exit the application if the user chooses not to continue
                        Application.Exit();
                    }
                }
            }
        }

        public (bool success, string message) MakeBackup(string ubicacion, string strConnection, string dbName, string nombre)
        {
            var con = new SqlConnection(strConnection);
            var cmd = new SqlCommand("BACKUP DATABASE " + dbName + " TO DISK='" + ubicacion + "/" + nombre + "'", con);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                return (true, "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                con.Close();
                return (false, ex.Message.ToString());
            }
        }
    }
}
