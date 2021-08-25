using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using VolunteerCenterDBClient.Models;

namespace VolunteerCenterDBClient.Views
{
    /// <summary>
    /// Interaction logic for OrgWindow.xaml
    /// </summary>
    public partial class OrgWindow : Window
    {
        public bool IsDeleting { get; private set; }
        private Organization _org;

        public Organization Org
        {
            get
            {
                Organization org = new Organization();
                org.name = name.Text;
                org.contactPerson = contactPerson.Text;
                org.email = email.Text;
                org.telephone = telephone.Text;

                if (_org != null)
                    org.id = _org.id;

                return org;
            }
        }

        public OrgWindow()
        {
            InitializeComponent();
            addButton.Visibility = Visibility.Visible;
        }

        public OrgWindow(Organization org, ObservableCollection<Event> events)
        {
            InitializeComponent();
            saveButton.Visibility = Visibility.Visible;
            deleteButton.Visibility = Visibility.Visible;
            eventsGrid.IsEnabled = true;
            _org = org;
            name.Text = org.name;
            contactPerson.Text = org.contactPerson;
            telephone.Text = org.telephone;
            email.Text = org.email;
            eventsGrid.ItemsSource = events;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            IsDeleting = true;
        }
    }
}
