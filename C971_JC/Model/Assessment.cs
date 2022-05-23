using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace C971_JC.Model
{
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int AssessmentID { get; set; }

        public string AssessmentName { get; set; }

        public string AssessmentStart { get; set; }

        public string AssessmentEnd { get; set; }

        public string AssessmentType { get; set; }

        public bool AssessmentNotifications { get; set; }

        public int CourseID { get; set; }

        [Ignore]

        public int NotifID { get; set; }

    }
}
