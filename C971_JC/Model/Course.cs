using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SQLite;


namespace C971_JC.Model
{
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int CourseID { get; set; }

        public string CourseName { get; set; }

        public string CourseStart { get; set; }

        public string CourseEnd { get; set; }

        public string CourseStatus { get; set; }

        public string CourseNote { get; set; }

        public bool CourseNotifications { get; set; }

        public string InstructorName { get; set; }

        public string InstructorPhone { get; set; }

        public string InstructorEmail { get; set; }

        public int TermID { get; set; }

        private ObservableCollection<Assessment> courseassessments = new ObservableCollection<Assessment>();

        [Ignore]
        public int NotifID { get; set; } 

        [Ignore]
        public ObservableCollection<Assessment> CourseAssessments
        {
            set { courseassessments = value; }
            get { return courseassessments; }
        }

    }
}
