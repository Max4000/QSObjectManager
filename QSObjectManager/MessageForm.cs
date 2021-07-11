using System;
using System.Windows.Forms;

namespace QSUsersHistoryManager
{
    public partial class MessageForm : Form
    {
        private string message;
        public MessageForm(string message)
        {
            this.message = message;
            InitializeComponent();
        }

        private void MessageForm_Load(object sender, EventArgs e)
        {
            this.label1.Text = this.message;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
