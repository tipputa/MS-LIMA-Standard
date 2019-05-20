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
        public TabMassSpectraView TabMassSpectraView { get; set; } = TabMassSpectraView.SingleMS;
        public TabMassSpectrumTable TabMassSpectrumTable { get; set; } = TabMassSpectrumTable.SinglePeak;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            this.MainWindowVM = new MainWindowVM();
            this.ControlRefresh = new ControlRefresh(MainWindowVM);
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
            MainWindowMetaInformationCompoundRefresh();
            SpectraTableRefresh();
        }

        private void DataGrid_Spectra_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MainWindowVM.SelectedSpectrum == null) return;
            MassSpectrumTableRefresh();
            MassSpectrumViewRefresh();
            MainWindowMetaInformationSpectraRefresh();
        }

        private void DataGrid_SingleMassSpectrumTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ControlRefresh.SelectedPeakChanged(TabMassSpectraView);
        }

        private void DataGrid_Consensus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void TabControl_MS2view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("tab view" + this.TabControl_MS2view.SelectedIndex);

            if (this.TabControl_MS2view.SelectedIndex == 0)
            {
                TabMassSpectraView = TabMassSpectraView.SingleMS;
            }
            else if (this.TabControl_MS2view.SelectedIndex == 1)
            {
                TabMassSpectraView = TabMassSpectraView.MultipleMS;
            }
            else if (this.TabControl_MS2view.SelectedIndex == 2)
            {
                TabMassSpectraView = TabMassSpectraView.ConsensusMS;
            }
            MassSpectrumViewRefresh();
        }

        private void Tab_MS_Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("tab table" + this.Tab_MS_Table.SelectedIndex);
            if (this.Tab_MS_Table.SelectedIndex == 0)
            {
                TabMassSpectrumTable = TabMassSpectrumTable.SinglePeak;
            }
            else if (this.Tab_MS_Table.SelectedIndex == 1)
            {
                TabMassSpectrumTable = TabMassSpectrumTable.Consensus;
            }
            MassSpectrumTableRefresh();
        }
        #endregion

        #region Refresh
        private void CompoundTableRefresh()
        {
            MainWindowVM.Refresh_ImportRawData();
            DataGrid_CompoundTable.ItemsSource = MainWindowVM.FilteredCompoundTable.View;
            MainWindowVM.SelectedCompoundBean = MainWindowVM.CompoundTable[0];
            MainWindowMetaInformationRefresh();
            MainWindowMetaInformationCompoundRefresh();
            SpectraTableRefresh();
        }

        private void SpectraTableRefresh()
        {
            DataGrid_Spectra.ItemsSource = MainWindowVM.SelectedCompoundBean.Spectra;
            MainWindowVM.SelectedSpectrum = MainWindowVM.SelectedCompoundBean.Spectra[0];
            MassSpectrumTableRefresh();
            MassSpectrumViewRefresh();
            MainWindowMetaInformationSpectraRefresh();
        }

        private void MassSpectrumTableRefresh()
        {
            if (MainWindowVM.SelectedSpectrum == null) return;
            if (TabMassSpectrumTable == TabMassSpectrumTable.SinglePeak)
            {
                SingleSpectrumTableRefresh();
            }
            else if(TabMassSpectrumTable == TabMassSpectrumTable.Consensus)
            {
                ConsensusSpectrumTableRefresh();
            }
        }

        private void MassSpectrumViewRefresh()
        {
            if (MainWindowVM.SelectedSpectrum == null) return;
            if (TabMassSpectraView == TabMassSpectraView.SingleMS)
            {
                ControlRefresh.SingleSpectrumViewRefresh();

            }
            else if(TabMassSpectraView == TabMassSpectraView.MultipleMS)
            {
                MultipleSpectraViewRefresh();
            }
            else if(TabMassSpectraView == TabMassSpectraView.ConsensusMS)
            {
                ConsensusSpectrumTableRefresh();
                ControlRefresh.ConsensusSpectrumViewRefresh();
            }
        }



        private void SingleSpectrumTableRefresh()
        {
            DataGrid_SingleMassSpectrumTable.ItemsSource = MainWindowVM.SelectedSpectrum.Spectrum;
        }

        private void ConsensusSpectrumTableRefresh()
        {
            MainWindowVM.ConsensusSpectraTable = MsGrouping.Excute(MainWindowVM.SelectedCompoundBean);
        }


        private void MultipleSpectraViewRefresh()
        {
            MainWindowVM.MultipleSpectra = ControlRefresh.MultipleSpectraRefresh();
        }


        private void MainWindowMetaInformationRefresh()
        {
            Label_NumComp.Text = "Number of compounds: " + MainWindowVM.CompoundTable.Count;
            Label_NumSpectra.Text = "Number of spectra: " + MainWindowVM.MsLimaData.DataStorage.RawLibraryFile.Count;
        }
        private void MainWindowMetaInformationCompoundRefresh()
        {
            Label_SelectedComp.Text = "Selected sompound: " + MainWindowVM.SelectedCompoundBean.Name;
        }

        private void MainWindowMetaInformationSpectraRefresh()
        {
            if (MainWindowVM.SelectedSpectrum == null) return;
            Label_SelectedSpectra.Text = "Spectra ID: " + MainWindowVM.SelectedSpectrum.Id + ", " + MainWindowVM.SelectedSpectrum.AdductIon.AdductIonName + ", " + MainWindowVM.SelectedSpectrum.CollisionEnergy + "eV";
        }


        #endregion


    }
}
