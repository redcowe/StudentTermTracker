using C971_JC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using C971_JC.Services;

namespace C971_JC
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentPage : ContentPage
    {
        //Variables for incoming assessments and course objects/lists. See constructor
        protected ObservableCollection<Assessment> _assessments;
        protected Course _course;

        //Constructor
        public AssessmentPage(Course course)
        {
            _assessments = new ObservableCollection<Assessment>();
            _assessments = course.CourseAssessments;
            _course = course;
            InitializeComponent();
            Refresh();
            
        }


        //Function for keeping the UI in sync with the DB
        private async void Refresh()
        {
            _assessments.Clear();
            string query = $"select * from Assessment where CourseID={_course.CourseID}";
            var assessments = await DatabaseService.db.QueryAsync<Assessment>(query);
            for (int i = 0; i < assessments.Count; i++)
            {
                _assessments.Add(assessments[i]);
            }
            assessmentView.ItemsSource = _assessments;
        }


        //Button and screen event listeners
        private async void addAssessmentButton_Clicked(object sender, EventArgs e)
        {
            if (_course.CourseAssessments.Count == 2)
            {
                await DisplayAlert("Warning", "A course can only have 2 assessments. Please delete an assessment before continuing.", "Ok");
            }
            else
            {
                await Application.Current.MainPage.Navigation.PushAsync(new AddAssessmentPage(_course));
            }
        }

        private async void deleteAssessmentButton_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync("Delete", "Please enter an id");

            if (result == null)
                return;

            int id = Convert.ToInt32(result);

            await DatabaseService.RemoveAssessment(id);

            Refresh();
        }

        private async void editAssessmentButton_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync("Edit", "Please enter an ID");

            if (result == null)
                return;

            int id = Convert.ToInt32(result);
            await Application.Current.MainPage.Navigation.PushAsync(new EditAssessmentPage(id));
        }

        private void assessmentView_Refreshing(object sender, EventArgs e)
        {
            Refresh();
            assessmentView.EndRefresh();
        }
    }
}