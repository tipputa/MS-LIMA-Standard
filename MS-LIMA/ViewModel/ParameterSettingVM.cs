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

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public ParameterSettingVM(MsLimaData data)
        {
            MsLimaData = data;
            CopyParam = MsLimaData.Parameter.Copy();
            SetCommands();

        }

        public void SetCommands()
        {
            SaveCommand = new DelegateCommand(x => Save());
            CancelCommand = new DelegateCommand(win => ((ParameterSettingWindow)win).Close());
        }

        

        public void Save()
        {
            MsLimaData.Parameter = CopyParam;
        }


    }
}
