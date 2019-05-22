using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Metabolomics.MsLima.Model
{
    public static class WindowUtility
    {
        public async static void StartUpInitializingWindow()
        {
            var window = new ShortMessage();
            window.Title = "Notice";
            window.LabelMessage.Content = "Initializing...";
            window.Owner = App.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
            await Test();
            window.Close();
        }

        public static void StartUpAllSpectraTableWindow(MsLimaData data)
        {
            var window = new AllSpectraMetaInformationTable(data);
            window.Owner = App.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        public static void StartUpComparativeSpectraViewer(MsLimaData data)
        {
            var window = new ComparativeSpectrumViewer(data);
            window.Owner = App.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }



        // 
        public static async Task Test()
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Before Task");
            await Task.Run(() =>
            {
                for (var i = 0; i < 3; i++)
                {
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Loop");
                    System.Threading.Thread.Sleep(10000);
                }
            });
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: After Task");
        }
    }
}
