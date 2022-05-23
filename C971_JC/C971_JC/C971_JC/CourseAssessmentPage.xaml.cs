using C971_JC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_JC
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseAssessmentPage : ContentPage
    {
        //Variable for incoming course object. See constructor
        protected Course _course;

        //Function for setting the labels with appropriate information
        private void SetLabels()
        {
            courseTitleLabel.Text = _course.CourseName;
            courseStartLabel.Text = _course.CourseStart;
            courseEndLabel.Text = _course.CourseEnd;
            courseStatusLabel.Text = _course.CourseStatus;

            if (_course.CourseNote == null || _course.CourseNote == "")
            {
                courseNotesLabel.Text = "No Notes";
            }
            else
            {
                courseNotesLabel.Text = _course.CourseNote;
            }

            if (_course.CourseNotifications == true)
            {
                courseNotificationLabel.Text = "Enabled";
            }
            else
            {
                courseNotificationLabel.Text = "Disabled";
            }
            courseInstructorEmail.Text = _course.InstructorEmail;
            courseInstructorName.Text = _course.InstructorName;
            courseInstructorPhone.Text = _course.InstructorPhone;

        }

        //Constructor
        public CourseAssessmentPage(Course course)
        {
            _course = course;
            InitializeComponent();
            SetLabels();
            
        }

        //Assessment button event listener
        private async void courseAssessments_Clicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new AssessmentPage(_course));
        }

        //Shave button event listener
        private async void courseShareNotes_Clicked(object sender, EventArgs e)
        {
            //Checking if there are notes to begin with
            if (courseNotesLabel.Text == "No Notes")
            {
                await DisplayAlert("Warning", "Please add notes before attempting to share.", "Ok");
            }
            else
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = courseNotesLabel.Text,
                    Title = "My Notes"
                });
            }
        }
    }
}