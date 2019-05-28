using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Metabolomics.Core;
using Metabolomics.MsLima.Bean;
using ChartDrawing;
namespace Metabolomics.MsLima.Model
{
    public static class WindowUtility
    {
        public async static void StartUpInitializingWindow()
        {
            var window = new ShortMessage();
            window.Title = "Notice";
            window.LabelMessage.Text = "Initializing...";
            window.Owner = App.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
            await Test();
            window.Close();
        }

        public static void StartUpParameterSettingWindow(MsLimaData data)
        {
            var window = new ParameterSettingWindow(data);
            window.Owner = App.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
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

        public static void CheckCompoundGroup(List<CompoundBean> compounds, float minRtDiff)
        {
            var rtString = "";
            var formulaString = "";
            var InChIKeyString = "";
            CompoundGroupUtility.CheckCompoundList(compounds, minRtDiff, ref rtString, ref formulaString, ref InChIKeyString);
            StartUpErrorMessageWindow("RT differece", rtString);
            StartUpErrorMessageWindow("Formula difference", formulaString);
            StartUpErrorMessageWindow("InChIKey differece", InChIKeyString);
        }

        public static void StartUpErrorMessageWindow(string title, string content)
        {
            if (string.IsNullOrEmpty(content)) return;
            Utility.CheckStringSize(content, 15, out var width, out var height);
            var window = new ShortMessage();
            window.Width = width;
            window.Height = height + 40;
            window.Title = title;
            window.LabelMessage.Text = content;
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
