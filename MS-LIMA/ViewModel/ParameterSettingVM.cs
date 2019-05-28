using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabolomics.MsLima;
using Metabolomics.MsLima.Bean;
using Metabolomics.Core;
namespace Metabolomics.MsLima.ViewModel
{
    public class ParameterSettingVM : ViewModelBase
    {
        public ParameterBean CopyParam;
        public MsLimaData MsLimaData;

        public double Ms2Tol { get => CopyParam.MS2Tol; set { if (CopyParam.MS2Tol == value) return; CopyParam.MS2Tol = value; OnPropertyChanged(nameof(Ms2Tol)); } }
        
        public CompoundGroupingKey CompoundGroupingKey {
            get { return CopyParam.CompoundGroupingKey; }
            set { CopyParam.CompoundGroupingKey = value; OnPropertyChanged(nameof(CompoundGroupingKey)); }
        }

        public int NumberOfDecimalPlace { get => CopyParam.NumberOfDecimalPlaces;
            set { if (CopyParam.NumberOfDecimalPlaces == value) return;
                CopyParam.NumberOfDecimalPlaces = value;
                OnPropertyChanged(nameof(NumberOfDecimalPlace));
            }
        }

        public int MinimumNumberOfSamplesForConsensus { get => CopyParam.MinimumNumberOfSamplesForConsensus;
            set { if (CopyParam.MinimumNumberOfSamplesForConsensus == value) return;
                CopyParam.MinimumNumberOfSamplesForConsensus = value;
                OnPropertyChanged(nameof(MinimumNumberOfSamplesForConsensus));
            }
        }

        public int AutoExportIntervalMillisecond {
            get => CopyParam.WinParam.AutoExportIntervalMillisecond;
            set {
                if (CopyParam.WinParam.AutoExportIntervalMillisecond == value) return;
                CopyParam.WinParam.AutoExportIntervalMillisecond = value;
                OnPropertyChanged(nameof(AutoExportIntervalMillisecond));
            }
        }

        public int GraphHeightInMultipleView {
            get => CopyParam.WinParam.GraphHeightInMultipleView;
            set {
                if (CopyParam.WinParam.GraphHeightInMultipleView == value) return;
                CopyParam.WinParam.GraphHeightInMultipleView = value;
                OnPropertyChanged(nameof(GraphHeightInMultipleView));
            }
        }

        public float RetentionTimeTol {
            get => CopyParam.RtTol;
            set {
                if (CopyParam.RtTol == value) return;
                CopyParam.RtTol = value;
                OnPropertyChanged(nameof(RetentionTimeTol));
            }
        }

        private int selectedId;
        public int SelectedId {
            get {
                return selectedId;
            }
            set {
                if (OnPropertyChangedIfSet(ref selectedId, value, nameof(SelectedId)))
                {
                    CopyParam.CompoundGroupingKey = (CompoundGroupingKey)Enum.ToObject(typeof(CompoundGroupingKey), selectedId);
                }

            }
        }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public ParameterSettingVM(MsLimaData data)
        {
            MsLimaData = data;
            CopyParam = MsLimaData.Parameter.Copy();
            selectedId = (int)CopyParam.CompoundGroupingKey;
            SetCommands();

        }

        public void SetCommands()
        {
            SaveCommand = new DelegateCommand(win => { Save(); ((ParameterSettingWindow)win).Close(); });
            CancelCommand = new DelegateCommand(win => ((ParameterSettingWindow)win).Close());
        }

        

        public void Save()
        {
            MsLimaData.Parameter.Update(CopyParam);
            MsLimaData.WriteParameterFile();
        }


    }
}
