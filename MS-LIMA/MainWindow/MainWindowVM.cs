using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Metabolomics.Core;
using Metabolomics.MsLima.Model;
using Metabolomics.MsLima.Bean;
using System.Windows.Controls;
using ChartDrawing;

namespace Metabolomics.MsLima
{
    public class MainWindowVM : ViewModelBase
    {
        #region properties

        public MsLimaData MsLimaData { get; set; }
        public MassSpectrumViewHandler MsHandler { get; set; }
        public ControlRefresh ControlRefresh { get; set; }
        public AutoRepeater AutoExporter { get; set; }

        public List<CompoundBean> CompoundTable {
            get => MsLimaData.DataStorage.CompoundList;
        }
        public FilteredTable FilteredCompoundTable { get; set; }

        public FilterSettingsForLibrary FilteredTableSetting { get; set; }
        public TabMassSpectraView TabMassSpectraView { get; set; }
        public TabMassSpectrumTable TabMassSpectrumTable { get; set; } = TabMassSpectrumTable.SinglePeak;

        public ICollectionView FilteredComponudTableView {
            get => FilteredCompoundTable.View;
        }

        public List<MsGroup> ConsensusSpectraTable {
            get {
                if (SelectedCompoundBean == null) return null;
                return MsGrouping.Excute(SelectedCompoundBean);
            }
        }

        private bool ShouldUpdateSingleMassSpectrumVM = true;
        private bool ShouldUpdateMultipleSpectrumVM = true;
        private bool ShouldUpdateConsensusSpectrumVM = true;

        #region Selected Items
        private AnnotatedPeak selectedPeak;
        private MassSpectrum selectedSpectrum;
        private CompoundBean selectedCompoundBean;
        private MsGroup selectedMsGroup;
        public AnnotatedPeak SelectedPeak {
            get => selectedPeak;
            set => OnPropertyChangedIfSet(ref selectedPeak, value, nameof(SelectedPeak));
        }

        public MassSpectrum SelectedSpectrum {
            get => selectedSpectrum;
            set {
                selectedSpectrum = value;
                OnPropertyChanged(nameof(SelectedSpectrum));
                SelectedSpectrumChanged();
            }
        }

        public CompoundBean SelectedCompoundBean {
            get => selectedCompoundBean;
            set {
                OnPropertyChangedIfSet(ref selectedCompoundBean, value, nameof(SelectedCompoundBean));
                SelectedCompoundChanged();
            }
        }

        public MsGroup SelectedMsGroup {
            get => selectedMsGroup;
            set => OnPropertyChangedIfSet(ref selectedMsGroup, value, nameof(SelectedMsGroup));
        }
        #endregion

        #region Label
        private string mainWindowTitle = Properties.Resources.Version;
        public string MainWindowTitle {
            get => mainWindowTitle;
            set => OnPropertyChangedIfSet(ref mainWindowTitle, value, nameof(MainWindowTitle));
        }
        public string LabelNumCompounds {
            get {
                if (CompoundTable == null) return ""; return "Number of compounds: " + CompoundTable.Count;
            }
        }
        public string LabelNumSpectra {
            get {
                if (MsLimaData.DataStorage.RawLibraryFile == null) return "Please import a library"; return "Number of spectra: " + MsLimaData.DataStorage.RawLibraryFile.Count;
            }
        }
        public string LabelSelectedCompound {
            get {
                if (SelectedCompoundBean == null) return ""; return "Selected sompound: " + SelectedCompoundBean.Name;
            }
        }
        public string LabelSelectedSpectra {
            get {
                if (SelectedSpectrum == null) return ""; return "Spectra ID: " + SelectedSpectrum.Id + ", " + SelectedSpectrum.AdductIon.AdductIonName + ", " + SelectedSpectrum.CollisionEnergy + "eV";
            }
        }

        private System.Windows.Media.Imaging.BitmapImage structureImage;

        public System.Windows.Media.Imaging.BitmapImage StructureImage {
            get => structureImage;
            set => OnPropertyChangedIfSet(ref structureImage, value, nameof(StructureImage)); }
        #endregion

