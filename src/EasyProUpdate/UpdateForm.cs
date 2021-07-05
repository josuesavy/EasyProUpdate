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
        public UpdateForm(bool remindLater = false)
        {
            if (!remindLater)
            {
                InitializeComponent();
                var resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateForm));
                Text = EPUpdate.DialogTitle;
                lblVersions.Text = String.Format("Version courante : {0}\nMise à jour version : {1}", EPUpdate.InstalledVersion, EPUpdate.UpdateVersion);
                rtbChangeLog.Text = EPUpdate.ChangeLog;
            }
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (EPUpdate.OpenDownloadPage)
            {
                var processStartInfo = new ProcessStartInfo(EPUpdate.DownloadURL);
                Process.Start(processStartInfo);
            }
            else
            {
                EPUpdate.DownloadUpdate();
            }
        }

        private void btLater_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
