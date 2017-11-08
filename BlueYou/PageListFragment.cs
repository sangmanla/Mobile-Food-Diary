using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using SQLite;
using System.Collections.Generic;
using Java.Interop;

using System.Globalization;
using Android.Views.InputMethods;
using Android.App;
using Android.Graphics;
using FFImageLoading;

namespace mealdiary {
    public class PageListFragment : MyFragment{
        protected List<Meal> list;
        public static bool fromList = false;
        public static Meal meal = null;
        public override View GetCurrentView(LayoutInflater inflater, ViewGroup container) {
            return inflater.Inflate(Resource.Layout.ListFragment, container, false); ;
        }

        public async override void OnViewCreated(View view, Bundle savedInstanceState) {
            base.OnViewCreated(view, savedInstanceState);
            SetAppearance();
            SetSearchEventHandler();
            await GetList();
        }

        private void SetSearchEventHandler() {
            SearchView searchView = FindViewById<SearchView>(Resource.Id.searchView);
            searchView.QueryTextSubmit += _searchView_QueryTextSubmit;
            searchView.Click += focusSearchView;
        }

        private void focusSearchView(object sender, EventArgs e) {
            SearchView searchView = (SearchView)sender;
            searchView.Focusable = true;
            searchView.Iconified = false;
            searchView.RequestFocusFromTouch();
        }

        private async void _searchView_QueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e) {
            InputMethodManager imm = (InputMethodManager)Context.GetSystemService("input_method");
            imm.HideSoftInputFromWindow(((SearchView)sender).WindowToken, HideSoftInputFlags.None);
            e.Handled = true;

            await GetList();
        }

        private void SetAppearance() {
            SetHintToSearchView();
        }

        private async System.Threading.Tasks.Task GetList() {
            string name = FindViewById<SearchView>(Resource.Id.searchView).Query;
            try {
                string sql = " SELECT *  " +
                        " FROM MEAL " +
                        " WHERE NAME LIKE '%" + name + "%' " +
                        " ORDER BY CREATEDATE DESC ";
                var db = new SQLiteAsyncConnection(MyUtil.DB_PATH);
                list = await db.QueryAsync<Meal>(sql);

                ListView lv = FindViewById<ListView>(Resource.Id.searchList);
                lv.Adapter = new ListAdaptor(view, list);
                lv.ItemClick += OnListItemClick;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        protected void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e) {
            ListView listView = sender as ListView;
            listView.Focusable = true;
            listView.RequestFocusFromTouch();

            for (int i=0;i< listView.ChildCount; i++) listView.GetChildAt(i).SetBackgroundColor(Color.Transparent);
            e.View.SetBackgroundColor(Color.Orange);

            meal = list[e.Position];

            // custom dialog
            if (dialog == null) dialog = new Dialog(view.Context);
            dialog.SetContentView(Resource.Layout.ViewInformation);
            dialog.FindViewById<TextView>(Resource.Id.viewComments).Text = meal.Comments;
            dialog.FindViewById<TextView>(Resource.Id.viewFoodName).Text = meal.Name;
            dialog.FindViewById<RatingBar>(Resource.Id.viewRatingBar).Rating = meal.Rate;
            dialog.FindViewById<TextView>(Resource.Id.viewMealType).Text = meal.CreateDate.ToString("yyyy-MM-dd") + " " + MyUtil.GetType(meal);
            if (!string.IsNullOrWhiteSpace(meal.ImageName)) {
                FFImageLoading.Views.ImageViewAsync imageView = dialog.FindViewById<FFImageLoading.Views.ImageViewAsync>(Resource.Id.viewImageView);

                ImageService.Instance
                        .LoadFile(System.IO.Path.Combine(PageRegisterFragment.documentsDirectory, meal.ImageName))
                        .DownSampleInDip(width:250)
                        .LoadingPlaceholder("loading.png", FFImageLoading.Work.ImageSource.CompiledResource)
                        .ErrorPlaceholder("error.png", FFImageLoading.Work.ImageSource.CompiledResource)
                        .Into(imageView);
            }

            dialog.FindViewById<Button>(Resource.Id.goModifyBtn).Click += GoModifyPage;
            dialog.FindViewById<Button>(Resource.Id.deleteBtn).Click += DeleteCurrentItem;
            dialog.CancelEvent += closeDialog;

            dialog.Show();
        }

        public void GoModifyPage(object sender, EventArgs e) {
            fromList = true;
            MyUtil.GoPage(this, PAGE.REGI);
            dialog.Dismiss();
        }

        public async void DeleteCurrentItem(object sender, EventArgs e) {
            string result = await MyUtil.DeleteItem(meal.ID);
            Toast.MakeText(this.Context, "Delete successfully!", ToastLength.Short).Show();
            dialog.Dismiss();
            await GetList();
        }

        private void closeDialog(object sender, EventArgs e) {
            dialog.Dismiss();
        }

        private void SetHintToSearchView() {
            SearchView search = FindViewById<SearchView>(Resource.Id.searchView);
            search.SetQueryHint("Name");
        }
    }
}