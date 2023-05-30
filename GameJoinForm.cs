using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scrabble
{
    public partial class GameJoinForm : Form
    {
        public GameJoinForm()
        {
            InitializeComponent();
            
        }

        private void GameJoinForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if(txtIP.Text!=String.Empty && txtPort.Text != String.Empty && txtName.Text != String.Empty)
            {
                String ip = txtIP.Text;
                int port = int.Parse(txtPort.Text);
                string name = txtName.Text;
                TcpClient client = new TcpClient(ip, port);
                Form1 form = new Form1(client, name);
                form.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Please Provide all the Credentials!");
            }
        }
    }
}
