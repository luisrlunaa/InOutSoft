using InOutSoft.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InOutSoft
{
    public partial class ProductForm : Form
    {
        ConnectionCx connectionCx = new ConnectionCx();
        public static SelectedProducts selectedProducts;
        public static ProductForm Instance;
        public ProductForm()
        {
            InitializeComponent();
            connectionCx.connection();
            Instance = this;
            selectedProducts = new SelectedProducts();
        }

        private void ProductForm_Load(object sender, EventArgs e)
        {
            if (selectedProducts != null && selectedProducts.idProduct > 0)
            {
                button1.Visible = true;
                btnGuardar.Visible = false;
            }
            else
            {
                button1.Visible = false;
                btnGuardar.Visible = true;
            }
        }

        public void Refresh()
        {
            txtProducto.Text = selectedProducts.Name;
            txtMarca.Text = selectedProducts.Marca;
            txtPCompra.Text = selectedProducts.Price.ToString();
            txtPVenta.Text = selectedProducts.PriceV.ToString();
            txtStock.Text = selectedProducts.Quantity.ToString();
            txtProductId.Text = selectedProducts.idProduct.ToString();

            if (selectedProducts != null && selectedProducts.idProduct > 0)
            {
                button1.Visible = true;
                btnGuardar.Visible = false;
            }
            else
            {
                button1.Visible = false;
                btnGuardar.Visible = true;
            }

            // Perform any additional updates needed to refresh the form
            this.Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtProducto.Text.Trim() != "")
            {
                if (txtMarca.Text.Trim() != "")
                {
                    if (txtPCompra.Text.Trim() != "")
                    {
                        if (txtPVenta.Text.Trim() != "")
                        {
                            if (txtStock.Text.Trim() != "")
                            {
                                using (SqlCommand cmd = new SqlCommand("ActualizarProducto", connectionCx.sqlConnection))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Add("@IdProducto", SqlDbType.Int).Value = Convert.ToInt32(txtProductId.Text);
                                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar).Value = txtProducto.Text;
                                    cmd.Parameters.Add("@Marca", SqlDbType.NVarChar).Value = txtMarca.Text;
                                    cmd.Parameters.Add("@Cantidad", SqlDbType.Decimal).Value = Convert.ToDecimal(txtStock.Text);
                                    cmd.Parameters.Add("@PrecioCompra", SqlDbType.Decimal).Value = Convert.ToDecimal(txtPCompra.Text);
                                    cmd.Parameters.Add("@PrecioVenta", SqlDbType.Decimal).Value = Convert.ToDecimal(txtPVenta.Text);

                                    connectionCx.Connect();
                                    cmd.ExecuteNonQuery();
                                    connectionCx.Disconnect();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Por Favor Ingrese Stock del Producto.", "Sistema de Ventas.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtStock.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Por Favor Ingrese Precio de Venta del Producto.", "Sistema de Ventas.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtPVenta.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Por Favor Ingrese Precio de Compra del Producto.", "Sistema de Ventas.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPCompra.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Por Favor Ingrese Marca del Producto.", "Sistema de Ventas.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMarca.Focus();
                }
            }
            else
            {
                MessageBox.Show("Por Favor Ingrese Nombre del Producto.", "Sistema de Ventas.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtProducto.Focus();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtProducto.Text.Trim() != "")
            {
                if (txtMarca.Text.Trim() != "")
                {
                    if (txtPCompra.Text.Trim() != "")
                    {
                        if (txtPVenta.Text.Trim() != "")
                        {
                            if (txtStock.Text.Trim() != "")
                            {
                                using (SqlCommand cmd = new SqlCommand("RegistrarProducto", connectionCx.sqlConnection))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar).Value = txtProducto.Text;
                                    cmd.Parameters.Add("@Marca", SqlDbType.NVarChar).Value = txtMarca.Text;
                                    cmd.Parameters.Add("@Cantidad", SqlDbType.Decimal).Value = Convert.ToDecimal(txtStock.Text);
                                    cmd.Parameters.Add("@PrecioCompra", SqlDbType.Decimal).Value = Convert.ToDecimal(txtPCompra.Text);
                                    cmd.Parameters.Add("@PrecioVenta", SqlDbType.Decimal).Value = Convert.ToDecimal(txtPVenta.Text);

                                    connectionCx.Connect();
                                    cmd.ExecuteNonQuery();
                                    connectionCx.Disconnect();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Por Favor Ingrese Stock del Producto.", "Sistema de Ventas.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtStock.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Por Favor Ingrese Precio de Venta del Producto.", "Sistema de Ventas.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtPVenta.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Por Favor Ingrese Precio de Compra del Producto.", "Sistema de Ventas.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtPCompra.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Por Favor Ingrese Marca del Producto.", "Sistema de Ventas.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMarca.Focus();
                }
            }
            else
            {
                MessageBox.Show("Por Favor Ingrese Nombre del Producto.", "Sistema de Ventas.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtProducto.Focus();
            }
        }
    }
}
