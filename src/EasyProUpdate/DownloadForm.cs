using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace EasyProUpdate
{
    public partial class DownloadForm : Form
    {
        private readonly string _downloadURL;

        private string _tempPath;

        private WebClient _webClient;

        Stopwatch sw = new Stopwatch();

        public DownloadForm(string downloadURL)
        {
            InitializeComponent();

            _downloadURL = downloadURL;
        }

        /// <summary>
        /// Lors du chargement de la fenêtre de téléchargement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadForm_Load(object sender, EventArgs e)
        {
            _webClient = new WebClient();

            var uri = new Uri(_downloadURL);

            _tempPath = string.Format(@"{0}{1}", Path.GetTempPath(), GetFileName(_downloadURL));

            _webClient.DownloadProgressChanged += OnDownloadProgressChanged;

            _webClient.DownloadFileCompleted += OnDownloadComplete;

            _webClient.DownloadFileAsync(uri, _tempPath);

            // Start the stopwatch which we will be using to calculate the download speed
            sw.Start();
        }

        /// <summary>
        /// Lors du changement de progression du téléchargement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double TimeRemaining = (e.TotalBytesToReceive - e.BytesReceived) * sw.Elapsed.TotalSeconds / e.BytesReceived;
            if (TimeRemaining < 60)
            {
                this.Text = string.Format("[{0}%] - Il reste {1} seconde(s)", e.ProgressPercentage, TimeRemaining.ToString("0"));
            }
            if (TimeRemaining > 60)
            {
                this.Text = string.Format("[{0}%] - Il reste {1} minute(s)", e.ProgressPercentage, TimeRemaining.ToString("0"));
            }
            this.lblInfos.Text = string.Format("{0} /{1} ({2}Ko/s)", FormatBytes(e.BytesReceived, 1, true), FormatBytes(e.TotalBytesToReceive, 1, true), (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));
            pbDownload.Value = e.ProgressPercentage;
            TaskbarManager.Instance.SetProgressValue(e.ProgressPercentage, 100);
        }

        /// <summary>
        /// Formats du nombre d'octects au plus proche type octects
        /// </summary>
        /// <param name="bytes">La quantité d'octets</param>
        /// <param name="decimalPlaces">Combien de décimales à afficher</param>
        /// <param name="showByteType">Ajouter le type d'octet sur l'extrémité de la chaîne</param>
        /// <returns>Les octets formatées selon les prescriptions</returns>
        private string FormatBytes(long bytes, int decimalPlaces, bool showByteType)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType = "o";

            // Vérifie si la meilleur taille est en KB
            if (newBytes > 1024 && newBytes < 1048576)
            {
                newBytes /= 1024;
                byteType = "Ko";
            }
            else if (newBytes > 1048576 && newBytes < 1073741824)
            {
                // Vérifie si la meilleur taille est en MB
                newBytes /= 1048576;
                byteType = "Mo";
            }
            else
            {
                // Meilleur taille en GB
                newBytes /= 1073741824;
                byteType = "Go";
            }

            // Montre les décimales
            if (decimalPlaces > 0)
                formatString += ":0.";

            // Ajoute les décimales
            for (int i = 0; i < decimalPlaces; i++)
                formatString += "0";

            // Ferme l'espace réservé
            formatString += "}";

            // Ajouter un type d'octets
            if (showByteType)
                formatString += byteType;

            return String.Format(formatString, newBytes);
        }

        /// <summary>
        /// Quand le téléchargement est terminé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                try
                {
                    var processStartInfo = new ProcessStartInfo { FileName = _tempPath, UseShellExecute = true };
                    if (Path.GetExtension(_tempPath) != ".msi")
                    {
                        processStartInfo.WorkingDirectory = Environment.CurrentDirectory;
                        processStartInfo.Verb = "runas";
                    }
                    Process.Start(processStartInfo);
                    if (EPUpdate.IsWinFormsApplication)
                    {
                        Application.Exit();
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, EPUpdate.DialogTitle);
                }
            }
        }

        /// <summary>
        /// Obtient le nom de fichier sur le serveur
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetFileName(string url)
        {
            var fileName = string.Empty;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            httpWebRequest.Method = "HEAD";
            httpWebRequest.AllowAutoRedirect = false;
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (httpWebResponse.StatusCode.Equals(HttpStatusCode.Redirect) || httpWebResponse.StatusCode.Equals(HttpStatusCode.Moved) || httpWebResponse.StatusCode.Equals(HttpStatusCode.MovedPermanently))
            {
                if (httpWebResponse.Headers["Location"] != null)
                {
                    var location = httpWebResponse.Headers["Location"];
                    fileName = GetFileName(location);
                    return fileName;
                }
            }
            if (httpWebResponse.Headers["content-disposition"] != null)
            {
                var contentDisposition = httpWebResponse.Headers["content-disposition"];
                if (!string.IsNullOrEmpty(contentDisposition))
                {
                    const string lookForFileName = "filename=";
                    var index = contentDisposition.IndexOf(lookForFileName, StringComparison.CurrentCultureIgnoreCase);
                    if (index >= 0)
                        fileName = contentDisposition.Substring(index + lookForFileName.Length);
                    if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    {
                        fileName = fileName.Substring(1, fileName.Length - 2);
                    }
                }
            }
            if (string.IsNullOrEmpty(fileName))
            {
                var uri = new Uri(url);

                fileName = Path.GetFileName(uri.LocalPath);
            }
            return fileName;
        }

        /// <summary>
        /// Lors de la fermeture de la fenêtre de téléchargement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _webClient.CancelAsync(); //Annule le téléchargement
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
        }
    }
}
