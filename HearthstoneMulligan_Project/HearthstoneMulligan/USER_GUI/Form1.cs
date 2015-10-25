using System;
using HearthstoneMulligan.USER_GUI;
using MetroFramework.Forms;

namespace HearthstoneMulligan
{
    public partial class UpdatingWindow : MetroForm
    {
        public UpdatingWindow()
        {
            InitializeComponent();

            ControlBox = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BringToFront();

            const int timeout = 10;

            MetroTaskWindow.ShowTaskWindow("Update - Information", new PopUp(), timeout);
        }
    }
}
