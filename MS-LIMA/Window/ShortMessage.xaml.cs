using System.Windows;
using Metabolomics.MsLima.ViewModel;

namespace Metabolomics.MsLima
{
    /// <summary>
    /// ShortMessage.xaml の相互作用ロジック
    /// </summary>
    public partial class ShortMessage : Window
    {
        public ShortMessageVM ShortMessageVM { get; set; }

        public ShortMessage()
        {
            InitializeComponent();
            ShortMessageVM = new ShortMessageVM();
            this.DataContext = ShortMessageVM;
        }
    }
}
