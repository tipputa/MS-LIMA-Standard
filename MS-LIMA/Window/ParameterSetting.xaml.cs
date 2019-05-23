using System.Windows;
using Metabolomics.MsLima.ViewModel;

namespace Metabolomics.MsLima
{
    /// <summary>
    /// ParameterSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class ParameterSettingWindow : Window
    {
        public ParameterSettingWindow(MsLimaData data)
        {
            InitializeComponent();
            var vm = new ParameterSettingVM(data);
            this.DataContext = vm;
        }
    }
}
