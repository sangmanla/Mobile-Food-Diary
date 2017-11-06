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
using Android;
using System.IO;
using Android.Graphics;
using SQLite;

namespace FoodDiary {
    public class ListAdaptor : BaseAdapter<Meal> {
        public List<Meal> items;
        View globalView;
        bool[] isSelected = null;

        public void SetSelected(int idx) {
            if (isSelected == null) {
                this.isSelected = new bool[items.Count];
                for (int i = 0; i < items.Count; i++) this.isSelected[i] = false;
            }

            for (int i = 0; i < items.Count; i++) this.isSelected[i] = false;
            this.isSelected[idx] = true;

        }
        public ListAdaptor(View view, List<Meal> items)
       : base() {
            this.items = items;
            this.globalView = view;

            this.isSelected = new bool[items.Count];
            for (int i = 0; i < items.Count; i++) this.isSelected[i] = false;
        }

        public override long GetItemId(int position) {
            return position;
        }
        public override Meal this[int position] {
            get { return items[position]; }
        }
        public override int Count {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent) {
            View view = convertView;
            if (view == null) {
                LayoutInflater inflater = LayoutInflater.From(globalView.Context);
                view = (View)inflater.Inflate(Resource.Layout.CustomListLayout, null, false);
            }

            var item = items[position];
            view.FindViewById<TextView>(Resource.Id.listName).Text = item.Name;
            view.FindViewById<RatingBar>(Resource.Id.listRate).Rating = item.Rate;
            view.FindViewById<TextView>(Resource.Id.listDate).Text = "" + MyUtil.GetType(item) + "  " + item.CreateDate.ToString("yyyy-MM-dd");

            if (isSelected[position] == true) view.SetBackgroundColor(Color.Orange);
            else view.SetBackgroundColor(Color.Transparent);

            return view;
        }
    }
}