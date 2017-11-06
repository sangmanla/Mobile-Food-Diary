using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace fooddiary {
    public abstract class MyFragment : Fragment {
        protected Dialog dialog = null;
        
        public View view;
        public static string PAGE_CONTENT = "PAGE_CONTENT";

        public override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            view = GetCurrentView(inflater, container);

            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState) {
            base.OnViewCreated(view, savedInstanceState);
            LinkEvent();
        }

        public virtual void LinkEvent() {

        }

        public T FindViewById<T>(int id) where T : View {
            return view.FindViewById<T>(id);
        }

        public View FindViewById(int id) {
            return view.FindViewById(id);
        }

        public abstract View GetCurrentView(LayoutInflater inflater, ViewGroup container);
    }
}