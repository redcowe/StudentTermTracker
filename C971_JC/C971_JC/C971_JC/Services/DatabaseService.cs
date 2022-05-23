using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SQLite;
using System.Threading.Tasks;
using Xamarin.Essentials;
using C971_JC.Model;

namespace C971_JC.Services
{
    public static class DatabaseService
    {
        public static SQLiteAsyncConnection db;

        public static async Task Init()
        {

            //Creating DB and tables

            if (db != null)
                return;

            var dbTermPath = Path.Combine(FileSystem.AppDataDirectory, "MyDB.db");

            db = new SQLiteAsyncConnection(dbTermPath);
            
            
            await db.CreateTableAsync<Term>();
            await db.CreateTableAsync<Course>();
            await db.CreateTableAsync<Assessment>();
  
        } 
        

        //Assessments insert overload
        public static async Task Insert(Assessment assessment)
        {
            await Init();

            await db.InsertAsync(assessment);
        }

        //Course insert overload
        public static async Task Insert(Course course)
        {
            await Init();

            await db.InsertAsync(course);
        }

        //Term insert overload
        public static async Task Insert(Term term)
        {
            await Init();

            try
            {
                
                var id = await db.InsertAsync(term);
                var count = await db.Table<Term>().CountAsync();
                _ = count;
            } 
            catch (SQLiteException)
            {
                throw;
                
            }
        }

        //Function to get all records in Terms table
        public static async Task<IEnumerable<Term>> GetTerms()
        {
            await Init();
            var terms = await db.Table<Term>().ToListAsync();
            return terms;
        }

        //Function to get all records in Courses table 

        public static async Task<IEnumerable<Course>> GetCourses()
        {
            await Init();
            var courses = await db.Table<Course>().ToListAsync();
            return courses;
        }

        public static async Task<IEnumerable<Assessment>> GetAssessments()
        {
            await Init();
            var assessments = await db.Table<Assessment>().ToListAsync();
            return assessments;
        }


        public static async Task RemoveTerm(int id)
        {
            await Init();
            await db.DeleteAsync<Term>(id);
        }

        public static async Task RemoveCourse(int id)
        {
            await Init();
            await db.DeleteAsync<Course>(id);
        }

        public static async Task RemoveAssessment(int id)
        {
            await Init();
            await db.DeleteAsync<Assessment>(id);
        }
    }
}
