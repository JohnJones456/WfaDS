using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WfaDS
{
    public partial class Menu_Geren : Form
    {
        private bool confirmacaoRealizada = false;

        public Menu_Geren()
        {
            InitializeComponent();
        }

        private void Menu_Geren_Load(object sender, EventArgs e)
        {
            timerHora.Start();
        }

        private void Menu_Geren_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (confirmacaoRealizada)
                return;

            DialogResult resultado = MessageBox.Show(
                "Tem certeza que deseja encerrar o programa?",
                "Confirmar Saída",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                confirmacaoRealizada = true;
            }
        }

        private void Menu_Geren_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void timerHora_Tick(object sender, EventArgs e)
        {
            lblHora.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void btnAeronaves_Click(object sender, EventArgs e)
        {

        }

        private void btnPilotos_Click(object sender, EventArgs e)
        {

        }

        private void gerenciarAeronaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Aeronaves_gen fr = new Aeronaves_gen();
            fr.Show();
        }

        private void gerenciarPilotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pilotos_gen fr = new Pilotos_gen();
            fr.Show();
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show("Tem certeza que deseja sair?", "Confirmação de Saída", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

    }
}