using System;
using MetroFramework.Forms;

namespace HearthstoneMulligan.USER_GUI
{
    public partial class UpdatingWindow : MetroForm
    {
        private bool doPopUp;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_doPopUp">Decides if a popup information for updating the main core should
        /// be generated</param>
        public UpdatingWindow(bool _doPopUp)
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

                MetroTaskWindow.ShowTaskWindow("Update - Information", new PopUp(this), timeout);
            }
        }
    }
}
