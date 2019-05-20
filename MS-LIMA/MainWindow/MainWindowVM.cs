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
        public TabMassSpectraView TabMassSpectraView { get; set; } = TabMassSpectraView.SingleMS;
        public TabMassSpectrumTable TabMassSpectrumTable { get; set; } = TabMassSpectrumTable.SinglePeak;

        public ICollectionView FilteredComponudTableView {
            get => FilteredCompoundTable.View;
        }


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
                OnPropertyChangedIfSet(ref selectedSpectrum, value, nameof(SelectedSpectrum));
                OnPropertyChanged(nameof(LabelSelectedSpectra));
                OnPropertyChanged(nameof(SingleMassSpectrumVM));
                OnPropertyChanged(nameof(SingleMassSpectrumUI));
            }
        }

        public CompoundBean SelectedCompoundBean {
            get => selectedCompoundBean;
            set {
                OnPropertyChangedIfSet(ref selectedCompoundBean, value, nameof(SelectedCompoundBean));
                OnPropertyChanged(nameof(LabelSelectedCompound));
                OnPropertyChanged(nameof(ConsensusSpectraTable));
                OnPropertyChanged(nameof(MultipleSpectra));
            }
        }

        public MsGroup SelectedMsGroup {
            get => selectedMsGroup;
            set => OnPropertyChangedIfSet(ref selectedMsGroup, value, nameof(SelectedMsGroup));
        }

        public List<MsGroup> ConsensusSpectraTable {
            get {
                if (SelectedCompoundBean == null) return null;
                return MsGrouping.Excute(SelectedCompoundBean);
            }
            //set => OnPropertyChangedIfSet(ref consensusSpectraTable, value, nameof(ConsensusSpectraTable));
        }

        #region Label
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
        #endregion

        #region ViewModel
        public DrawVisualMassSpectrum SingleMassSpectrumVM {
            get {
                if (SelectedSpectrum == null) return new DrawVisualMassSpectrum();
                return  MsHandler.GetMassSpectrumDrawVisual(SelectedSpectrum);
            }
        }

        private DrawVisualMassSpectrum consensusSpectrumVM = new DrawVisualMassSpectrum();
        public DrawVisualMassSpectrum ConsensusSpectrumVM {
            get {
                if (ConsensusSpectraTable == null) return new DrawVisualMassSpectrum();
                return MsHandler.GetMassSpectrumDrawVisualFromConsensus(ConsensusSpectraTable);
            }
        }
        #endregion

        #region UI
        private MassSpectrumUI singleMassSpectrumUI;
        public MassSpectrumUI SingleMassSpectrumUI {
            get {
                singleMassSpectrumUI.UpdateFE(SingleMassSpectrumVM); 
                return singleMassSpectrumUI;
            }
            set {
                singleMassSpectrumUI = value;
            }
        }
        public MassSpectrumUI ConsensusSpectrumUI { get; set; }

        public StackPanel MultipleSpectra {
            get {
                if (SelectedCompoundBean == null) return null;
                return ControlRefresh.MultipleSpectraRefresh();
            }
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

        public DelegateCommand ImportFileCommand { get; set; }

        public DelegateCommand SaveAsMspCommand { get; set; }

        #endregion
        #endregion

        public MainWindowVM() {
            this.MsLimaData = new MsLimaData();
            this.MsHandler = new MassSpectrumViewHandler(MsLimaData.Parameter);
            this.AutoExporter = new AutoRepeater(MsLimaData.Parameter.AutoExportIntervalMillisecond);
            this.AutoExporter.OnTimeEventHandler += (o, e) => { AutoExportFunction(); }; 
            SingleMassSpectrumUI = new MassSpectrumUI(SingleMassSpectrumVM);
            ConsensusSpectrumUI = new MassSpectrumUI(ConsensusSpectrumVM);
            ControlRefresh = new ControlRefresh(this);
            SetCommands();
        }


        private void SetCommands()
        {
            ImportFileCommand = new DelegateCommand(
                x => ImportFile()
                );

            SaveAsMspCommand = new DelegateCommand(
                x => ExportUtility.SaveAsMsp(CompoundTable),
                x => !IsDataLoaded());

        }

        public void AutoExportFunction()
        {

        }

        public void ImportFile()
        {
            ImportUtility.ImportFile(MsLimaData);
            RefreshImportRawData();
        }

        public void RefreshImportRawData()
        {
            FilteredCompoundTable = new FilteredTable(this.CompoundTable);
            FilteredTableSetting = new FilterSettingsForLibrary(this.FilteredCompoundTable.View);
            FilteredCompoundTable.View.Filter = this.FilteredTableSetting.CompoundFilter;

            SaveAsMspCommand.RaiseCanExecuteChanged();

            SelectedCompoundBean = CompoundTable[0];

            OnPropertyChanged(nameof(FilteredComponudTableView));
            OnPropertyChanged(nameof(LabelNumCompounds));
            OnPropertyChanged(nameof(LabelNumSpectra));
        }


        private bool IsDataLoaded()
        {
            return (CompoundTable == null || CompoundTable.Count == 0);
        } 

    }
}
