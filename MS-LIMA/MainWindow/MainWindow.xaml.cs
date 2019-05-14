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

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            this.MainWindowVM = new MainWindowVM();
            
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
                CompoundTableRefresh();
            }

        }
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
            MassSpectrumTableRefresh();

        }

        private void DataGrid_SingleMassSpectrumTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGrid_Consensus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TabControl_MS2view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion

        #region Refresh
        private void CompoundTableRefresh()
        {
            MainWindowVM.Refresh_ImportRawData();
            this.DataGrid_CompoundTable.ItemsSource = MainWindowVM.CompoundTable;
            MainWindowVM.SelectedCompoundBean = MainWindowVM.CompoundTable[0];
            SpectraTableRefresh();
        }

        private void SpectraTableRefresh()
        {
            this.DataGrid_Spectra.ItemsSource = MainWindowVM.SelectedCompoundBean.Spectra;
            MainWindowVM.SelectedSpectrum = MainWindowVM.SelectedCompoundBean.Spectra[0];

            MassSpectrumTableRefresh();
        }

        private void MassSpectrumTableRefresh()
        {
            if (this.MainWindowVM.SelectedSpectrum == null) return;
            if (this.TabControl_MS2view.SelectedIndex == 0)
            {
                this.SelectedSpectrumUI.Content = new ChartDrawing.MassSpectrumUI(MassSpectrumViewHandler.GetMassSpectrumDrawVisual(MainWindowVM.SelectedSpectrum));
            }
            if (this.Tab_MS_Table.SelectedIndex == 0)
            {
                SingleSpectrumTableRefresh();
            }
            else
            {
                ConsensusSpectrumTableRefresh();
            }
        }



        private void SingleSpectrumTableRefresh()
        {
            this.DataGrid_SingleMassSpectrumTable.ItemsSource = MainWindowVM.SelectedSpectrum.Spectrum;
        }

        private void ConsensusSpectrumTableRefresh()
        {

        }
        #endregion

    }
}
