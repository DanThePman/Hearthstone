using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HearthstoneMulligan.USER_GUI
{
    class DllDownloader
    {
        public static UpdatingWindow form;
        public void DownloadLatestDLL()
        {
            WebClient wbClient = new WebClient();

            string dllDownloadUrl = "https://github.com/DanThePman/Hearthstone/raw/master/HearthstoneMulligan.dll";

            wbClient.DownloadFileCompleted += DllDownloadCompleted;
            wbClient.DownloadProgressChanged += DllDownloadProgressChanged;
            wbClient.DownloadFileTaskAsync(dllDownloadUrl, Environment.CurrentDirectory +
                                                            @"\HearthstoneMulliganNew.dll");
        }

        private void DllDownloadProgressChanged(object sender,
            DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            float x = downloadProgressChangedEventArgs.BytesReceived /
                downloadProgressChangedEventArgs.TotalBytesToReceive;

            float percent = x * 100;

            form.metroProgressBar1.Value = (int)percent;
        }

        private void DllDownloadCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            form.metroProgressBar1.Value = form.metroProgressBar1.Maximum;

            Process process = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = Environment.CurrentDirectory + @"\replace.bat"
                }
            };
            process.Start();
        }
    }
}
