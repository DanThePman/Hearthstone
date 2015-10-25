namespace HearthstoneMulligan.USER_GUI
{
    partial class PopUp
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.yesButton = new MetroFramework.Controls.MetroButton();
            this.noButton = new MetroFramework.Controls.MetroButton();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // yesButton
            // 
            this.yesButton.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.yesButton.Highlight = true;
            this.yesButton.Location = new System.Drawing.Point(276, 90);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(75, 23);
            this.yesButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.yesButton.TabIndex = 1;
            this.yesButton.Text = "Yes";
            this.yesButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.yesButton.UseSelectable = true;
            this.yesButton.Click += new System.EventHandler(this.yesButton_Click);
            // 
            // noButton
            // 
            this.noButton.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.noButton.Location = new System.Drawing.Point(13, 90);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(75, 23);
            this.noButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.noButton.TabIndex = 2;
            this.noButton.Text = "No";
            this.noButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.noButton.UseMnemonic = false;
            this.noButton.UseSelectable = true;
            this.noButton.Click += new System.EventHandler(this.noButton_Click);
            // 
            // metroLabel1
            // 
            this.metroLabel1.Location = new System.Drawing.Point(13, 12);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(338, 75);
            this.metroLabel1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel1.TabIndex = 0;
            this.metroLabel1.Text = "An update for MulliganCore is available.\r\nDo you want to update now?\r\n\r\nNote: Sma" +
    "rtBot will close.";
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroLabel1.WrapToLine = true;
            // 
            // PopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.yesButton);
            this.Name = "PopUp";
            this.Size = new System.Drawing.Size(369, 204);
            this.ResumeLayout(false);

        }

        #endregion
        private MetroFramework.Controls.MetroButton noButton;
        private MetroFramework.Controls.MetroButton yesButton;
        private MetroFramework.Controls.MetroLabel metroLabel1;
    }
}
