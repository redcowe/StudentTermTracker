using C971_JC.Model;
using C971_JC.Services;
using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_JC
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAssessmentPage : ContentPage
    {
        //Variable for incoming id. See constructor
        protected int _id;

        //Function to fill the dropdown picker
        private void fillPicker()
        {
            List<string> vs = new List<string>();
            vs.Add("Performance");
            vs.Add("Objective");
            assessmentType.ItemsSource = vs;
        }
        //Function to set fields on page
        private  async void setFields()
        {
            //Selecting the assessment and filing fields
            string query = $"select * from Assessment where AssessmentID={_id}";
            var result = await DatabaseService.db.QueryAsync<Assessment>(query);
            Assessment assessment = result[0];
            fillPicker();

            assessmentName.Text = assessment.AssessmentName;
            assessmentStart.Date = DateTime.Parse(assessment.AssessmentStart);
            assessmentEnd.Date = DateTime.Parse(assessment.AssessmentEnd);
            assessmentType.SelectedItem = assessment.AssessmentType;
            assessmentNotifications.IsToggled = assessment.AssessmentNotifications;
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

        //Constructor
        public EditAssessmentPage(int id)
        {
            _id = id;
            InitializeComponent();
            setFields();
        }

        //Button click events
        private  async void addAsssessmentSaveButton_Clicked(object sender, EventArgs e)
        {
            //Checking if form is complete
            if (isFormComplete() == false)
            {
                await DisplayAlert("Warning", "Please make sure all fields are complete before submitting.", "Ok");
            }
            else
            {
                //checking if dates are correct
                if (checkStartDateBeforeEndDate() == false)
                {
                    await DisplayAlert("Warning", "Please make sure the start date is before the end date before continuing", "OK");
                }
                else
                {
                    //Grabbing values and updating db
                    var name = assessmentName.Text;
                    var start = assessmentStart.Date.ToString("MM/dd/yyyy");
                    var end = assessmentEnd.Date.ToString("MM/dd/yyyy");
                    var type = assessmentType.SelectedItem.ToString();
                    var notifs = assessmentNotifications.IsToggled;

                    string query = $"update Assessment set AssessmentName='{name}', AssessmentStart='{start}', AssessmentEnd='{end}', AssessmentType='{type}', AssessmentNotifications={notifs} where AssessmentID={_id}";
                    await DatabaseService.db.QueryAsync<Assessment>(query);
                    await Application.Current.MainPage.Navigation.PopAsync();

                    if (notifs == true)
                    {
                        CrossLocalNotifications.Current.Show("Asessment", $"You have an assessment planned for {start}. Good luck!", _id, DateTime.Parse(start));
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