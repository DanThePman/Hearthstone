using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;

namespace HearthstoneMulligan.USER_GUI
{
    /// <summary>
    /// Size of Config file for proc bar throwed/throwable
    /// </summary>
    class DllDownloader
    {
        /*Main download currentForm kept here to have access over the GUI thread*/
        public UpdatingWindow currentForm;
        private static bool isCoreUpdate;
        public void DownloadLatestDLL(string url, string hardDiskPathToSafe, bool _isCoreUpdate = true)
        {
            if (currentForm == null)
            {
                MessageBox.Show("Current Form is NULL", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dllDownloadCompleted = false;
            configFileDownloadCompleted = !_isCoreUpdate;//init with true if no core update
            isCoreUpdate = _isCoreUpdate;

            WebClient wbClient = new WebClient();

            wbClient.DownloadFileCompleted += DllDownloadCompleted;
            wbClient.DownloadProgressChanged += DllDownloadProgressChanged;
            wbClient.DownloadFileTaskAsync(url, hardDiskPathToSafe);

            if (isCoreUpdate)
                DownloadConfigFile();
        }

        private void DllDownloadProgressChanged(object sender,
            DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            float x = downloadProgressChangedEventArgs.BytesReceived /
                downloadProgressChangedEventArgs.TotalBytesToReceive;

            float percent = x * 100;

            currentForm.metroProgressBar1.Value = (int)percent;
        }

        private bool dllDownloadCompleted = false;
        private bool configFileDownloadCompleted = false;
        private void DllDownloadCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            dllDownloadCompleted = true;

            if (configFileDownloadCompleted)
            {
                if (isCoreUpdate)
                    OnUpdateFinished();
                else
                {
                    currentForm.Close();
                }
            }
        }

        private void DownloadConfigFile()
        {
            WebClient wbClient2 = new WebClient();
            wbClient2.DownloadFileCompleted += (sender, args) =>
            {
                configFileDownloadCompleted = true;

                if (dllDownloadCompleted)
                {
                    if (isCoreUpdate)
                        OnUpdateFinished();
                    else
                    {
                        currentForm.Close();
                    }
                }
            };

            string dllDownloadUrlOfConfigFile = "https://raw.githubusercontent.com/DanThePman/Hearthstone/master/Download/PlaceContentInThe_MulliganProfiles_Folder/MulliganCore.config";
            wbClient2.DownloadFileTaskAsync(dllDownloadUrlOfConfigFile, Environment.CurrentDirectory 
                + @"\MulliganProfiles\MulliganCore.config");
        }

        private void OnUpdateFinished()
        {
            currentForm.metroProgressBar1.Value = currentForm.metroProgressBar1.Maximum;

            Process process = new Process
            {
                StartInfo =
                    {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = Environment.CurrentDirectory + @"\ReplaceUpdate.exe"
                    }
            };
            process.Start();
        }
    }
}
