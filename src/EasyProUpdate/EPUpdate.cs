using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace EasyProUpdate
{
    public class EPUpdate
    {
        public static String DialogTitle;

        internal static String ChangeLog;

        internal static String DownloadURL;

        internal static Version UpdateVersion;

        internal static Version InstalledVersion;

        internal static bool IsWinFormsApplication;

        /// <summary>
        /// URL du fichier XML qui contient les informations sur la dernière version de l'application.
        /// </summary>
        public static String AppCastURL;

        /// <summary>
        /// Ouvre l'url de téléchargement dans le navigateur par défaut si vrai. Très utile si vous avez une application portable.
        /// </summary>
        public static bool OpenDownloadPage;

        /// <summary>
        /// Définit la culture actuelle de la fenêtre de notification de mise à jour automatique. Définissez cette valeur si vos supports d'application
        /// Fonctionnelle pour changer le language de la demande.
        /// </summary>
        public static CultureInfo CurrentCulture;

        /// <summary>
        /// Un type délégué pour l'accrochage des notifications de mise à jour.
        /// </summary>
        /// <param name="args">Un objet contenant tous les paramètres recus du fichier XML AppCast. Si il y aura une erreur en cherchant le fichier XML alors cet objet sera nulle.</param>
        public delegate void CheckForUpdateEventHandler(UpdateInfoEventArgs args);

        /// <summary>
        /// Un événement que les clients peuvent utiliser pour être notifié quand la mise à jour est cochée.
        /// </summary>
        public static event CheckForUpdateEventHandler CheckForUpdateEvent;

        /// <summary>
        /// Commence à vérifier pour la nouvelle version de l'application et l dialogue d'affichage pour l'utilisateur si une mise à jour est disponible.
        /// </summary>
        public static void Start()
        {
            Start(AppCastURL);
        }

        /// <summary>
        /// Commence à vérifier pour la nouvelle version de l'application et le dialogue d'affichage pour l'utilisateur si une mise à jour est disponible.
        /// </summary>
        /// <param name="appCast">URL du fichier XML qui contient des informations sur la dernière version de l'application.</param>
        public static void Start(String appCast)
        {
            AppCastURL = appCast;

            IsWinFormsApplication = Application.MessageLoop;

            var backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += BackgroundWorkerDoWork;

            backgroundWorker.RunWorkerAsync();
        }

        private static void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            Assembly mainAssembly = Assembly.GetEntryAssembly();

            InstalledVersion = mainAssembly.GetName().Version;

            WebRequest webRequest = WebRequest.Create(AppCastURL);
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            WebResponse webResponse;

            try
            {
                webResponse = webRequest.GetResponse();
            }
            catch (Exception)
            {
                if (CheckForUpdateEvent != null)
                {
                    CheckForUpdateEvent(null);
                }
                return;
            }

            Stream appCastStream = webResponse.GetResponseStream();

            var receivedAppCastDocument = new XmlDocument();

            if (appCastStream != null)
            {
                receivedAppCastDocument.Load(appCastStream);
            }
            else
            {
                if (CheckForUpdateEvent != null)
                {
                    CheckForUpdateEvent(null);
                }
                return;
            }

            XmlNodeList appCastItems = receivedAppCastDocument.SelectNodes("item");

            if (appCastItems != null)
                foreach (XmlNode item in appCastItems)
                {
                    XmlNode appCastVersion = item.SelectSingleNode("version");
                    if (appCastVersion != null)
                    {
                        String appVersion = appCastVersion.InnerText;
                        UpdateVersion = new Version(appVersion);
                    }
                    else
                        continue;

                    XmlNode appCastTitle = item.SelectSingleNode("title");

                    DialogTitle = appCastTitle != null ? appCastTitle.InnerText : "";

                    XmlNode appCastChangeLog = item.SelectSingleNode("changelog");

                    ChangeLog = appCastChangeLog != null ? appCastChangeLog.InnerText : "";

                    XmlNode appCastUrl = item.SelectSingleNode("url");

                    DownloadURL = appCastUrl != null ? appCastUrl.InnerText : "";

                    if (IntPtr.Size.Equals(8))
                    {
                        XmlNode appCastUrl64 = item.SelectSingleNode("url64");

                        var downloadURL64 = appCastUrl64 != null ? appCastUrl64.InnerText : "";

                        if (!string.IsNullOrEmpty(downloadURL64))
                        {
                            DownloadURL = downloadURL64;
                        }
                    }
                }

            if (UpdateVersion == null)
                return;

            var args = new UpdateInfoEventArgs
            {
                DownloadURL = DownloadURL,
                Changelog = ChangeLog,
                CurrentVersion = UpdateVersion,
                InstalledVersion = InstalledVersion,
                IsUpdateAvailable = false,
            };

            if (UpdateVersion > InstalledVersion)
            {
                args.IsUpdateAvailable = true;
                if (CheckForUpdateEvent == null)
                {
                    var thread = new Thread(ShowUI);
                    thread.CurrentCulture = thread.CurrentUICulture = CurrentCulture ?? Application.CurrentCulture;
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
            }

            if (CheckForUpdateEvent != null)
            {
                CheckForUpdateEvent(args);
            }
        }

        /// <summary>
        /// Affiche la fenêtre de demande update
        /// </summary>
        private static void ShowUI()
        {
            var updateForm = new UpdateForm();

            updateForm.ShowDialog();
        }

        private static Attribute GetAttribute(Assembly assembly, Type attributeType)
        {
            object[] attributes = assembly.GetCustomAttributes(attributeType, false);
            if (attributes.Length == 0)
            {
                return null;
            }
            return (Attribute)attributes[0];
        }

        /// <summary>
        /// Ouvre la fenêtre de téléchargement pour télécharger la mise à jour et exécute le programme d'installation une fois le téléchargement terminé.
        /// </summary>
        public static void DownloadUpdate()
        {
            var downloadDialog = new DownloadForm(DownloadURL);

            try
            {
                downloadDialog.ShowDialog();
            }
            catch (TargetInvocationException)
            {
            }
        }
    }

    /// <summary>
    /// Objet de cette classe qui vous donne tous les détails sur la mise à jour utile dans le traitement de la mise à jour de la logique même.
    /// </summary>
    public class UpdateInfoEventArgs : EventArgs
    {
        /// <summary>
        /// Si la nouvelle mise à jour est disponible, renvoie true sinon false.
        /// </summary>
        public bool IsUpdateAvailable { get; set; }

        /// <summary>
        /// URL de téléchargement du fichier de mise à jour.
        /// </summary>
        public string DownloadURL { get; set; }

        /// <summary>
        /// URL de la page Web indiquant des changements dans la nouvelle mise à jour.
        /// </summary>
        public string Changelog { get; set; }

        /// <summary>
        /// Retourne la nouvelle version de l'application disponible pour télécharger.
        /// </summary>
        public Version CurrentVersion { get; set; }

        /// <summary>
        /// Retourne la version de l'application installée sur le PC de l'utilisateur.
        /// </summary>
        public Version InstalledVersion { get; set; }
    }
}
