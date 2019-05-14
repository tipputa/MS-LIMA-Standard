using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Metabolomics.Core;
using Metabolomics.MsLima.Bean;

namespace Metabolomics.MsLima
{
    public class FilteredTable
    {
        public ICollectionView View { get; private set; }

        public FilteredTable(System.Collections.IList list)
        {
            View = System.Windows.Data.CollectionViewSource.GetDefaultView(list);
        }
    }

    public class FilterSettingsForLibrary : ViewModelBase
    {
        #region member and variables

        private string _metaboliteNameFilter = "";
        private string _inchiKeyFilter = "";
        private string _mzFilter = "";
        private string _rtFilter = "";

        private ICollectionView view;

        public string MetaboliteNameFilter {
            get { return _metaboliteNameFilter; }
            set { if (_metaboliteNameFilter == value) return; _metaboliteNameFilter = value; Update(); OnPropertyChanged("MetaboliteNameFilter"); }
        }

        public string InChIKeyFilter {
            get { return _inchiKeyFilter; }
            set { if (_inchiKeyFilter == value) return; _inchiKeyFilter = value; Update(); OnPropertyChanged("InChIKeyFilter"); }
        }

        public string RetentionTimeFilter {
            get { return _rtFilter; }
            set { if (_rtFilter == value) return; _rtFilter = value; Update(); OnPropertyChanged("RetentionTimeFilter"); }
        }

        public string MzFilter {
            get { return _mzFilter; }
            set { if (_mzFilter == value) return; _mzFilter = value; Update(); OnPropertyChanged("MzFilter"); }
        }

        private void Update()
        {
            this.view.Refresh();
        }



        #endregion

        public FilterSettingsForLibrary(ICollectionView view)
        {
            this.view = view;
        }

        public bool MspFilter(object sender)
        {
            var msp = (MspBean)sender;
            if (this.MetaboliteNameFilter != string.Empty && !msp.Name.ToLower().Contains(this.MetaboliteNameFilter.ToLower())) return false;
            if (this.RetentionTimeFilter != string.Empty && msp.RetentionTime.ToString().IndexOf(this.RetentionTimeFilter, 0) < 0) return false;
            if (this.MzFilter != string.Empty && msp.PrecursorMz.ToString().IndexOf(this.MzFilter, 0) < 0) return false;
            if (this.InChIKeyFilter != string.Empty && !msp.InchiKey.ToLower().Contains(this.InChIKeyFilter.ToLower())) return false;
            return true;
        }

        public bool CompoundFilter(object sender)
        {
            var comp = (CompoundBean)sender;
            if (this.MetaboliteNameFilter != string.Empty && !comp.Name.ToLower().Contains(this.MetaboliteNameFilter.ToLower())) return false;
            if (this.RetentionTimeFilter != string.Empty && !comp.RetentionTimes.Contains(this.RetentionTimeFilter.ToLower())) return false;
            if (this.MzFilter != string.Empty && comp.MolecularWeight.ToString().IndexOf(this.MzFilter, 0) < 0) return false;
            if (this.InChIKeyFilter != string.Empty && !comp.InChIKey.ToLower().Contains(this.InChIKeyFilter.ToLower())) return false;

            return true;
        }

    }
}
