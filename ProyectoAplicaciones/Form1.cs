using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProyectoAplicaciones
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Evento Click del botón 
        private void btnServidor_Click(object sender, EventArgs e)
        {
            // Creo un nuevo form llamado servidor
            Servidor servidor = new Servidor();
            // El nuevo form utilizo la propiedad ShowDialog
            servidor.ShowDialog();
        }

        private void btnCliente_Click(object sender, EventArgs e)
        {
            // Creo un nuevo form llamado cliente
            Cliente cliente = new Cliente();
            // El nuevo form utilizo la propiedad ShowDialog
            cliente.ShowDialog();

        }
    }
}
