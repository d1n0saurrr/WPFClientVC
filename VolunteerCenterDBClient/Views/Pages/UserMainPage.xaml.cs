using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace VolunteerCenterDBClient.Views.Pages
{
    /// <summary>
    /// Interaction logic for UserMainPage.xaml
    /// </summary>
    public partial class UserMainPage : Page
    {
        public UserMainPage()
        {
            InitializeComponent();
        }

        private void add()
        {
            MaterialDesignThemes.Wpf.Card card = new MaterialDesignThemes.Wpf.Card();
            card.Width = 700;
            card.Height = 100;
            card.Margin = new Thickness(0, 10, 0, 10);

            Grid cardGrid = new Grid();

            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(2, GridUnitType.Star);
            RowDefinition rowDefinition1 = new RowDefinition();
            rowDefinition1.Height = new GridLength(1, GridUnitType.Star);
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(7.5, GridUnitType.Star);
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            columnDefinition.Width = new GridLength(2.5, GridUnitType.Star);

            cardGrid.RowDefinitions.Add(rowDefinition);
            cardGrid.RowDefinitions.Add(rowDefinition1);
            cardGrid.ColumnDefinitions.Add(columnDefinition);
            cardGrid.ColumnDefinitions.Add(columnDefinition1);

            TextBlock eventName = new TextBlock();
            eventName.Text = "Таврида арт";
            eventName.FontSize = 40;
            eventName.Margin = new Thickness(15, 10, 0, 0);
            Grid.SetRow(eventName, 0);
            Grid.SetColumn(eventName, 0);
            cardGrid.Children.Add(eventName);

            Button button = new Button();
            button.Content = "Подать заявку";
            button.Background = FindResource("buttonBackground") as Brush;
            button.BorderBrush = FindResource("buttonBackground") as Brush;
            button.Margin = new Thickness(15, 29, 16, 30);
            button.Width = 159;
            button.Height = 41;
            Binding binding = new Binding("ChooseEventCommand");
            binding.Source = this.DataContext;
            Binding binding1 = new Binding("SelectedIndex");
            binding1.Source = this.DataContext;
            button.SetBinding(Button.CommandProperty, binding);
            button.SetBinding(Button.CommandParameterProperty, binding1);
            Grid.SetRowSpan(button, 2);
            Grid.SetColumn(button, 1);
            cardGrid.Children.Add(button);

            DockPanel dockPanel = new DockPanel();

            TextBlock org = new TextBlock();
            org.Text = "Организатор:";
            org.Margin = new Thickness(15, 5, 2, 6);
            dockPanel.Children.Add(org);

            TextBlock orgName = new TextBlock();
            orgName.Text = "Да-да я";
            orgName.Margin = new Thickness(0, 5, 25, 6);
            dockPanel.Children.Add(orgName);

            TextBlock date = new TextBlock();
            date.Text = "Дата проведения:";
            date.Margin = new Thickness(0, 5, 2, 6);
            dockPanel.Children.Add(date);

            TextBlock eventDate = new TextBlock();
            eventDate.Text = "10.10.2020 - 11.11.2020";
            eventDate.Margin = new Thickness(0, 5, 25, 5.5);
            dockPanel.Children.Add(eventDate);

            Grid.SetRow(dockPanel, 1);
            Grid.SetColumnSpan(dockPanel, 2);
            cardGrid.Children.Add(dockPanel);
            card.Content = cardGrid;

            //listBox.Items.Add(card);
        }

        private void Page_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DockPanel dockPanel = ((DockPanel)Application.Current.MainWindow.FindName("_menu"));
            dockPanel.Visibility = Visibility.Visible;

            Binding binding = new Binding("AllEventsCommand")
            {
                Source = this.DataContext
            };
            (dockPanel.FindName("main") as Button).SetBinding(Button.CommandProperty, binding);
            (dockPanel.FindName("main") as Button).Click += MainClick;

            binding = new Binding("MyEventsCommand")
            {
                Source = this.DataContext
            };
            (dockPanel.FindName("myEvents") as Button).SetBinding(Button.CommandProperty, binding);
            (dockPanel.FindName("myEvents") as Button).Click += MyEventsClick;

            binding = new Binding("ProfileCommand")
            {
                Source = this.DataContext
            };
            (dockPanel.FindName("profile") as Button).SetBinding(Button.CommandProperty, binding);
            (dockPanel.FindName("profile") as Button).Click += ProfileClick;

            binding = new Binding("GoBackCommand")
            {
                Source = this.DataContext
            };
            (dockPanel.FindName("exit") as Button).SetBinding(Button.CommandProperty, binding);
            (dockPanel.FindName("exit") as Button).Click += ExitClick;

            Grid.SetRow((dockPanel.FindName("_navigationFrame") as Frame), 1);
            //Grid.SetRowSpan((dockPanel.FindName("_navigationFrame") as Frame), 0);
        }

        private void MainClick(object sender, RoutedEventArgs e)
        {
            listBox.Visibility = Visibility.Visible;
            listBoxUserEvents.Visibility = Visibility.Collapsed;
            listBoxUserRequest.Visibility = Visibility.Collapsed;
            profileGrid.Visibility = Visibility.Collapsed;
            //events = (this.DataContext as UserMainPageViewModel).ItemsSourceEvents;
        }

        private void MyEventsClick(object sender, RoutedEventArgs e)
        {
            listBox.Visibility = Visibility.Collapsed;
            listBoxUserEvents.Visibility = Visibility.Visible;
            listBoxUserRequest.Visibility = Visibility.Visible;
            profileGrid.Visibility = Visibility.Collapsed;
        }

        private void ProfileClick(object sender, RoutedEventArgs e)
        {
            listBox.Visibility = Visibility.Collapsed;
            listBoxUserEvents.Visibility = Visibility.Collapsed;
            listBoxUserRequest.Visibility = Visibility.Collapsed;
            profileGrid.Visibility = Visibility.Visible;
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            DockPanel dockPanel = ((DockPanel)Application.Current.MainWindow.FindName("_menu"));
            dockPanel.Visibility = Visibility.Collapsed;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show((sender as ListBox).SelectedIndex.ToString());
        }
    }
}
