using System;
using HearthstoneMulligan.USER_GUI;
using MetroFramework.Forms;

namespace HearthstoneMulligan
{
    public partial class UpdatingWindow : MetroForm
    {
        private bool doPopUp = true;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_doPopUp">Decides if a popup information for updating the main core should
        /// be generated</param>
        public UpdatingWindow(bool _doPopUp = true)
        {
            doPopUp = _doPopUp;
            InitializeComponent();

            ControlBox = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BringToFront();

            if (doPopUp)
            {
                const int timeout = 10;

                MetroTaskWindow.ShowTaskWindow("Update - Information", new PopUp(), timeout);
            }
        }
    }
}
