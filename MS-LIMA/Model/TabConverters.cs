using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabolomics.MsLima.Model
{
    public class TabMassSpectraViewIntConverter : ValueConverterBase<TabMassSpectraView, int>
    {
        public override int Convert(TabMassSpectraView value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public override TabMassSpectraView ConvertBack(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return (TabMassSpectraView)value;
        }
    }

    public class TabMassSpectraTableIntConverter : ValueConverterBase<TabMassSpectrumTable, int>
    {
        public override int Convert(TabMassSpectrumTable value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public override TabMassSpectrumTable ConvertBack(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return (TabMassSpectrumTable)value;
        }
    }

}
