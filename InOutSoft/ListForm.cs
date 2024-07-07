using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace InOutSoft
{
    public partial class ListForm : Form
    {
        ConnectionCx connectionCx = new ConnectionCx();
        public ListForm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                connectionCx.Disconnect();
                var id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value.ToString());
                if (id > 0)
                {
                    if (MessageBox.Show("¿Está Seguro que Desea Eliminar?", "In/OutSoft System", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    {
                        var tipo = dataGridView1.CurrentRow.Cells["tipoDeTrabajo"].Value.ToString();
                        using (SqlCommand cmd = new SqlCommand("eliminarTaller", connectionCx.sqlConnection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                            cmd.Parameters.Add("@tipoDeTrabajo", SqlDbType.NVarChar).Value = tipo;

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
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            To_pdf();
        }

        private void buscar()
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


            var query = string.Format("select * from Taller where {0} {1} fecha BETWEEN convert(datetime, CONVERT(varchar(10), @fecha1, 103), 103) AND convert(datetime, CONVERT(varchar(10), @fecha2, 103), 103) ORDER BY id",
                                      (!string.IsNullOrWhiteSpace(cbtipo.Text) ? string.Format("tipoDeTrabajo = '{0}' and", cbtipo.Text) : string.Empty),
                                      (!string.IsNullOrWhiteSpace(textBox1.Text) ? string.Format("Datos like '%{0}%' and", textBox1.Text.ToUpper()) : string.Empty));

            comando.CommandText = query;
            comando.Parameters.AddWithValue("@fecha1", dtpfecha1.Value.Date);
            comando.Parameters.AddWithValue("@fecha2", dtpfecha2.Value.Date);

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
                dataGridView1.Rows[renglon].Cells["id"].Value = Convert.ToString(dr.GetInt32(dr.GetOrdinal("id")));
                dataGridView1.Rows[renglon].Cells["tipoDeTrabajo"].Value = dr.GetString(dr.GetOrdinal("tipoDeTrabajo"));
                dataGridView1.Rows[renglon].Cells["cliente"].Value = dr.GetString(dr.GetOrdinal("cliente"));
                dataGridView1.Rows[renglon].Cells["marca"].Value = dr.GetString(dr.GetOrdinal("Marca"));
                dataGridView1.Rows[renglon].Cells["modelo"].Value = dr.GetString(dr.GetOrdinal("Datos"));
                dataGridView1.Rows[renglon].Cells["averia"].Value = dr.GetString(dr.GetOrdinal("averia"));
                dataGridView1.Rows[renglon].Cells["precio"].Value = Convert.ToString(dr.GetDecimal(dr.GetOrdinal("precio")));
                dataGridView1.Rows[renglon].Cells["nota"].Value = dr.GetString(dr.GetOrdinal("nota"));
                dataGridView1.Rows[renglon].Cells["fecha"].Value = dr.GetDateTime(dr.GetOrdinal("fecha")).ToString("dd/MM/yyyy");

                total += Convert.ToDouble(dataGridView1.Rows[renglon].Cells["precio"].Value);
            }

            txttotalG.Text = Convert.ToString(total);
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
                doc.Add(new Paragraph("Reporte de Listado de Celulares en Reparacion                       "));
                doc.Add(new Paragraph("                       "));
                GenerarDocumento(doc);
                doc.AddCreationDate();
                doc.Add(new Paragraph("                       "));
                doc.Add(new Paragraph("Total de Ventas      : " + txttotalG.Text));
                doc.Add(new Paragraph("                       "));
                doc.Add(new Paragraph("____________________________________"));
                doc.Add(new Paragraph("                         Firma              "));
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

        private void ListForm_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;

            cbtipo.DisplayMember = "descripcion";
            cbtipo.ValueMember = "id";
            cbtipo.Items.Add("AMBOS");
            cbtipo.Items.Add("ENTRADA");
            cbtipo.Items.Add("SALIDA");
            cbtipo.SelectedIndex = 0;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[dataGridView1.CurrentRow.Index].Selected = true;
                button3.Enabled = true;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            HomeForm.Instance.idreparacion = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value.ToString());
            HomeForm.Instance.descripcion = dataGridView1.CurrentRow.Cells["tipoDeTrabajo"].Value.ToString();
            HomeForm.Instance.marca = dataGridView1.CurrentRow.Cells["marca"].Value.ToString();
            HomeForm.Instance.averia = dataGridView1.CurrentRow.Cells["averia"].Value.ToString();
            HomeForm.Instance.cliente = dataGridView1.CurrentRow.Cells["cliente"].Value.ToString();
            HomeForm.Instance.precio = dataGridView1.CurrentRow.Cells["precio"].Value.ToString();
            HomeForm.Instance.datos = dataGridView1.CurrentRow.Cells["modelo"].Value.ToString();
            HomeForm.Instance.nota = dataGridView1.CurrentRow.Cells["nota"].Value.ToString();
            HomeForm.Instance.Refresh();
            this.Close();
        }
    }
}
