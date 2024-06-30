using System;
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
            this.Size = new System.Drawing.Size(335, 610);
            ListForm F = new ListForm();
            F.Show();
        }

        private void btnList_Click(object sender, System.EventArgs e)
        {
            this.Size = new System.Drawing.Size(335, 610);
            ListForm F = new ListForm();
            F.Show();
        }

        private void btnIn_Click(object sender, System.EventArgs e)
        {
            this.Size = new System.Drawing.Size(1138, 670);
            lblTypeTitle.Text = "Generar Entrada";
            lblType.Text = "Entrada";
        }

        private void btnOut_Click(object sender, System.EventArgs e)
        {
            this.Size = new System.Drawing.Size(1138, 670);
            lblTypeTitle.Text = "Generar Salida";
            lblType.Text = "Salida";
        }

        private void HomeForm_Load(object sender, System.EventArgs e)
        {
            this.Size = new System.Drawing.Size(335, 610);
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

        }
    }
}
