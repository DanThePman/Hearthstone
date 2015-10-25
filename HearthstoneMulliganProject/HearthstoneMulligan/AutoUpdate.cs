using System.Diagnostics;
using System.Net;
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

            if (currentVersion != downloadStringCompletedEventArgs.Result)
            {
                //MainWindow window =  new MainWindow();
                //window.ShowDialog();
                DllDownloader.form = new UpdatingWindow();
                DllDownloader.form.ShowDialog();
            }
        }
    }
}
