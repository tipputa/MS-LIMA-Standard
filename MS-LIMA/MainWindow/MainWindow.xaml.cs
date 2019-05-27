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
using Metabolomics.MsLima.Model;
using Metabolomics.Core.Handler;
using Metabolomics.Core;
using Microsoft.Win32;

namespace Metabolomics.MsLima {

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowVM();
            //Image_Structure.Source = SmilesUtility.ConvertDrawingImageToBitmap(SmilesUtility.SmilesToImage("C", 300, 300));
        }
    }
}
