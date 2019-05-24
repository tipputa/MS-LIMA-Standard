using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabolomics.MsLima.Bean;

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

    public class EnumToCllectionConverter : ValueConverterBase<CompoundGroupingKey, IEnumerable<string>>
    {
        public override IEnumerable<string> Convert(CompoundGroupingKey value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetNames(typeof(CompoundGroupingKey)).ToList();
        }

        public override CompoundGroupingKey ConvertBack(IEnumerable<string> value, Type targetType, object parameter, CultureInfo culture)
        {
            return new CompoundGroupingKey();
            /*
            CompoundGroupingKey[] values = new CompoundGroupingKey[value.Count()];
            string[] names = Enum.GetNames(typeof(CompoundGroupingKey));


            for (int x = 0; x < values.Length; x++)
            {
                //Parse specific string to enum
                values[x] = (CompoundGroupingKey)Enum.Parse(typeof(CompoundGroupingKey), names[x]);
            }
            */
        }
    }

}