        #region ViewModel
        private DrawVisualMassSpectrum singleMassSpectrumVM = new DrawVisualMassSpectrum();
        public DrawVisualMassSpectrum SingleMassSpectrumVM {
            get => singleMassSpectrumVM;
            set {
                singleMassSpectrumVM = value;
                OnPropertyChanged(nameof(SingleMassSpectrumVM));
                SingleMassSpectrumUI.UpdateFE(SingleMassSpectrumVM);
            }
        }

        private DrawVisualMassSpectrum consensusSpectrumVM = new DrawVisualMassSpectrum();
        public DrawVisualMassSpectrum ConsensusSpectrumVM {
            get => consensusSpectrumVM;
            set {
                consensusSpectrumVM = value;
                OnPropertyChanged(nameof(ConsensusSpectrumVM));
                ConsensusSpectrumUI.UpdateFE(ConsensusSpectrumVM);
            }
        }
        #endregion

        #region UI
        private MassSpectrumUI singleMassSpectrumUI;
        public MassSpectrumUI SingleMassSpectrumUI {
            get => singleMassSpectrumUI;
            set => OnPropertyChangedIfSet(ref singleMassSpectrumUI, value, nameof(SingleMassSpectrumUI));
        }

        private MassSpectrumUI consensusSpectrumUI;
        public MassSpectrumUI ConsensusSpectrumUI {
            get => consensusSpectrumUI;
            set => OnPropertyChangedIfSet(ref consensusSpectrumUI, value, nameof(ConsensusSpectrumUI));
        }

        private StackPanel multipleSpectra;
        public StackPanel MultipleSpectra {
            get => multipleSpectra;
            set => OnPropertyChangedIfSet(ref multipleSpectra, value, nameof(MultipleSpectra));
        }

        #endregion

        #region Filter

        private string filterNameText;
        private string filterMzText;
        private string filterRtText;
        private string filterInChIKeyText;

        public string FilterNameText {
            get => filterNameText;
            set {
                OnPropertyChangedIfSet(ref filterNameText, value, nameof(FilterNameText));
                this.FilteredTableSetting.MetaboliteNameFilter = value;
            }
        }


        public string FilterMzText {
            get => filterMzText;
            set {
                OnPropertyChangedIfSet(ref filterMzText, value, nameof(FilterMzText));
                this.FilteredTableSetting.MzFilter = value;
            }
        }
        public string FilterRtText {
            get => filterRtText;
            set {
                OnPropertyChangedIfSet(ref filterRtText, value, nameof(FilterRtText));
                this.FilteredTableSetting.RetentionTimeFilter = value;
            }
        }

        public string FilterInChIKeyText {
            get => filterInChIKeyText;
            set {
                OnPropertyChangedIfSet(ref filterInChIKeyText, value, nameof(FilterInChIKeyText));
                this.FilteredTableSetting.InChIKeyFilter = value;
            }
        }
        #endregion


        #region Command

        #region Selection Changed Commands
        public DelegateCommand SelectionChangedSingleSpectrumTableCommand { get; set; }
        public DelegateCommand SelectionChangedConsensusTableCommand { get; set; }

        //public DelegateCommand SelectionChangedTabControlMsViewCommand { get; set; }
        public DelegateCommand SelectionChangedTabControlMsTableCommand { get; set; }


        #endregion
        #region Menu Commands
        public DelegateCommand ImportFileCommand { get; set; }
        public DelegateCommand ImportMassBankFileCommand { get; set; }

        public DelegateCommand SaveAsMspCommand { get; set; }
        public DelegateCommand SaveAsMspWithoutRTCommand { get; set; }
        public DelegateCommand SaveAsMzMineCommand { get; set; }

        public DelegateCommand WindowLoaded { get; set; }

        public DelegateCommand StartUpSettingWindow { get; set; }

        public DelegateCommand StartUpWindowAllSpectra { get; set; }

        public DelegateCommand StartUpWindowComparativeViewer { get; set; }

        public DelegateCommand ConvertAccurateMassToTheoreticalMass { get; set; }
        public DelegateCommand DropRetentionTime { get; set; }
        public DelegateCommand RemoveUnannotatedCommand { get; set; }
        public DelegateCommand SaveCommonProductIonCommand { get; set; }
        public DelegateCommand UpdateSmilesAndInChiBasedOnInChIKeyCommand { get; set; }
        public DelegateCommand UpdateCommonMetaDataCommand { get; set; }
        #endregion


        #endregion
        #endregion

