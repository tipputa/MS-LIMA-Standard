using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Metabolomics.MsLima.Bean;
using Metabolomics.Core.Handler;

namespace Metabolomics.MsLima { 

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        public string test;
        public MsLimaDataBean MsLimaDataBean { get; set; }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            this.MsLimaDataBean = new MsLimaDataBean();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
