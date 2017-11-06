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
using FFImageLoading;

namespace fooddiary {
    public class MainListAdaptor : BaseAdapter<PickedMeal>{
        List<PickedMeal> items;
        View globalView;
        public bool type = false;  // true up table, false below table
        bool[] isSelected = null;

        public void SetSelected(int idx) {
            if (isSelected == null) {
                this.isSelected = new bool[items.Count];
                for (int i = 0; i < items.Count; i++) this.isSelected[i] = false;
            }

            for (int i = 0; i < items.Count; i++) this.isSelected[i] = false;
            this.isSelected[idx] = true;

        }

        public MainListAdaptor(View view, List<PickedMeal> items)
       : base() {
            this.globalView = view;
            this.items = items;

            this.isSelected = new bool[items.Count];
            for (int i = 0; i < items.Count; i++) this.isSelected[i] = false;
        }

        public override long GetItemId(int position) {
            return position;
        }
        public override PickedMeal this[int position] {
            get { return items[position]; }
        }
        public override int Count {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent) {
            View view = convertView;
            if (view == null) {
                LayoutInflater inflater = LayoutInflater.From(globalView.Context);
                view = (View)inflater.Inflate(Resource.Layout.MainListLayout, null, false);
            }

            if (this.items.Count == 1 && items[0].Name == MyUtil.NO_DATA) {
                LayoutInflater inflater = LayoutInflater.From(view.Context);
                View layout = (View)inflater.Inflate(Resource.Layout.CustomListLayout, null, false);

                view.FindViewById<TextView>(Resource.Id.listName).Text = "No data.";
                view.FindViewById<ImageView>(Resource.Id.listImage).SetImageURI(null);

                return view;
            } else {
                var item = items[position];

                if (isSelected[position] == true) view.SetBackgroundColor(Color.LightGray);
                else view.SetBackgroundColor(Color.Transparent);

                view.FindViewById<TextView>(Resource.Id.listName).Text = item.Name;
                view.FindViewById<RatingBar>(Resource.Id.mainRatingBar).Rating = item.Rate;

                if (type) view.FindViewById<TextView>(Resource.Id.recentDate).Text = item.CreateDate.ToString("yyyy-MM-dd");
                else view.FindViewById<TextView>(Resource.Id.recentDate).Text = item.Count + " times";

                if (!string.IsNullOrWhiteSpace(item.ImageName)) {
                    FFImageLoading.Views.ImageViewAsync imageView = view.FindViewById<FFImageLoading.Views.ImageViewAsync>(Resource.Id.listImage);
                    //imageView.SetImageBitmap(MyUtil.GetThumbnailBitmap(item.ImageName, false));

                    ImageService.Instance
                        .LoadFile(System.IO.Path.Combine(PageRegisterFragment.documentsDirectory, item.ImageName))
                        .DownSample(width:100)
                        .LoadingPlaceholder("loading.png", FFImageLoading.Work.ImageSource.CompiledResource)
                        .ErrorPlaceholder("error.png", FFImageLoading.Work.ImageSource.CompiledResource)
                        .Into(imageView);
                }

                return view;
            }
        }
    }
}