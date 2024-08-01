using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace InOutSoft
{
    public partial class ListProducts : Form
    {
        ConnectionCx connectionCx = new ConnectionCx();
        public static ListProducts Instance;
        public ListProducts()
        {
            InitializeComponent();
            connectionCx.connection();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                connectionCx.Disconnect();
                var id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["idProducto"].Value.ToString());
                if (id > 0)
                {
                    if (MessageBox.Show("¿Está Seguro que Desea Eliminar?", "In/OutSoft System", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    {
                        using (SqlCommand cmd = new SqlCommand("EliminarProducto", connectionCx.sqlConnection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@IdProducto", SqlDbType.Int).Value = id;

                            connectionCx.Connect();
                            cmd.ExecuteNonQuery();
                            connectionCx.Disconnect();
                        }
                    }
                    button3.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Por Favor Seleccione una fila antes de eliminarla");
                    button3.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            buscar();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            limpiar();
            buscar();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            To_pdf();
        }

        public void buscar()
        {
            connectionCx.Disconnect();
            double total = 0;
            dataGridView1.Rows.Clear();
            //variable de tipo Sqlcommand
            SqlCommand comando = new SqlCommand();
            //variable SqlDataReader para leer los datos
            SqlDataReader dr;
            comando.Connection = connectionCx.sqlConnection;
            //declaramos el comando para realizar la busqueda

            var query = string.Format("select IdProducto,Nombre,Marca,Cantidad,PrecioCompra,PrecioVenta from Producto {0} ORDER BY IdProducto", string.IsNullOrWhiteSpace(textBox1.Text) ? string.Empty : " Where Nombre = '" + textBox1.Text + "'");
            comando.CommandText = query;

            //especificamos que es de tipo Text
            comando.CommandType = CommandType.Text;
            //se abre la conexion
            connectionCx.Connect();
            //limpiamos los renglones de la datagridview
            dataGridView1.Rows.Clear();
            //a la variable DataReader asignamos  el la variable de tipo SqlCommand
            dr = comando.ExecuteReader();
            while (dr.Read())
            {
                //variable de tipo entero para ir enumerando los la filas del datagridview
                int renglon = dataGridView1.Rows.Add();

                // especificamos en que fila se mostrará cada registro
                // nombredeldatagrid.filas[numerodefila].celdas[nombrdelacelda].valor=\
                dataGridView1.Rows[renglon].Cells["idProducto"].Value = Convert.ToString(dr.GetInt32(dr.GetOrdinal("IdProducto")));
                dataGridView1.Rows[renglon].Cells["nombreProducto"].Value = dr.GetString(dr.GetOrdinal("Nombre"));
                dataGridView1.Rows[renglon].Cells["Marca"].Value = dr.GetString(dr.GetOrdinal("Marca"));
                dataGridView1.Rows[renglon].Cells["cantidad"].Value = Convert.ToString(dr.GetDecimal(dr.GetOrdinal("Cantidad")));
                dataGridView1.Rows[renglon].Cells["precioC"].Value = Convert.ToString(dr.GetDecimal(dr.GetOrdinal("PrecioCompra")));
                dataGridView1.Rows[renglon].Cells["precioV"].Value = Convert.ToString(dr.GetDecimal(dr.GetOrdinal("PrecioVenta")));
            }

            connectionCx.Disconnect();
        }

        private void limpiar()
        {
            dataGridView1.Rows.Clear();
            textBox1.Clear();
        }

        #region PDF
        private void To_pdf()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = @"C:";
            saveFileDialog1.Title = "Guardar Reporte";
            saveFileDialog1.DefaultExt = "pdf";
            saveFileDialog1.Filter = "pdf Files (*.pdf)|*.pdf| All Files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            string filename = "Reporte" + DateTime.Now.ToString();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = saveFileDialog1.FileName;
            }

            if (filename.Trim() != "")
            {
                Document doc = new Document(PageSize.LETTER, 10f, 10f, 10f, 0f);
                FileStream file = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                PdfWriter.GetInstance(doc, file);
                doc.Open();
                string remito = lblLogo.Text;
                string ubicado = lblDir.Text;
                string envio = "Fecha : " + DateTime.Now.ToString();

                iTextSharp.text.Chunk chunk = new iTextSharp.text.Chunk(remito, FontFactory.GetFont("ARIAL", 16, iTextSharp.text.Font.BOLD, color: BaseColor.BLUE));
                doc.Add(new Paragraph("                                                                                                                                                                                                                                                     " + envio, FontFactory.GetFont("ARIAL", 7, iTextSharp.text.Font.ITALIC)));
                doc.Add(new Paragraph(chunk));
                doc.Add(new Paragraph(ubicado, FontFactory.GetFont("ARIAL", 9, iTextSharp.text.Font.NORMAL)));
                doc.Add(new Paragraph("                       "));
                doc.Add(new Paragraph("Reporte de Listado de Productos en Inventario                       "));
                doc.Add(new Paragraph("                       "));
                GenerarDocumento(doc);
                doc.AddCreationDate();
                doc.Add(new Paragraph("                       "));
                doc.Close();
                Process.Start(filename);//Esta parte se puede omitir, si solo se desea guardar el archivo, y que este no se ejecute al instante
            }
        }
        private void GenerarDocumento(Document document)
        {
            int i, j;
            PdfPTable datatable = new PdfPTable(dataGridView1.ColumnCount);
            datatable.DefaultCell.Padding = 3;
            float[] headerwidths = GetTamañoColumnas(dataGridView1);
            datatable.SetWidths(headerwidths);
            datatable.WidthPercentage = 100;
            datatable.DefaultCell.BorderWidth = 1;
            datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            for (i = 0; i < dataGridView1.ColumnCount; i++)
            {
                datatable.AddCell(dataGridView1.Columns[i].HeaderText);
            }
            datatable.HeaderRows = 1;
            datatable.DefaultCell.BorderWidth = 1;
            for (i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    if (dataGridView1[j, i].Value != null)
                    {
                        datatable.AddCell(new Phrase(dataGridView1[j, i].Value.ToString(), FontFactory.GetFont("ARIAL", 8, iTextSharp.text.Font.NORMAL)));//En esta parte, se esta agregando un renglon por cada registro en el datagrid
                    }
                }
                datatable.CompleteRow();
            }
            document.Add(datatable);
        }
        private float[] GetTamañoColumnas(DataGridView dg)
        {
            float[] values = new float[dg.ColumnCount];
            for (int i = 0; i < dg.ColumnCount; i++)
            {
                values[i] = (float)dg.Columns[i].Width;
            }
            return values;
        }
        #endregion

        private void ListProducts_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button1.BackColor = Color.SteelBlue;
            button1.Text = "Nuevo";
            buscar();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[dataGridView1.CurrentRow.Index].Selected = true;
                button3.Enabled = true;
            }

            button1.Text = "Editar";
            button1.BackColor = Color.MediumSeaGreen;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var cantidad = Convert.ToDouble(dataGridView1.CurrentRow.Cells["cantidad"].Value);
            if (cantidad > 0)
            {
                HomeForm.selectedProducts.Add(new Model.SelectedProducts()
                {
                    idProduct = Convert.ToInt32(dataGridView1.CurrentRow.Cells["idProducto"].Value),
                    Name = dataGridView1.CurrentRow.Cells["nombreProducto"].Value.ToString(),
                    Price = Convert.ToDouble(dataGridView1.CurrentRow.Cells["precioC"].Value),
                    PriceV = Convert.ToDouble(dataGridView1.CurrentRow.Cells["precioV"].Value),
                });

                HomeForm.Instance.Refresh();
                this.Close();
            }
            else
                MessageBox.Show("Seleccione un producto con cantidad mayor a cero o actualice el producto con la cantidad existente.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProductForm product = new ProductForm();
            if (button1.Text == "Editar")
            {
                ProductForm.selectedProducts = new Model.SelectedProducts()
                {
                    idProduct = Convert.ToInt32(dataGridView1.CurrentRow.Cells["idProducto"].Value),
                    Name = dataGridView1.CurrentRow.Cells["nombreProducto"].Value.ToString(),
                    Price = Convert.ToDouble(dataGridView1.CurrentRow.Cells["precioC"].Value),
                    PriceV = Convert.ToDouble(dataGridView1.CurrentRow.Cells["precioV"].Value),
                    Quantity = Convert.ToDouble(dataGridView1.CurrentRow.Cells["cantidad"].Value),
                    Marca = dataGridView1.CurrentRow.Cells["Marca"].Value.ToString(),
                };

                product.Refresh();
                product.Show();
            }
            else
            {
                product.Show();
            }
        }
    }
}
