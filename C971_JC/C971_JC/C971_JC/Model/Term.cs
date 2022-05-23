using SQLite;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace C971_JC.Model
{
    public class Term
    {
        [PrimaryKey, AutoIncrement]
        public int TermID { get; set; }
        public string TermName { get; set; }
        public string TermStart { get; set; }
        public string TermEnd { get; set; }

        private ObservableCollection<Course> termcourses = new ObservableCollection<Course>();

        [Ignore]
        public ObservableCollection<Course> TermCourses
        {
            set { termcourses = value; }
            get { return termcourses; }
        }
    }
}
