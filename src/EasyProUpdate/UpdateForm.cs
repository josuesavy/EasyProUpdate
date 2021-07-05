using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyProUpdate
{
    public partial class UpdateForm : Form
    {
        /// <summary>
        /// Lors du chargement de la fenêtre de Mise à jour
        /// </summary>
        /// <param name="remindLater"></param>
        public UpdateForm(bool remindLater = false)
        {
            if (!remindLater)
            {
                InitializeComponent();
                var resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateForm));
                Text = EPUpdate.DialogTitle;
                lblVersions.Text = String.Format("Version courante : {0}\nMise à jour version : {1}", EPUpdate.InstalledVersion, EPUpdate.UpdateVersion);
                rtbChangeLog.Text = EPUpdate.ChangeLog;
                int countClick = 1;
            }
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Evènement du bouton "Télécharger maintenant"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (EPUpdate.OpenDownloadPage)
            {
                var processStartInfo = new ProcessStartInfo(EPUpdate.DownloadURL);
                Process.Start(processStartInfo);
            }
            else
                EPUpdate.DownloadUpdate();
        }

        /// <summary>
        /// Evènement du bouton "Télécharger plus tard"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btLater_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void rtbChangeLog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
                e.SuppressKeyPress = true;
        }

        private void rtbChangeLog_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}
