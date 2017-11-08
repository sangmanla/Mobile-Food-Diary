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
using Java.Lang;
using SQLite;
using System.Globalization;

namespace mealdiary {
    public class Meal {
        public Meal() {

        }
        public Meal(int id) {
            this.ID = id;
        }
        public Meal(string name, int rate, int type, string imageName, string comment) {
            Name = name;
            Rate = rate;
            Type = type;
            ImageName = imageName;
            Comments = comment;
            CreateDate = DateTime.Now;
        }

        public Meal(string name, int rate, int type, string imageName, string comment, string createDate) {
            Name = name;
            Rate = rate;
            Type = type;
            ImageName = imageName;
            Comments = comment;
            CreateDate = DateTime.ParseExact(createDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }



        public Meal(string name, int rate, int type, string imageName, string comment, DateTime dt) {
            Name = name;
            Rate = rate;
            Type = type;
            ImageName = imageName;
            Comments = comment;
            CreateDate = dt;
        }

        public Meal(int id, string name, int rate, int type, string imageName, string comment, DateTime dt) {
            ID = id;
            Name = name;
            Rate = rate;
            Type = type;
            ImageName = imageName;
            Comments = comment;
            CreateDate = dt;
        }

        public Meal getMeal(int id) {
            return null;
        }
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public int Rate { get; set; }
        public int Type { get; set; }
        public string ImageName { get; set; }
        public string Comments { get; set; }
        public DateTime CreateDate { get; set; }

        public Meal CopyMeal() {
            return new Meal(ID, Name, Rate, Type, ImageName, Comments, CreateDate);
        }

        public static explicit operator Meal(Java.Lang.Object v) {
            throw new NotImplementedException();
        }
    }
}