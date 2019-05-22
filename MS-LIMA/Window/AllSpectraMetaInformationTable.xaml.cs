using System.Windows;
using Metabolomics.MsLima.ViewModel;

namespace Metabolomics.MsLima
{
    /// <summary>
    /// AllSpectraMetaInformationTable.xaml の相互作用ロジック
    /// </summary>
    public partial class AllSpectraMetaInformationTable : Window
    {
        public AllSpectraMetaInformationTable(MsLimaData data)
        {
            InitializeComponent();
            this.DataContext = new AllSpectraMetaInformationTableVM(data);
        }
    }
}
