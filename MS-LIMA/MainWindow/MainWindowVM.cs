using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabolomics.Core;
using Metabolomics.MsLima.Bean;

namespace Metabolomics.MsLima
{
    public class MainWindowVM : ViewModelBase
    {
        #region properties

        public MsLimaData MsLimaData { get; set; }
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
        #endregion

        public MainWindowVM() {
            this.MsLimaData = new MsLimaData();
        }


        public void Refresh_ImportRawData()
        {
            FilteredCompoundTable = new FilteredTable(this.CompoundTable);
            FilteredTableSetting = new FilterSettingsForLibrary(this.FilteredCompoundTable.View);
            FilteredCompoundTable.View.Filter = this.FilteredTableSetting.CompoundFilter;
        }

    }
}
