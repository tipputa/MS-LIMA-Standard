using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Metabolomics.MsLima.Bean;
using Metabolomics.MsLima.Model;
using Metabolomics.Core.Handler;
using Metabolomics.Core;
using Microsoft.Win32;

namespace Metabolomics.MsLima {

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        public MainWindowVM MainWindowVM;
        public ControlRefresh ControlRefresh;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            this.MainWindowVM = new MainWindowVM();
            this.ControlRefresh = MainWindowVM.ControlRefresh;
            this.DataContext = this.MainWindowVM;


        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        #region MenuItems
        private void MenuItem_Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "MSP file(*.msp)|*.msp; Text file (*.txt)|*.txt; all files(*)|*;",
                Title = "Import a library file",
                RestoreDirectory = true,
                Multiselect = false
            };

            if (ofd.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.MainWindowVM.MsLimaData.DataStorage.SetLibrary(ofd.FileName);
                //var dt = DateTime.Now;
                //var mspFileNewPath = System.IO.Path.GetDirectoryName(mspFilePath) + "\\" + System.IO.Path.GetFileNameWithoutExtension(mspFilePath) + "_StartMod_"
                //    + dt.ToString("yy_MM_dd_HH_mm_ss") + ".msp";
                //System.IO.File.Copy(mspFilePath, mspFileNewPath, true);
                //var mspQueriesOld = MspFileParcer.MspFileReaderMakeCompoundList(mspFileNewPath);
                //var mspQueries = MspFileParcer.GetCompList(mspQueriesOld);
                //this.mainWindowVM.Data = new DataStorageBean(mspFileNewPath, mspQueries);
                //this.mainWindowVM.Refresh_ImportRawData(mspQueries);
                //this.Title = this.MainWindowTitle + " " + mspFileNewPath;
                Mouse.OverrideCursor = null;
            }
        }

        #region Export
        private void MenuItem_Export_withoutRT_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MenuItem_ExportAsCsv_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
        #endregion

        #region SelectionChanged
        private void DataGrid_CompoundTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MainWindowVM.SelectedCompoundBean == null) return;
            SpectraTableRefresh();
        }

        private void DataGrid_Spectra_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MainWindowVM.SelectedSpectrum == null) return;
            MassSpectrumViewRefresh();
        }

        private void DataGrid_SingleMassSpectrumTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ControlRefresh.SelectedPeakChanged(MainWindowVM.TabMassSpectraView);
        }

        private void DataGrid_Consensus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ControlRefresh.SelectedConsensusPeakChanged(MainWindowVM.TabMassSpectraView);
        }

        private void TabControl_MS2view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("tab view" + this.TabControl_MS2view.SelectedIndex);

            if (this.TabControl_MS2view.SelectedIndex == 0)
            {
                MainWindowVM.TabMassSpectraView = TabMassSpectraView.SingleMS;
            }
            else if (this.TabControl_MS2view.SelectedIndex == 1)
            {
                MainWindowVM.TabMassSpectraView = TabMassSpectraView.MultipleMS;
            }
            else if (this.TabControl_MS2view.SelectedIndex == 2)
            {
                MainWindowVM.TabMassSpectraView = TabMassSpectraView.ConsensusMS;
            }
            MassSpectrumViewRefresh();
        }

        private void Tab_MS_Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine(e.OriginalSource);
            Console.WriteLine("tab table" + this.Tab_MS_Table.SelectedIndex);
            if (e.OriginalSource is TabControl)
            {
                if (this.Tab_MS_Table.SelectedIndex == 0)
                {
                    MainWindowVM.TabMassSpectrumTable = TabMassSpectrumTable.SinglePeak;
                }
                else if (this.Tab_MS_Table.SelectedIndex == 1)
                {
                    MainWindowVM.TabMassSpectrumTable = TabMassSpectrumTable.Consensus;
                }
            }
        }
        #endregion

        #region Refresh

        private void SpectraTableRefresh()
        {
            MainWindowVM.SelectedSpectrum = MainWindowVM.SelectedCompoundBean.Spectra[0];
            MassSpectrumViewRefresh();
        }

        private void MassSpectrumViewRefresh()
        { 
        }



        #endregion


    }
}
