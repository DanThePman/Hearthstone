using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using HearthstoneMulligan.USER_GUI;

namespace HearthstoneMulligan
{
    class AutoUpdate
    {
        public static void CheckUpdate()
        {
            WebClient wbClient = new WebClient();
            wbClient.DownloadStringCompleted += WbClientOnDownloadVersionCompleted;
            string assemblyVersionUrl = "https://raw.githubusercontent.com/DanThePman/Hearthstone/master/assemblyVersion.txt";
            wbClient.DownloadStringTaskAsync(assemblyVersionUrl);
        }

        private static void WbClientOnDownloadVersionCompleted(object sender, 
            DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string currentVersion = fvi.ProductVersion;
            string latestVersion = downloadStringCompletedEventArgs.Result;
            latestVersion = latestVersion.Replace("\n", "");

            if (currentVersion != latestVersion)
            {
                //MainWindow window =  new MainWindow();
                //window.ShowDialog();
                DllDownloader.form = new UpdatingWindow();
                DllDownloader.form.ShowDialog();
            }
        }
    }
}
