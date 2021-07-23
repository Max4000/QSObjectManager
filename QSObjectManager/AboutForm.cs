using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace QSObjectManager
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;

            Process.Start("explorer.exe", "https://qubdata.ru");
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            linkLabel1.LinkArea = new LinkArea(0, 20);

            

            label1.Text =
@"Менеджер объектов Qlik Sence
                  ver 1.0.1 
     Анатолий Барановский";
        }
    }
}
