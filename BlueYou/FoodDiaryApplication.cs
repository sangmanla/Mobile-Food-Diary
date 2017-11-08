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

namespace mealdiary {
    [Application]
    public class MealDiaryApplication : Application {

        public MealDiaryApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) {
        }

        public override void OnCreate() {
            base.OnCreate();

            //This method needs to be called before any database calls can be made!
            DatabaseUpdates mydata = new DatabaseUpdates();
            mydata.SetContext(this);
        }
    }

    public class DatabaseUpdates {
        private MyDBHelper _helper;

        public void SetContext(Context context) {
            if (context != null) {
                _helper = new MyDBHelper(context);
            }
        }
    }
}