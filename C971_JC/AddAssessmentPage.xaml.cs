using C971_JC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using C971_JC.Services;
using Plugin.LocalNotifications;

namespace C971_JC
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAssessmentPage : ContentPage
    {
        //Variable for incoming course list. See constructor
        protected Course _course;

        //Function for filling the picker dropdown menu
        private void fillPicker()
        {
            List<string> vs = new List<string>();
            vs.Add("Performance");
            vs.Add("Objective");
            assessmentType.ItemsSource = vs;
        }

        //Constructor
        public AddAssessmentPage(Course course)
        {
            _course = course;
            InitializeComponent();
            fillPicker();
        }

        //Function for checking if a performance or objective assessment already exsits
        private bool checkExistingAssessmentType()
        {
            var result = true;

            if (_course.CourseAssessments.Count == 0)
            {
                result = false;
            }
            else
            {
                for (int i = 0; i < _course.CourseAssessments.Count; i++)
                {
                    if (_course.CourseAssessments[i].AssessmentType == assessmentType.SelectedItem.ToString())
                    {
                        break;
                    }
                    else
                    {
                        result = false;
                    }
                }
                
            }

            return result;
        }

        //Function making sure the form is complete
        private bool isFormComplete()
        {
            var result = false;

            if (assessmentName.Text == null || assessmentName.Text == "")
            {
                return result;
            }
            else if (assessmentType.SelectedItem == null)
            {
                return result;
            }
            else
            {
                result = true;
            }

            return result;
        }

        //Function for checking if start date is before end date
        private bool checkStartDateBeforeEndDate()
        {
            var result = false;
            var start = assessmentStart.Date;
            var end = assessmentEnd.Date;
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

        //Button event listeners
        private async void addAsssessmentSaveButton_Clicked(object sender, EventArgs e)
        {
            //Checking if form is complete
            if (isFormComplete() == false)
            {
                await DisplayAlert("Warning", "Please make sure all fields are complete before submitting.", "Ok");
            }
            else
            {
                //Checking if there more than one of each assessment type and creating assessment
                if (checkExistingAssessmentType() == true)
                {
                    await DisplayAlert("Warning", "There can only be one of each type of assessment. Please change the type before continuing", "Ok");
                }
                else
                {
                    //Cechking if dates are proper
                    if (checkStartDateBeforeEndDate() == false)
                    {
                        await DisplayAlert("Warning", "Please make sure the start date is before the end date before continuing.", "Ok");
                    }
                    else
                    {
                        Assessment assessment = new Assessment
                        {
                            AssessmentName = assessmentName.Text,
                            AssessmentStart = assessmentStart.Date.ToString("MM/dd/yyyy"),
                            AssessmentEnd = assessmentEnd.Date.ToString("MM/dd/yyyy"),
                            AssessmentType = assessmentType.SelectedItem.ToString(),
                            AssessmentNotifications = assessmentNotifications.IsToggled,
                            CourseID = _course.CourseID,
                            NotifID = _course.CourseAssessments.Count
                        };

                        //Setting notifications
                        if (assessmentNotifications.IsToggled == true)
                        {
                            CrossLocalNotifications.Current.Show("Assessment", $"You have an assessment planned for {assessment.AssessmentStart}. Good luck!", assessment.NotifID, DateTime.Parse(assessment.AssessmentStart));
                        }

                        await DatabaseService.Insert(assessment);
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                }
            }
        }

        private async void cancelAddAssessmentButton_Clicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}