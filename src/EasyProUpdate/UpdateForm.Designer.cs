namespace EasyProUpdate
{
    partial class UpdateForm
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
            this.btUpdate = new System.Windows.Forms.Button();
            this.btLater = new System.Windows.Forms.Button();
            this.pctLogo = new System.Windows.Forms.PictureBox();
            this.lblVersions = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.rtbChangeLog = new System.Windows.Forms.RichTextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pctLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // btUpdate
            // 
            this.btUpdate.Image = global::EasyProUpdate.Properties.Resources.tick;
            this.btUpdate.Location = new System.Drawing.Point(111, 255);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(157, 25);
            this.btUpdate.TabIndex = 25;
            this.btUpdate.Text = "Télécharger maintenant";
            this.btUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btUpdate.UseVisualStyleBackColor = true;
            this.btUpdate.Click += new System.EventHandler(this.btUpdate_Click);
            // 
            // btLater
            // 
            this.btLater.Image = global::EasyProUpdate.Properties.Resources.cross;
            this.btLater.Location = new System.Drawing.Point(274, 255);
            this.btLater.Name = "btLater";
            this.btLater.Size = new System.Drawing.Size(146, 25);
            this.btLater.TabIndex = 24;
            this.btLater.Text = "Télécharger plus tard";
            this.btLater.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btLater.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btLater.UseVisualStyleBackColor = true;
            this.btLater.Click += new System.EventHandler(this.btLater_Click);
            // 
            // pctLogo
            // 
            this.pctLogo.Image = global::EasyProUpdate.Properties.Resources.Update;
            this.pctLogo.Location = new System.Drawing.Point(12, 12);
            this.pctLogo.Name = "pctLogo";
            this.pctLogo.Size = new System.Drawing.Size(80, 80);
            this.pctLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctLogo.TabIndex = 23;
            this.pctLogo.TabStop = false;
            // 
            // lblVersions
            // 
            this.lblVersions.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersions.ForeColor = System.Drawing.Color.White;
            this.lblVersions.Location = new System.Drawing.Point(112, 93);
            this.lblVersions.Name = "lblVersions";
            this.lblVersions.Size = new System.Drawing.Size(347, 42);
            this.lblVersions.TabIndex = 22;
            this.lblVersions.Text = "Version actuelle : -\r\nVersion de la mise à jour : -";
            // 
            // lblDescription
            // 
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.ForeColor = System.Drawing.Color.White;
            this.lblDescription.Location = new System.Drawing.Point(111, 48);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblDescription.Size = new System.Drawing.Size(348, 34);
            this.lblDescription.TabIndex = 21;
            this.lblDescription.Text = "Une nouvelle version de l\'application est disponible et peut-être téléchargée.";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rtbChangeLog
            // 
            this.rtbChangeLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.rtbChangeLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbChangeLog.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbChangeLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(49)))), ((int)(((byte)(58)))));
            this.rtbChangeLog.Location = new System.Drawing.Point(111, 149);
            this.rtbChangeLog.Name = "rtbChangeLog";
            this.rtbChangeLog.ReadOnly = true;
            this.rtbChangeLog.Size = new System.Drawing.Size(348, 83);
            this.rtbChangeLog.TabIndex = 19;
            this.rtbChangeLog.TabStop = false;
            this.rtbChangeLog.Text = "";
            this.rtbChangeLog.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbChangeLog_LinkClicked);
            this.rtbChangeLog.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbChangeLog_KeyDown);
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(108, 6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(351, 42);
            this.lblTitle.TabIndex = 26;
            this.lblTitle.Text = "Mise à jour disponible";
            // 
            // UpdateForm
            // 
            this.AcceptButton = this.btUpdate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.ClientSize = new System.Drawing.Size(512, 301);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btUpdate);
            this.Controls.Add(this.btLater);
            this.Controls.Add(this.pctLogo);
            this.Controls.Add(this.lblVersions);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.rtbChangeLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.pctLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btUpdate;
        private System.Windows.Forms.Button btLater;
        private System.Windows.Forms.PictureBox pctLogo;
        private System.Windows.Forms.Label lblVersions;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.RichTextBox rtbChangeLog;
        private System.Windows.Forms.Label lblTitle;
    }
}