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
using Microsoft.Win32;

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

        public static void UpdateMetaData(List<CompoundBean> compounds)
        {
            var s = "Please use following file format (with header).\r\n";
            s += "InChIKey\tInChI\tSMILES\r\n";
            s += "AAAAAAAA-AAAAAAAAAAAAAA-A\tInChI=/AAA/AAAA\tCNCO\r\n";
            s += "BBBBBBBB-BBBBBBBBBBBBBB-B\tInChI=/BBB/BBBB\tCCCCCC\r\n";

            if (MessageBox.Show(s, "Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Text file (*.txt)|*.txt| all files(*)|*";
                ofd.Title = "Import a reference text file (InChIKey, SMILES, InChI)";
                ofd.RestoreDirectory = true;
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == true)
                {

                    var tmpList = Reader.ReadTemporalFile.GetTextBasedInChIKeyLibraryFile(ofd.FileName, out List<string> error1);
                    if (error1.Count > 0)
                    {
                        var str = "The file format is not corrects.";
                        var counter = 0;
                        foreach (var f in error1)
                        {
                            str += f + "\r\n";
                            counter++;
                            if(counter > 10)
                            {
                                str += "There are " + (error1.Count - counter) + "additional errors.";
                                break;
                            }
                        }

                        MessageBox.Show(str, "File format error", MessageBoxButton.OK);
                        return;
                    }
                    CompoundGroupUtility.UpdateMetaData(compounds, tmpList, out List<string> errorMessage);
                    if (errorMessage.Count > 0)
                    {
                        var str = "Several InChIKey are not correct.";
                        var counter = 0;
                        foreach (var f in error1)
                        {
                            str += f + "\r\n";
                            counter++;
                            if (counter > 10)
                            {
                                str += "There are " + (error1.Count - counter) + "additional errors.";
                                break;
                            }
                        }
                        MessageBox.Show(str, "File information is not enough.", MessageBoxButton.OK);

                    }
                    else
                    {
                        MessageBox.Show("SMILES and InChI were updated.", "Notice", MessageBoxButton.OK);
                    }
                }
            }
        }

        public static void UpdateCommonMetaData(List<CompoundBean> compounds)
        {
            var s = "If you want to overwrite common meta data, \r\nplease use following file format (like msp format).\r\n\r\n";
            s += "INSTRUMENT: Your setting\r\n";
            s += "INSTRUMENTTYPE: Your setting\r\n";
            s += "SPECTRUMTYPE: Your setting\r\n";
            s += "MSLEVEL: Your setting\r\n";
            s += "LICENSE: Your setting\r\n";
            s += "AUTHORS: Your setting\r\n";
            s += "COMMENT: Your setting\r\n";

            if (MessageBox.Show(s, "Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Text file (*.txt)|*.txt| all files(*)|*";
                ofd.Title = "Import a reference file";
                ofd.RestoreDirectory = true;
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == true)
                {
                    var common = Reader.ReadTemporalFile.GetCommonMetaDataFromFile(ofd.FileName);
                    foreach (var c in compounds)
                    {
                        foreach (var spec in c.Spectra)
                        {
                            if (!string.IsNullOrEmpty(common.Authors))
                            {
                                spec.Authors = common.Authors;
                            }
                            if (!string.IsNullOrEmpty(common.Comment))
                            {
                                spec.Comment = common.Comment;
                            }
                            if (!string.IsNullOrEmpty(common.Instrument))
                            {
                                spec.Instrument = common.Instrument;
                            }
                            if (!string.IsNullOrEmpty(common.InstrumentType))
                            {
                                spec.InstrumentType = common.InstrumentType;
                            }
                            if (!string.IsNullOrEmpty(common.License))
                            {
                                spec.License = common.License;
                            }
                            if (!string.IsNullOrEmpty(common.MsLevel))
                            {
                                spec.MsLevel = common.MsLevel;
                            }
                            if (!string.IsNullOrEmpty(common.SpectrumType))
                            {
                                spec.SpectrumType = common.SpectrumType;
                            }
                        }
                    }
                    MessageBox.Show("Common meta data werre updated", "Notice", MessageBoxButton.OK);
                }
            }
        }


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
