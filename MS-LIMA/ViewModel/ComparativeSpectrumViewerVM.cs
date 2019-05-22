using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Metabolomics.Core;
using Metabolomics.MsLima.Model;
using Metabolomics.MsLima.Bean;
using ChartDrawing;

namespace Metabolomics.MsLima.ViewModel
{
    public class ComparativeSpectrumViewerVM : ViewModelBase
    {
        #region member variables and properties

        private string leftFilePath;
        private string rightFilePath;

        private string leftNameText;
        private string leftMzText;
        private string leftRtText;
        private string rightNameText;
        private string rightMzText;
        private string rightRtText;
        private string targetInChIKey;

        private float totalScore;
        private float dotScore;
        private float revScore;
        private float matchScore;

        private bool share = true;

        public MassSpectrumViewHandler MsHandler;
        public FilteredTable FilteredTableLeft { get; set; }
        public FilteredTable FilteredTableRight { get; set; }
        public FilterSettingsForLibrary FilterSettingLeft { get; set; }
        public FilterSettingsForLibrary FilterSettingRight { get; set; }
        public bool Share {
            get { return share; }
            set { if (share == value) return; share = value; OnPropertyChanged("Share"); }
        }

        public ICollectionView LeftSpectraTable {
            get => FilteredTableLeft.View;
        }

        public ICollectionView RightSpectraTable {
            get => FilteredTableRight.View;
        }

        
        public MassSpectrum SelectedMassSpectrumLeft { get; set; }
        public MassSpectrum SelectedMassSpectrumRight { get; set; }


        public string LeftFilePath {
            get => leftFilePath;
            set => OnPropertyChangedIfSet(ref leftFilePath, value, nameof(LeftFilePath));
        }

        public string RightFilePath {
            get => rightFilePath;
            set => OnPropertyChangedIfSet(ref rightFilePath, value, nameof(RightFilePath));
        }

        public string LeftNameText {
            get => leftNameText;
            set { if (OnPropertyChangedIfSet(ref leftNameText, value, nameof(LeftNameText)))
                {
                    this.FilterSettingLeft.MetaboliteNameFilter = value;
                    if (share)
                    {
                        this.RightNameText = value;
                    }
                }
            }
        }

        public string RightNameText {
            get => rightNameText;
            set {
                if (OnPropertyChangedIfSet(ref rightNameText, value, nameof(RightNameText)))
                {
                    this.FilterSettingRight.MetaboliteNameFilter = value;
                    if (share)
                    {
                        this.LeftNameText = value;
                    }
                }
            }
        }

        public string LeftMzText {
            get => leftMzText;
            set {
                if (OnPropertyChangedIfSet(ref leftMzText, value, nameof(LeftMzText)))
                {
                    this.FilterSettingLeft.MetaboliteNameFilter = value;
                    if (share)
                    {
                        this.RightMzText = value;
                    }
                }
            }
        }
        public string RightMzText {
            get => rightMzText;
            set {
                if (OnPropertyChangedIfSet(ref rightMzText, value, nameof(RightMzText)))
                {
                    this.FilterSettingRight.MetaboliteNameFilter = value;
                    if (share)
                    {
                        this.LeftMzText = value;
                    }
                }
            }
        }

        public string LeftRtText {
            get => leftRtText;
            set {
                if (OnPropertyChangedIfSet(ref leftRtText, value, nameof(LeftRtText)))
                {
                    this.FilterSettingLeft.MetaboliteNameFilter = value;
                    if (share)
                        this.RightRtText = value;
                }
            }
        }

        public string RightRtText {
            get => rightRtText;
            set {
                if (OnPropertyChangedIfSet(ref rightRtText, value, nameof(RightRtText)))
                {
                    this.FilterSettingRight.MetaboliteNameFilter = value;
                    if (share)
                        this.LeftRtText = value;
                }
            }
        }


        public string TargetInChIKey {
            get => targetInChIKey;
            set {
                if(OnPropertyChangedIfSet(ref targetInChIKey, value, nameof(TargetInChIKey)))
                    this.FilterSettingRight.InChIKeyFilter = value;
            }
        }

        public DrawVisual MassSpectrumVM { get; set; } = new DrawVisual();

        private DefaultUC massSpectrumWithRef;
        public DefaultUC MassSpectrumWithRef {
            get => massSpectrumWithRef;
            set {
                massSpectrumWithRef = value;
                OnPropertyChanged(nameof(MassSpectrumWithRef));
            }
        }

        public float TotalScore {
            get => totalScore;
            set => OnPropertyChangedIfSet(ref totalScore, value, nameof(TotalScore));
        }

        public float DotScore {
            get => dotScore;
            set => OnPropertyChangedIfSet(ref dotScore, value, nameof(DotScore));
        }

        public float RevScore {
            get => revScore;
            set => OnPropertyChangedIfSet(ref revScore, value, nameof(RevScore));
        }

        public float MatchScore {
            get => matchScore;
            set => OnPropertyChangedIfSet(ref matchScore, value, nameof(MatchScore));
        }

        #endregion

        #region Commands
        public DelegateCommand ImportLeftFileCommand { get; set; }
        public DelegateCommand ImportRightFileCommand { get; set; }
        public DelegateCommand ReflectionCheckBoxCheckedCommand { get; set; }
        public DelegateCommand ReflectionCheckBoxUncheckedCommand { get; set; }
        public DelegateCommand SameInChIKeyCheckBoxCheckedCommand { get; set; }
        public DelegateCommand SameInChIKeyCheckBoxUncheckedCommand { get; set; }
        public DelegateCommand SpectraSelectionChangedCommand { get; set; }

        #endregion

        public ComparativeSpectrumViewerVM(MsLimaData data)
        {
            MsHandler = new MassSpectrumViewHandler(data.Parameter);
            var spectra = ComparativeSpectrumViewerModel.ConvertCompoundDataToSpectra(data);
            MassSpectrumWithRef = new DefaultUC(MassSpectrumVM);
            LeftFilterUpdate(spectra);
            RightFilterUpdate(spectra);
            SetCommands();
        }

        public void SetCommands() {
            ImportLeftFileCommand = new DelegateCommand(
                x => LeftFilterUpdate(ComparativeSpectrumViewerModel.ImportFile()));
                    
            ImportRightFileCommand = new DelegateCommand(
                x => RightFilterUpdate(ComparativeSpectrumViewerModel.ImportFile()));
            SpectraSelectionChangedCommand = new DelegateCommand(
                x =>
                {
                    MassSpectrumVM = MsHandler.GetMassSpectrumWithRefDrawVisual(SelectedMassSpectrumLeft, SelectedMassSpectrumRight);
                    MassSpectrumWithRef = new DefaultUC(MassSpectrumVM);
                });
        }

        private void LeftFilterUpdate(List<MassSpectrum> spectra)
        {
            FilteredTableLeft = new FilteredTable(spectra);
            FilterSettingLeft = new FilterSettingsForLibrary(FilteredTableLeft.View);
            FilteredTableLeft.View.Filter = FilterSettingLeft.MassSpectrumFilter;
            OnPropertyChanged(nameof(LeftSpectraTable));
        }

        private void RightFilterUpdate(List<MassSpectrum> spectra)
        {
            FilteredTableRight = new FilteredTable(spectra);
            FilterSettingRight = new FilterSettingsForLibrary(FilteredTableRight.View);
            FilteredTableRight.View.Filter = FilterSettingRight.MassSpectrumFilter;
            OnPropertyChanged(nameof(RightSpectraTable));
        }

    }
}
