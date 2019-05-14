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
        public MsLimaData MsLimaData { get; set; }

        public MainWindowVM() {
            this.MsLimaData = new MsLimaData();
        }


        public string Test { get; set; } = "testc";
        public FilteredTable FilteredCompoundTable { get; set; }
        public FilterSettingsForLibrary FilteredTableSetting { get; set; }

        public ObservableCollection<CompoundBean> CompoundTable {
            get { return new ObservableCollection<CompoundBean>(MsLimaData.DataStorage.CompoundList); }
        }

        private MassSpectrum selectedSpectrum;
        public MassSpectrum SelectedSpectrum{
            get => selectedSpectrum;
            set => OnPropertyChangedIfSet(ref selectedSpectrum, value, nameof(SelectedSpectrum));
        }

        private CompoundBean selectedCompoundBean;
        public CompoundBean SelectedCompoundBean {
            get => selectedCompoundBean;
            set => OnPropertyChangedIfSet(ref selectedCompoundBean, value, nameof(SelectedCompoundBean));
        }

        public void Refresh_ImportRawData()
        {
            FilteredCompoundTable = new FilteredTable(this.CompoundTable);
            FilteredTableSetting = new FilterSettingsForLibrary(this.FilteredCompoundTable.View);
            this.FilteredCompoundTable.View.Filter = this.FilteredTableSetting.CompoundFilter;

            /*            this.mainWindow.DataGrid_RawData.ItemsSource = RawData;
                        this.mainWindow.DataGrid_RawData.UpdateLayout();
                        if (data.rawData.Count == 0) return;
                        SelectedCompDataVM = data.rawData[0];
                        CompRefresh();
              */
        }

    }
}
