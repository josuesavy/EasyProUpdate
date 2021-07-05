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
using System.IO.Compression;

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
            int intHours = 0;
            int intMinutes = 0;
            int intSeconds = 0;
            int EstimatedTime = 0;
            int ByteProcessTime = 0;

            ByteProcessTime = Convert.ToInt32(e.BytesReceived / sw.Elapsed.TotalSeconds);
            EstimatedTime = Convert.ToInt32((e.TotalBytesToReceive - e.BytesReceived) / ByteProcessTime);

            if (EstimatedTime > 60)
            {
                intMinutes = Convert.ToInt32(EstimatedTime / 60);
                intSeconds = Convert.ToInt32(EstimatedTime % 60);

                if (intMinutes > 60)
                {
                    intHours = Convert.ToInt32(intMinutes / 60);
                    intMinutes = Convert.ToInt32(intMinutes % 60);
                }
            }
            else
                intSeconds = EstimatedTime;

            string TimeRemaining = string.Format("Il reste {0:#0} heure(s) {1:#0} min(s)", intHours, intMinutes);

            if (intHours == 0)
                TimeRemaining = string.Format("Il reste {0:#0} min(s)", intMinutes);

            if (intMinutes == 0)
                TimeRemaining = string.Format("Il reste {0:#0} heure(s)", intHours);

            if (intSeconds == 0)
                TimeRemaining = string.Format("Il reste {0:#0} heure(s) {1:#0} min(s)", intHours, intMinutes);

            if (intHours == 0 & intMinutes == 0)
                TimeRemaining = string.Format("Il reste {0:#0} sec(s)", intSeconds);

            if (intHours == 0 & intSeconds == 0)
                TimeRemaining = string.Format("Il reste {0:#0} min(s)", intMinutes);

            if (intMinutes == 0 & intSeconds == 0)
                TimeRemaining = string.Format("Il reste {0:#0} heure(s)", intHours);

            if (intHours == 0 & intMinutes == 0 & intSeconds == 0)
                TimeRemaining = "Téléchargement terminé";

            this.Text = string.Format("[{0}%] - {1}", e.ProgressPercentage, TimeRemaining);
            string speedDownloading = (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00");
            lblInfos.Text = string.Format("{0}/{1} ({2}Ko/s)", FormatBytes(e.BytesReceived, 1, true), FormatBytes(e.TotalBytesToReceive, 1, true), speedDownloading);

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

            // Vérifie si la meilleur taille est en Ko
            if (newBytes > 1024 && newBytes < 1048576)
            {
                newBytes /= 1024;
                byteType = "Ko";
            }
            else if (newBytes > 1048576 && newBytes < 1073741824)
            {
                // Vérifie si la meilleur taille est en Mo
                newBytes /= 1048576;
                byteType = "Mo";
            }
            else
            {
                // Meilleur taille en Go
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

                    string fileExtract = Path.GetExtension(_tempPath);
                    string fileMsi = Path.GetExtension(_tempPath);
                    string fileExe = Path.GetExtension(_tempPath);

                    if (fileExtract == ".zip" || fileExtract == ".gz" || fileExtract == ".rar" || fileExtract == ".tar")
                    {
                        using (ZipArchive archive = ZipFile.OpenRead(_tempPath))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if (entry.FullName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                                {
                                    string path = Path.Combine(Path.GetTempPath(), entry.FullName);
                                    entry.ExtractToFile(path);
                                    processStartInfo.WorkingDirectory = Environment.CurrentDirectory;
                                    processStartInfo.Verb = "runas";
                                    Process.Start(path);
                                }
                                else if (entry.FullName.EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                                {
                                    string path = Path.Combine(Path.GetTempPath(), entry.FullName);
                                    entry.ExtractToFile(path);
                                    Process.Start(path);
                                }
                            }
                        }
                    }
                    else if (fileMsi == ".msi")
                        Process.Start(processStartInfo);

                    else if (fileExe == ".exe")
                    {
                        processStartInfo.WorkingDirectory = Environment.CurrentDirectory;
                        processStartInfo.Verb = "runas";
                        Process.Start(processStartInfo);
                    }

                    if (EPUpdate.IsWinFormsApplication)
                        Application.Exit();
                    else
                        Environment.Exit(0);

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
                        fileName = fileName.Substring(1, fileName.Length - 2);
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
