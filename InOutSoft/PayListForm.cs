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
    public partial class PayListForm : Form
    {
        ConnectionCx connectionCx = new ConnectionCx();
        public PayListForm()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            buscar(true);
        }

        private void PayListForm_Load(object sender, EventArgs e)
        {
            buscar(false);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            To_pdf();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            limpiar();
        }

        private void buscar(bool fromButton)
        {
            connectionCx.Disconnect();
            double total = 0;
            //variable de tipo Sqlcommand
            SqlCommand comando = new SqlCommand();
            //variable SqlDataReader para leer los datos
            SqlDataReader dr;
            comando.Connection = connectionCx.sqlConnection;
            //declaramos el comando para realizar la busqueda
            comando.CommandText = "select * from Pagos where fecha = convert(datetime, CONVERT(varchar(10), @fecha1, 103), 103) ORDER BY id_pago";
            if (fromButton)
                comando.Parameters.AddWithValue("@fecha1", dtpfecha1.Value.Date);
            else
                comando.Parameters.AddWithValue("@fecha1", DateTime.Today.Date);

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
                dataGridView1.Rows[renglon].Cells["id"].Value = Convert.ToString(dr.GetInt32(dr.GetOrdinal("id_pago")));
                dataGridView1.Rows[renglon].Cells["monto"].Value = Convert.ToString(dr.GetDecimal(dr.GetOrdinal("monto")));
                dataGridView1.Rows[renglon].Cells["fecha"].Value = dr.GetDateTime(dr.GetOrdinal("fecha")).ToString("dd/MM/yyyy");

                total += Convert.ToDouble(dataGridView1.Rows[renglon].Cells["monto"].Value);
            }

            txttotalG.Text = total.ToString();
            connectionCx.Disconnect();
        }

        private void limpiar()
        {
            dataGridView1.Rows.Clear();
            buscar(false);
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
                doc.Add(new Paragraph("Reporte de Listado de Pagos                       "));
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
    }
}
