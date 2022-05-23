using C971_JC.Model;
using C971_JC.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_JC
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermCoursesPage : ContentPage
    {
        //Declarations for incoming courses list and term object. See constructor
        protected ObservableCollection<Course> _courses;
        protected Term _term;

        public TermCoursesPage(Term term)
        {
            _term = term;
            _courses = new ObservableCollection<Course>();
            _courses = term.TermCourses;
            InitializeComponent();
            SetPage();
            Refresh();
        }
        
        //Function for refreshing the View
        private async void Refresh()
        {
            //Clearing the list if anything was in there
            _courses.Clear();

            //query for selecting the proper Classes
            string query = $"select * from Course where TermID={_term.TermID}";
            var courses = await DatabaseService.db.QueryAsync<Course>(query);

            for (int i = 0; i < courses.Count; i++)
            {
                _courses.Add(courses[i]);
            }

            courseView.ItemsSource = _courses;
        }

        //Function for setting the title, items, etc
        private void SetPage()
        {
            var termTitle = $"{_term.TermName}: {_term.TermStart} - {_term.TermEnd}";
            termNameLabel.Text = termTitle;
        }

        private void courseView_Refreshing(object sender, EventArgs e)
        {
            Refresh();
            courseView.EndRefresh();
        }

        //Button and list item click event listeners

        private async void addCourseButton_Clicked(object sender, EventArgs e)
        {
            if (_term.TermCourses.Count == 6)
            {
                await DisplayAlert("Warning", "A term can have a maximum of 6 courses. Please delete a course before continuing", "OK");
            }
            else
            {
                await Application.Current.MainPage.Navigation.PushAsync(new AddCoursePage(_term));
            }
                
        }

        private async void deleteCourseButton_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync("Delete", "Please enter an id");

            if (result == null)
                return;

            int id = Convert.ToInt32(result);
            await DatabaseService.RemoveCourse(id);
            Refresh();
        }

        private async void editCourseButton_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync("Edit", "Please enter an id");

            if (result == null)
                return;

            int id = Convert.ToInt32(result);
            await Application.Current.MainPage.Navigation.PushAsync(new EditCoursePage(id));
        }

        private async void courseView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var course = e.Item as Course;

            await Application.Current.MainPage.Navigation.PushAsync(new CourseAssessmentPage(course));
        }
    }
}