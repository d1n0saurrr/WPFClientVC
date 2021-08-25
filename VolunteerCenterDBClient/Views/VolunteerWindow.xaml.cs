using System;
using System.Windows;
using VolunteerCenterDBClient.Models;

namespace VolunteerCenterDBClient.Views
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        public bool IsDeleting { get; private set; }
        private Volunteer _vol;

        public Volunteer Volunteer
        {
            get
            {
                Volunteer vol = new Volunteer(firstName.Text, lastName.Text, patronymic.Text, dateOfBirth.SelectedDate, sex.Text,
                        citizenship.Text, institute.Text, studyGroup.Text, yearOfGraduation.Text, telephone.Text);
                if (_vol != null)
                    vol.id = _vol.id;

                return vol;
            }
        }

        public VolunteerWindow()
        {
            InitializeComponent();
            addButton.Visibility = Visibility.Visible;
        }

        public VolunteerWindow(Volunteer volunteer)
        {
            InitializeComponent();
            _vol = volunteer;
            firstName.Text = volunteer.firstName;
            lastName.Text = volunteer.lastName;
            patronymic.Text = volunteer.patronymic;
            dateOfBirth.SelectedDate = volunteer.dateOfBirth;
            sex.Text = volunteer.sex;
            citizenship.Text = volunteer.citizenship;
            institute.Text = volunteer.institute;
            studyGroup.Text = volunteer.studyGroup;
            yearOfGraduation.Text = volunteer.yearOfGraduation;
            telephone.Text = volunteer.telephone;
            saveButton.Visibility = Visibility.Visible;
            deleteButton.Visibility = Visibility.Visible;
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
