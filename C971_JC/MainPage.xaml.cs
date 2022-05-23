using C971_JC.Model;
using C971_JC.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace C971_JC
{
    public partial class MainPage : ContentPage
    {
        //Variable declaration for the list of terms that appear in UI
        private ObservableCollection<Term> _terms;

        //Constructor
        public MainPage()
        {
            InitializeComponent();
            InitializeDB();
            PreloadTerms();
            Refresh();
            
        }

        //Function for DB initalization
        private async void InitializeDB()
        {
            await DatabaseService.Init();
        }

        //Function to refresh the display
        public void Refresh()
        {
            _terms = new ObservableCollection<Term>();
            var count = DatabaseService.db.Table<Term>().ToListAsync().Result.Count;
            for (int i = 0; i < count; i++)
            {
                _terms.Add(DatabaseService.db.Table<Term>().ToListAsync().Result[i]);
            }

            termView.ItemsSource = _terms;
        }


        //Hardcoding data into the app
        private async void PreloadTerms()
        {

            Term term = new Term
            {
                TermID = 1,
                TermName = "Spring term",
                TermStart = "04/01/2022",
                TermEnd = "10/01/2022",
                
                
            };

            Course course = new Course
            {
                CourseID = 1,
                CourseName = "Software II",
                CourseStart = "04/01/2022",
                CourseEnd = "05/01/2022",
                CourseNote = "Notes",
                CourseStatus = "Completed",
                CourseNotifications = false,
                InstructorEmail = "jcowel7@wgu.edu",
                InstructorName = "Joshua Cowell",
                InstructorPhone = "757-290-7017",
                TermID = 1
            };

            term.TermCourses.Add(course);

            Assessment pa = new Assessment
            {
                AssessmentID = 1,
                AssessmentName = "B103 Software II",
                AssessmentStart= "04/01/2022",
                AssessmentEnd = "05/01/2022",
                AssessmentNotifications = false,
                AssessmentType = "Performance",
                CourseID = course.CourseID
               
            };

            Assessment oa = new Assessment
            {
                AssessmentID = 2,
                AssessmentName = "D810 Software I",
                AssessmentStart = "03/01/2022",
                AssessmentEnd = "03/15/2022",
                AssessmentNotifications = false,
                AssessmentType = "Objective",
                CourseID = course.CourseID

            };

            course.CourseAssessments.Add(pa);
            course.CourseAssessments.Add(oa);
            
            await DatabaseService.db.InsertOrReplaceAsync(term);
            await DatabaseService.db.InsertOrReplaceAsync(course);
            await DatabaseService.db.InsertOrReplaceAsync(pa);
            await DatabaseService.db.InsertOrReplaceAsync(oa);
            
        }

       
        //Add button event listener
        protected async void  Button_Clicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new AddTermPage());
        }

        //Delete button event listener
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync("Delete", "Please enter an ID");

            if (result == null)
                return;

            int id = Convert.ToInt32(result);

            await DatabaseService.RemoveTerm(id);

            Refresh();
        }

        //Function for pull down refresh
        private void termView_Refreshing(object sender, EventArgs e)
        {
            Refresh();
            termView.EndRefresh();
        }

        //Edit button
        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            var result = await DisplayPromptAsync("Edit", "Please enter an ID");

            if (result == null)
                return;

            int id = Convert.ToInt32(result);

            //Passing in the id to use in EditTerms
            await Application.Current.MainPage.Navigation.PushAsync(new EditTerms(id));
        }

        //Item tapped event
        private async void termView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var term = e.Item as Term;
            await Application.Current.MainPage.Navigation.PushAsync(new TermCoursesPage(term));
        }
    }
}
