using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using HearthstoneMulligan.USER_GUI;

namespace HearthstoneMulligan
{
    class AutoUpdateInit
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
                new UpdatingWindow(true).ShowDialog();
            }
            else
            if (!System.IO.File.Exists(Environment.CurrentDirectory + @"\De.TorstenMandelkow.MetroChart.dll"))
            {
                DllDownloader dllDownloader = new DllDownloader
                {
                    currentForm = new UpdatingWindow(false) { Text = "Updating external dlls..." }
                };
                dllDownloader.DownloadLatestDLL("https://github.com/DanThePman/Hearthstone/raw/master/Download/PlaceContentInThe_SmartBot_Folder/De.TorstenMandelkow.MetroChart.dll",
                    Environment.CurrentDirectory + @"\De.TorstenMandelkow.MetroChart.dll", false);
                dllDownloader.currentForm.ShowDialog();

                MessageBox.Show("De.TorstenMandelkow.MetroChart.dll downloading. Please restart your SmartBot if" +
                                " you planned to use the coach mode", "Information - External dll update",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}