        public MainWindowVM()
        {
            Task.Run(() => SmilesUtility.TryClassLoad());
            MsLimaData = new MsLimaData();
            MsHandler = new MassSpectrumViewHandler(MsLimaData.Parameter);
            AutoExporter = new AutoRepeater(MsLimaData.Parameter.WinParam.AutoExportIntervalMillisecond);
            AutoExporter.OnTimeEventHandler += (o, e) => { AutoExportFunction(); };
            SingleMassSpectrumUI = new MassSpectrumUI(SingleMassSpectrumVM);
            ConsensusSpectrumUI = new MassSpectrumUI(ConsensusSpectrumVM);

            ControlRefresh = new ControlRefresh(this);
            SetCommands();
        }

        private void MainWindowLoad()
        {
            //WindowUtility.StartUpInitializingWindow();
        }


        private void SetCommands()
        {
            WindowLoaded = new DelegateCommand(x => MainWindowLoad());

            #region MenuItems
            ImportFileCommand = new DelegateCommand(
                x =>
                {
                    ImportUtility.ImportFile(MsLimaData, AutoExporter);
                    ImportFile();
                }
            );

            ImportMassBankFileCommand = new DelegateCommand(
                x =>
                {
                    ImportUtility.ImportMassBankFile(MsLimaData, AutoExporter);
                    ImportFile();
                }
            );

            SaveAsMspCommand = new DelegateCommand(
                x => ExportUtility.SaveAsMsp(CompoundTable),
                x => !IsDataLoaded());

            SaveAsMspWithoutRTCommand = new DelegateCommand(
                x => ExportUtility.SaveAsMspWithoutRT(CompoundTable),
                x => !IsDataLoaded());

            SaveAsMzMineCommand = new DelegateCommand(
                x => ExportUtility.SaveCompoundTableAsMzMineFormat(CompoundTable),
                x => !IsDataLoaded());

            ConvertAccurateMassToTheoreticalMass = new DelegateCommand(
                x => CompoundGroupUtility.ConvertActualMassToTheoreticalMass(CompoundTable),
                x => !IsDataLoaded());

            DropRetentionTime = new DelegateCommand(
                x => CompoundGroupUtility.DropRetentionTime(CompoundTable),
                x => !IsDataLoaded());

            RemoveUnannotatedCommand = new DelegateCommand(
                x => CompoundGroupUtility.RemoveUnannotatedPeaks(CompoundTable),
                x => !IsDataLoaded());

            SaveCommonProductIonCommand = new DelegateCommand(
                x => ExportUtility.SaveCommonProductIonTable(CompoundTable),
                x => !IsDataLoaded());

            UpdateSmilesAndInChiBasedOnInChIKeyCommand = new DelegateCommand(
                x => WindowUtility.UpdateMetaData(CompoundTable),
                x => !IsDataLoaded());

            UpdateCommonMetaDataCommand = new DelegateCommand(
              x => WindowUtility.UpdateCommonMetaData(CompoundTable),
              x => !IsDataLoaded());

            #endregion


            #region SelectionChanged
            SelectionChangedSingleSpectrumTableCommand = new DelegateCommand(
                x => SingleSpectrumTableSelectionChanged()
                );

            SelectionChangedConsensusTableCommand = new DelegateCommand(
                x => ConsensusTableSelectionChanged()
                );
            SelectionChangedTabControlMsTableCommand = new DelegateCommand(
                x => SelectionChangedTabConrtrolMsView()
                );
            #endregion


            #region launch window
            StartUpSettingWindow = new DelegateCommand(
                x => WindowUtility.StartUpParameterSettingWindow(MsLimaData));

            StartUpWindowAllSpectra = new DelegateCommand(
                x => WindowUtility.StartUpAllSpectraTableWindow(MsLimaData),
                x => !IsDataLoaded());
            StartUpWindowComparativeViewer = new DelegateCommand(
                x => WindowUtility.StartUpComparativeSpectraViewer(MsLimaData),
                x => !IsDataLoaded());
            #endregion
        }


        #region Methods for SelectionChanged Command

        public void SingleSpectrumTableSelectionChanged()
        {
            ControlRefresh.SelectedPeakChanged(TabMassSpectraView);
        }

        public void ConsensusTableSelectionChanged()
        {
            ControlRefresh.SelectedConsensusPeakChanged(TabMassSpectraView);
        }

