using C971_JC.Model;
using C971_JC.Services;
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
    public partial class AddTermPage : ContentPage
    {
        //Constructor
        public AddTermPage()
        {
            InitializeComponent();
        }


        //Cancel button event listener
        private void Button_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }

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

        //Save button event listeners
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            // checking if form is complete
            if (isFormComplete() == false)
            {
                await DisplayAlert("Warning", "Please make sure all fields are complete before submitting.", "Ok");
            }
            else
            {
                //checking if dates are proper
                if (checkStartDateBeforeEndDate() == false)
                {
                    await DisplayAlert("Warning", "Please make sure the start date is before the end date before continuing.", "OK");
                }
                else
                {
                    Term term = new Term
                    {
                        TermName = termName.Text,
                        TermStart = termStart.Date.ToString("MM/dd/yyyy"),
                        TermEnd = termEnd.Date.ToString("MM/dd/yyyy")
                    };

                    await DatabaseService.Insert(term);
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
            }   
        }
    }
}