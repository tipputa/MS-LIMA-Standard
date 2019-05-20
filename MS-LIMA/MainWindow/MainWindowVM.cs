using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabolomics.Core;
using Metabolomics.MsLima.Bean;
using System.Windows.Controls;

namespace Metabolomics.MsLima
{
    public class MainWindowVM : ViewModelBase
    {
        #region properties

        public MsLimaData MsLimaData { get; set; }
        public MassSpectrumViewHandler MsHandler { get; set; }

        public FilteredTable FilteredCompoundTable { get; set; }
        public FilterSettingsForLibrary FilteredTableSetting { get; set; }

        public ObservableCollection<CompoundBean> CompoundTable {
            get { return new ObservableCollection<CompoundBean>(MsLimaData.DataStorage.CompoundList); }
        }

        private AnnotatedPeak selectedPeak;
        public AnnotatedPeak SelectedPeak {
            get => selectedPeak;
            set => OnPropertyChangedIfSet(ref selectedPeak, value, nameof(SelectedPeak));
        }

        private MassSpectrum selectedSpectrum;
        public MassSpectrum SelectedSpectrum {
            get => selectedSpectrum;
            set => OnPropertyChangedIfSet(ref selectedSpectrum, value, nameof(SelectedSpectrum));
        }

        private CompoundBean selectedCompoundBean;
        public CompoundBean SelectedCompoundBean {
            get => selectedCompoundBean;
            set => OnPropertyChangedIfSet(ref selectedCompoundBean, value, nameof(SelectedCompoundBean));
        }

        private MsGroup selectedMsGroup;
        public MsGroup SelectedMsGroup {
            get => selectedMsGroup;
            set => OnPropertyChangedIfSet(ref selectedMsGroup, value, nameof(SelectedMsGroup));
        }

        private List<MsGroup> consensusSpectraTable;
        public List<MsGroup> ConsensusSpectraTable {
            get => consensusSpectraTable;
            set => OnPropertyChangedIfSet(ref consensusSpectraTable, value, nameof(ConsensusSpectraTable));
        }

        #region ViewModel
        private DrawVisualMassSpectrum singleMassSpectrumVM = new DrawVisualMassSpectrum();
        public DrawVisualMassSpectrum SingleMassSpectrumVM {
            get => singleMassSpectrumVM;
            set {
                OnPropertyChangedIfSet(ref singleMassSpectrumVM, value, nameof(SingleMassSpectrumVM));
                SingleMassSpectrumUI.UpdateFE(value);
            }
        }

        private DrawVisualMassSpectrum consensusSpectrumVM = new DrawVisualMassSpectrum();
        public DrawVisualMassSpectrum ConsensusSpectrumVM {
            get => consensusSpectrumVM;
            set { OnPropertyChangedIfSet(ref consensusSpectrumVM, value, nameof(ConsensusSpectrumVM));
                ConsensusSpectrumUI.UpdateFE(value);
            }
        }
    #endregion

    #region UI
    public MassSpectrumUI SingleMassSpectrumUI { get; set; }
        public MassSpectrumUI ConsensusSpectrumUI { get; set; }

        private StackPanel multipleSpectra;
        public StackPanel MultipleSpectra {
            get => multipleSpectra;
            set => OnPropertyChangedIfSet(ref multipleSpectra, value, nameof(MultipleSpectra)); }

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
        #endregion

        public MainWindowVM() {
            this.MsLimaData = new MsLimaData();
            this.MsHandler = new MassSpectrumViewHandler(MsLimaData.Parameter);

            SingleMassSpectrumUI = new MassSpectrumUI(SingleMassSpectrumVM);
            ConsensusSpectrumUI = new MassSpectrumUI(ConsensusSpectrumVM);
            MultipleSpectra = new StackPanel();
        }


        public void Refresh_ImportRawData()
        {
            FilteredCompoundTable = new FilteredTable(this.CompoundTable);
            FilteredTableSetting = new FilterSettingsForLibrary(this.FilteredCompoundTable.View);
            FilteredCompoundTable.View.Filter = this.FilteredTableSetting.CompoundFilter;
        }
    }
}
