namespace NJN1DBackup
{
    partial class Signin
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wbSignin = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // wbSignin
            // 
            this.wbSignin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbSignin.Location = new System.Drawing.Point(0, 0);
            this.wbSignin.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbSignin.Name = "wbSignin";
            this.wbSignin.Size = new System.Drawing.Size(427, 340);
            this.wbSignin.TabIndex = 0;
            this.wbSignin.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wbSignin_DocumentCompleted);
            this.wbSignin.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.wbSignin_Navigated);
            // 
            // Signin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 340);
            this.Controls.Add(this.wbSignin);
            this.Name = "Signin";
            this.Text = "Signin";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbSignin;
    }
}