        public void SelectionChangedTabConrtrolMsView()
        {
            SingleMassSpectrumRefresh();
            MsSpectraViewRefresh();
        }

        public void SelectedCompoundChanged()
        {

            OnPropertyChanged(nameof(LabelSelectedCompound));
            OnPropertyChanged(nameof(ConsensusSpectraTable));
            ShouldUpdateMultipleSpectrumVM = true;
            ShouldUpdateConsensusSpectrumVM = true;
            SelectedSpectrum = selectedCompoundBean.Spectra[0];
            if (!string.IsNullOrEmpty(SelectedCompoundBean.Smiles))
            {
                StructureImage = SmilesUtility.SmilesToMediaImageSource(SelectedCompoundBean.Smiles, 200, 200);
            }
            MsSpectraViewRefresh();
        }

        public void SelectedSpectrumChanged()
        {
            OnPropertyChanged(nameof(LabelSelectedSpectra));
            ShouldUpdateSingleMassSpectrumVM = true;
            SingleMassSpectrumRefresh();
        }

        public void SingleMassSpectrumRefresh()
        {
            if (TabMassSpectraView == TabMassSpectraView.SingleMS && ShouldUpdateSingleMassSpectrumVM)
            {
                SingleMassSpectrumVM = MsHandler.GetMassSpectrumDrawVisual(SelectedSpectrum);
                ShouldUpdateSingleMassSpectrumVM = false;
            }
        }

        public void MsSpectraViewRefresh()
        {
            if (TabMassSpectraView == TabMassSpectraView.MultipleMS && ShouldUpdateMultipleSpectrumVM)
            {
                MultipleSpectra = ControlRefresh.MultipleSpectraRefresh();
                ShouldUpdateMultipleSpectrumVM = false;
            }
            else if (TabMassSpectraView == TabMassSpectraView.ConsensusMS && ShouldUpdateConsensusSpectrumVM)
            {
                ConsensusSpectrumVM = MsHandler.GetMassSpectrumDrawVisualFromConsensus(ConsensusSpectraTable);
                ShouldUpdateConsensusSpectrumVM = false;
            }
        }
        #endregion


        #region Methods for MenuItem Commands

        public void ImportFile()
        {
            if (MsLimaData.DataStorage.CompoundList == null || MsLimaData.DataStorage.CompoundList.Count == 0) return;
            FilteredCompoundTable = new FilteredTable(this.CompoundTable);
            FilteredTableSetting = new FilterSettingsForLibrary(this.FilteredCompoundTable.View);
            FilteredCompoundTable.View.Filter = this.FilteredTableSetting.CompoundFilter;
            MainWindowTitle = Properties.Resources.Version + " File: " + MsLimaData.DataStorage.FilePath;
            SelectedCompoundBean = CompoundTable[0];
            OnPropertyChangedAfterFileImported();
        }

        #endregion

        #region Utilities
        private void OnPropertyChangedAfterFileImported()
        {
            StartUpWindowComparativeViewer.RaiseCanExecuteChanged();
            StartUpWindowAllSpectra.RaiseCanExecuteChanged();
            SaveAsMspCommand.RaiseCanExecuteChanged();
            SaveAsMspWithoutRTCommand.RaiseCanExecuteChanged();
            SaveAsMzMineCommand.RaiseCanExecuteChanged();
            UpdateSmilesAndInChiBasedOnInChIKeyCommand.RaiseCanExecuteChanged();
            ConvertAccurateMassToTheoreticalMass.RaiseCanExecuteChanged();
            DropRetentionTime.RaiseCanExecuteChanged();
            RemoveUnannotatedCommand.RaiseCanExecuteChanged();
            SaveCommonProductIonCommand.RaiseCanExecuteChanged();
            UpdateCommonMetaDataCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(FilteredComponudTableView));
            OnPropertyChanged(nameof(LabelNumCompounds));
            OnPropertyChanged(nameof(LabelNumSpectra));
        }

        private bool IsDataLoaded()
        {
            return (CompoundTable == null || CompoundTable.Count == 0);
        }

        public void AutoExportFunction()
        {
            Task.Run(() => ExportCompoundTable.ExportCompoundTableAsMsp(MsLimaData.DataStorage.FilePath, MsLimaData.DataStorage.CompoundList));
        }


        #endregion
    }
}
