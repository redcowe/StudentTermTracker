using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C971_JC.Model;
using C971_JC.Services;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_JC
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTerms : ContentPage
    {
        //Variable for incoming id. See constructor
        protected int _id;

        //Function for setting fields
        private async void SetFields()
        {
            //finding the selected term
            var result = await DatabaseService.db.QueryAsync<Term>($"select * from Term where TermID={_id}");

            var term = result[0];

            termName.Text = term.TermName;
            termStart.Date = DateTime.Parse(term.TermStart);
            termEnd.Date = DateTime.Parse(term.TermEnd);
        }

        //Function for checking if start date is before end date
        private bool checkStartDateBeforeEndDate()
        {
            var result = false;
            var start = termStart.Date;
            var end = termEnd.Date;
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


        //Function for checking if form is complete
        private bool isFormComplete()
        {
            var result = false;

            if (termName.Text == "" || termName.Text == null)
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
        public EditTerms(int id)
        {
            _id = id;
            InitializeComponent();    
            SetFields();
        }

        //Cancel button
        private void Button_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }

        //Save button
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            //Checking if form is complete
            if (isFormComplete() == false)
            {
                await DisplayAlert("Warning", "Please make sure all fields are complete before submitting.", "Ok");
            }
            else
            {
                //Checking if dates are right
                if (checkStartDateBeforeEndDate() == false)
                {
                    await DisplayAlert("Warning", "Please make sure the start date is before the end date before continuing.", "OK");
                }
                else
                {
                    var name = termName.Text;
                    var start = termStart.Date.ToString("MM/dd/yyyy");
                    var end = termEnd.Date.ToString("MM/dd/yyyy");

                    var query = $"update Term set TermName='{name}', TermStart='{start}', TermEnd='{end}' where TermID={_id}";

                    await DatabaseService.db.QueryAsync<Term>(query);
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
            }
        }
    }
}