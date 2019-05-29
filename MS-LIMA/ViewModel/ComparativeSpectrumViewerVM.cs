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

        public ParameterBean Param { get; set; }
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
                    if (Share)
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
                    if (Share)
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
                    this.FilterSettingLeft.MzFilter = value;
                    if (Share)
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
                    this.FilterSettingRight.MzFilter = value;
                    if (Share)
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
                    this.FilterSettingLeft.RetentionTimeFilter = value;
                    if (Share)
                        this.RightRtText = value;
                }
            }
        }

        public string RightRtText {
            get => rightRtText;
            set {
                if (OnPropertyChangedIfSet(ref rightRtText, value, nameof(RightRtText)))
                {
                    this.FilterSettingRight.RetentionTimeFilter = value;
                    if (Share)
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
            Param = data.Parameter;
            MsHandler = new MassSpectrumViewHandler(data.Parameter);
            var spectra = ComparativeSpectrumViewerModel.ConvertCompoundDataToSpectra(data);
            var spectra2 = ComparativeSpectrumViewerModel.ConvertCompoundDataToSpectra(data);
            MassSpectrumWithRef = new DefaultUC(MassSpectrumVM);
            LeftFilePath = data.DataStorage.FilePath;
            RightFilePath = data.DataStorage.FilePath;
            LeftFilterUpdate(spectra);
            RightFilterUpdate(spectra2);
            SetCommands();
        }

        public void SetCommands() {
            ImportLeftFileCommand = new DelegateCommand(
                x => {
                    var spectra = ComparativeSpectrumViewerModel.ImportFile(out string path);
                    if (spectra != null && spectra.Count > 0) {
                        LeftFilePath = path;
                        LeftFilterUpdate(spectra);
                } });
                    
            ImportRightFileCommand = new DelegateCommand(
                x => {
                    var spectra = ComparativeSpectrumViewerModel.ImportFile(out string path);
                    if (spectra != null && spectra.Count > 0)
                    {
                        RightFilePath = path;
                        RightFilterUpdate(spectra);
                    }
                });
            SpectraSelectionChangedCommand = new DelegateCommand(
                x =>
                {
                    SetScores(SelectedMassSpectrumLeft, SelectedMassSpectrumRight);
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

        public void SetScores(MassSpectrum spectrum1, MassSpectrum spectrum2)
        {
            var ms2tol = (float)this.Param.MS2Tol;
            var dotProductFactor = 1.0;
            var reverseDotProdFactor = 1.0;
            var presensePercentageFactor = 1.0;
            if (spectrum1 == null || spectrum2 == null)
            {
                this.DotScore = 0;
                this.RevScore = 0;
                this.MatchScore = 0;
                this.TotalScore = 0;
            }
            else
            {
                this.DotScore = (float)MsSimilarityScoring.GetMassSpectraSimilarity(spectrum1, spectrum2, ms2tol) * 1000;
                this.RevScore = (float)MsSimilarityScoring.GetReverseSearchSimilarity(spectrum1, spectrum2, ms2tol) * 1000;
                this.MatchScore = (float)MsSimilarityScoring.GetPresenceSimilarityBasedOnReference(spectrum1, spectrum2, ms2tol) * 1000;
                this.TotalScore = (float)((dotProductFactor * DotScore + reverseDotProdFactor * RevScore + presensePercentageFactor * MatchScore) / (dotProductFactor + reverseDotProdFactor + presensePercentageFactor));
            }
        }


    }
}
