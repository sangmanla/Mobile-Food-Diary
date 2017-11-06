using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Database.Sqlite;
using static Android.Database.Sqlite.SQLiteDatabase;
using System.IO;
using SQLite;

namespace FoodDiary {
    public class MyDBHelper : SQLiteOpenHelper {
        private static int VERSION = 1;
        private Context context = null;
        SQLiteDatabase db;

        public MyDBHelper(Context _context) : base(_context, MyUtil.DATABASE_FILE_NAME, null, VERSION)
        {
            db = WritableDatabase;
            this.context = _context;
        }

        public override void OnCreate(SQLiteDatabase db) {
            try {
                //db.ExecSQL(@"
                //        CREATE TABLE IF NOT EXISTS MEAL (
                //            Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                //            FirstName       TEXT NOT NULL,
                //            LastName        TEXT NOT NULL )");

                var connection = new SQLiteAsyncConnection(MyUtil.DB_PATH);
                connection.CreateTableAsync<Meal>();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
            //if (oldVersion < 2) {
            //    //perform any database upgrade tasks for versions prior to  version 2              
            //}
            //if (oldVersion < 3) {
            //    //perform any database upgrade tasks for versions prior to  version 3
            //}
            OnCreate(db);
        }
    }
}