using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace VolunteerCenterDBClient
{
    public class MultiValueToOneConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //return values?.All(o => o?.Equals(values[0]) == true) == true || values?.All(o => o == null) == true;
            int index, index1;
            Button button = (Button)values[0];
            DataGrid dataGrid = (DataGrid)values[1];

            if (button != null && dataGrid != null)
            {
                index = dataGrid.Items.IndexOf(button);
                index1 = dataGrid.SelectedIndex;

                return index == index1;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
