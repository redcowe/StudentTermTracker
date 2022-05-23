using C971_JC.Model;
using C971_JC.Services;
using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_JC
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCoursePage : ContentPage
    {
        //Variable for incoming Term object. See constructor
        protected Term _term;

        //Filling the dropdown picker in the UI
        private void fillStatusPicker()
        {
            List<string> vs = new List<string>();
            vs.Add("Completed");
            vs.Add("In-Progress");
            vs.Add("Planning");
            vs.Add("Dropped");

            courseStatus.ItemsSource = vs;
        }

        private bool isFormComplete()
        {
            var result = false;

            if (courseName.Text == "" || courseName.Text == null)
            {
                return result;
            }
            else if (courseStatus.SelectedItem == null)
            {
                return result;
            }
            else if (courseInstructorEmail.Text == "" || courseInstructorEmail.Text == null)
            {
                return result;
            }
            else if (courseInstructorName.Text == "" || courseInstructorName.Text == null)
            {
                return result;
            }
            else if (courseInstructorPhone.Text == "" || courseInstructorPhone.Text == null)
            {
                return result;
            }
            else
            {
                result = true;
            }

            return result;
        }

        //Functions for validating dates, email, etc
        private bool checkStartDateBeforeEndDate()
        {
            var result = false;
            var start = courseStart.Date;
            var end = courseEnd.Date;
            if (end < start)
            {
                result = false;
            }
            else
            {
                result = true;
            }

            return result;
        }

        private bool isValidEmail()
        {
            var result = false;

            try
            {
                MailAddress mailAddress = new MailAddress(courseInstructorEmail.Text);
                result = true;
            }
            catch (FormatException)
            {
                return result;
            }

            return result;
        }

        private bool isValidPhoneNumber()
        {
            var result = false;


            try
            {
                var r = new Regex(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$");

                if (r.IsMatch(courseInstructorPhone.Text))
                {
                    result = true;
                }
                else
                {
                    return result;
                }

            }
            catch (Exception)
            {
                return result;
            }

            return result;
        }


        //Constructor
        public AddCoursePage(Term term)
        {
            _term = term;
            InitializeComponent();
            fillStatusPicker();
        }
        //Button Event listener
        private async void courseSaveButton_Clicked(object sender, EventArgs e)
        {
            //Checking if form is complete
            if (isFormComplete() == false)
            {
                await DisplayAlert("Warning", "Please make sure all fields are complete before submitting. Notes are optional.", "Ok");
            }
            else
            {
                //Checking id dates are correct
                if (checkStartDateBeforeEndDate() == false)
                {
                    await DisplayAlert("Warning", "Please make sure the start date is before the end date before continuing.", "OK");
                }
                else
                {
                    //Checking if email is valid
                    if (isValidEmail() == false)
                    {
                        await DisplayAlert("Warning", "Please make sure the email is valid.", "OK");
                    }
                    else
                    {
                        //Checking if phone is valid
                        if (isValidPhoneNumber() == false)
                        {
                            await DisplayAlert("Warning", "Please make sure the phone number is valid.", "OK");
                        }
                        else
                        {
                            var notifs = courseNotifications.IsToggled ? true : false;
                            Course course = new Course
                            {

                                CourseName = courseName.Text,
                                CourseStart = courseStart.Date.ToString("MM/dd/yyyy"),
                                CourseEnd = courseEnd.Date.ToString("MM/dd/yyyy"),
                                CourseStatus = courseStatus.SelectedItem.ToString(),
                                CourseNote = courseNotes.Text,
                                CourseNotifications = notifs,
                                InstructorEmail = courseInstructorEmail.Text,
                                InstructorName = courseInstructorName.Text,
                                InstructorPhone = courseInstructorPhone.Text,
                                TermID = _term.TermID,
                                NotifID = _term.TermCourses.Count
                            };

                            //Setting Notifications
                            if (course.CourseNotifications == true)
                            {
                                CrossLocalNotifications.Current.Show("Course", $"You have a course planned to start on {course.CourseStart}. Good luck!", course.NotifID, DateTime.Parse(course.CourseStart));
                            }
                            await DatabaseService.Insert(course);
                            await Application.Current.MainPage.Navigation.PopAsync();
                        }
                    } 
                }
            }
        }
    }
}