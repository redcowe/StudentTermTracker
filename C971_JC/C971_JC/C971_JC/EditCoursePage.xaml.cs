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
    public partial class EditCoursePage : ContentPage
    {
        //Variable for incoming id. See constructor
        protected int _id;

        //Function for filling dropdown picker
        private void fillStatusPicker()
        {
            List<string> vs = new List<string>();
            vs.Add("Completed");
            vs.Add("In-Progress");
            vs.Add("Planning");
            vs.Add("Dropped");

            courseStatus.ItemsSource = vs;
        }

        // Function for filling the form fields with appropriate information
        private async void fillFields()
        {
            //Query the database and getting the correct course
            string query = $"select * from Course where CourseID={_id}";
            var result = await DatabaseService.db.QueryAsync<Course>(query);
            var course = result[0];
            fillStatusPicker();


            //Setting fields
            courseName.Text = course.CourseName;
            courseStart.Date = DateTime.Parse(course.CourseStart);
            courseEnd.Date = DateTime.Parse(course.CourseEnd);
            courseStatus.SelectedItem = course.CourseStatus;
            courseNotes.Text = course.CourseNote;
            courseNotifications.IsToggled = course.CourseNotifications;
            courseInstructorName.Text = course.InstructorName;
            courseInstructorEmail.Text = course.InstructorEmail;
            courseInstructorPhone.Text = course.InstructorPhone;
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

        //Function checking if form is complete
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

        //Constructor
        public EditCoursePage(int id)
        {
            _id = id;
            InitializeComponent();
            fillFields();
        }

        //Save button event listener
        private async void courseSaveButton_Clicked(object sender, EventArgs e)
        {
            //Checking if all fields are complete
            if (isFormComplete() == false)
            {
                await DisplayAlert("Warning", "Please make sure all fields are complete before submitting. Notes are optional", "Ok");
            }
            else
            {
                //Checking that dates are correct
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
                            //Variables for different fields and the query string
                            var cname = courseName.Text;
                            var start = courseStart.Date.ToString("MM/dd/yyyy");
                            var end = courseEnd.Date.ToString("MM/dd/yyyy");
                            var status = courseStatus.SelectedItem.ToString();
                            var notifs = courseNotifications.IsToggled;
                            var notes = courseNotes.Text;
                            var iname = courseInstructorName.Text;
                            var iphone = courseInstructorPhone.Text;
                            var iemail = courseInstructorEmail.Text;
                            string query =
                                 $"update Course set CourseName='{cname}', CourseStart='{start}', CourseEnd='{end}', CourseStatus='{status}', CourseNotifications={notifs}, CourseNote='{notes}', InstructorName='{iname}', InstructorPhone='{iphone}', InstructorEmail='{iemail}' where CourseID={_id}";

                            //Updating DB
                            await DatabaseService.db.QueryAsync<Course>(query);

                            await Application.Current.MainPage.Navigation.PopAsync();

                            if (notifs == true)
                            {
                                CrossLocalNotifications.Current.Show("Course", $"You have a course coming up on {start}. Good luck!", _id, DateTime.Parse(start));

                            }
                        }                        
                    } 
                }
            } 
        }
    }
}