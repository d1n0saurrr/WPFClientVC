using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace VolunteerCenterDBClient.Views.Pages
{
    /// <summary>
    /// Interaction logic for AdminMainPage.xaml
    /// </summary>
    public partial class AdminMainPage : Page
    {
        public AdminMainPage()
        {
            InitializeComponent();
            start.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);

            GridCursor.Margin = new Thickness(150 * index, 0, 0, 0);

            addButtonVol.Visibility = Visibility.Collapsed;
            searchVol.Visibility = Visibility.Collapsed;
            findVolButton.Visibility = Visibility.Collapsed;
            volsGrid.Visibility = Visibility.Collapsed;

            addButtonEvent.Visibility = Visibility.Collapsed;
            searchEvent.Visibility = Visibility.Collapsed;
            findEventButton.Visibility = Visibility.Collapsed;
            eventsGrid.Visibility = Visibility.Collapsed;

            addButtonOrg.Visibility = Visibility.Collapsed;
            searchOrg.Visibility = Visibility.Collapsed;
            findOrgButton.Visibility = Visibility.Collapsed;
            orgsGrid.Visibility = Visibility.Collapsed;

            usersGrid.Visibility = Visibility.Collapsed;

            switch (index)
            {
                case 0:
                    addButtonVol.Visibility = Visibility.Visible;
                    searchVol.Visibility = Visibility.Visible;
                    findVolButton.Visibility = Visibility.Visible;
                    volsGrid.Visibility = Visibility.Visible;
                    break;

                case 1:
                    addButtonEvent.Visibility = Visibility.Visible;
                    searchEvent.Visibility = Visibility.Visible;
                    findEventButton.Visibility = Visibility.Visible;
                    eventsGrid.Visibility = Visibility.Visible;
                    break;

                case 2:
                    addButtonOrg.Visibility = Visibility.Visible;
                    searchOrg.Visibility = Visibility.Visible;
                    findOrgButton.Visibility = Visibility.Visible;
                    orgsGrid.Visibility = Visibility.Visible;
                    break;

                case 3:
                    usersGrid.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
