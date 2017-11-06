using Android.App;
using Android.Views;
using Android.OS;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Content;
using Android.Util;
using Android.Widget;
using Android.Graphics;
using Android.Views.InputMethods;

namespace FoodDiary {
    [Activity(Label = "FoodDiary", MainLauncher = true, Theme = "@style/MyTheme")] // , Icon = "@drawable/icon", 
    public class MyAppCompatActivity : AppCompatActivity {
        public static PAGE CURRENT_PAGE = PAGE.HOME;
        protected DrawerLayout drawerLayout;
        protected NavigationView navigationView;
        public ActionBarDrawerToggle mDrawerToggle;
        protected override void OnCreate(Bundle savedInstanceState) {
            
            base.OnCreate(savedInstanceState);
            base.SetContentView(Resource.Layout.BaseLayout);
            SetMenuInfo();
            SetContent();
        }

        public override void OnBackPressed() {
            if (CURRENT_PAGE == PAGE.HOME) base.OnBackPressed();
            else MyUtil.GoPage(FragmentManager.FindFragmentByTag<MyFragment>(MyFragment.PAGE_CONTENT), PAGE.HOME);
        }

        private void SetContent() {
            FragmentTransaction ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.page_placeholder, new PageMainFragment(), MyFragment.PAGE_CONTENT);
            ft.Commit();
        }

        protected void SetMenuInfo() {
            var toolbar = FindViewById<MyCustomToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetHomeButtonEnabled(true);

            toolbar.SetNavigationIcon(Resource.Drawable.navi_icon);
            toolbar.SetTitle(Resource.String.ApplicationName);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += (sender, e) => {
                e.MenuItem.SetChecked(true);
                drawerLayout.CloseDrawers();
                // DrawLayout에서 자동으로 OnOptionsItemSelected 함수를 호출해주지 않아서 명시적으로 호출해준다.
                OnOptionsItemSelected(e.MenuItem);
            };
        }

        public override bool OnOptionsItemSelected(IMenuItem item) {
            MyFragment fragement = FragmentManager.FindFragmentByTag<MyFragment>(MyFragment.PAGE_CONTENT);

            View view = this.CurrentFocus;
            if (view != null) {
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }

            switch (item.ItemId) {
                case Resource.Id.nav_home:
                    MyUtil.GoPage(fragement, PAGE.HOME);
                    return true;
                case Resource.Id.nav_register:
                    MyUtil.GoPage(fragement, PAGE.REGI);
                    return true;
                case Resource.Id.nav_list:
                    MyUtil.GoPage(fragement, PAGE.LIST);
                    return true;
                default :
                    if (drawerLayout.IsDrawerOpen(navigationView)) drawerLayout.CloseDrawers();
                    else drawerLayout.OpenDrawer(navigationView);
                    return true;
            }
        }

        public override void SetContentView(int layoutResID) {
            if (drawerLayout != null) {
                LayoutInflater inflater = (LayoutInflater)GetSystemService(LayoutInflaterService);
                ViewGroup.LayoutParams lp = new ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.MatchParent);
                View stubView = inflater.Inflate(layoutResID, drawerLayout, false);
                drawerLayout.AddView(stubView, lp);
            }
        }

        public override void SetContentView(View view) {
            if (drawerLayout != null) {
                ViewGroup.LayoutParams lp = new ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.MatchParent);
                drawerLayout.AddView(view, lp);
            }
        }

        public override void SetContentView(View view, ViewGroup.LayoutParams parameters) {
            if (drawerLayout != null) {
                drawerLayout.AddView(view, parameters);
            }
        }
    }

    public class MyCustomToolbar : V7Toolbar {

        public MyCustomToolbar(Context context) : base(context){
        }

        public MyCustomToolbar(Context context, IAttributeSet attrs) : base(context, attrs) {
        }

        public MyCustomToolbar(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) {
        }


        public override void SetTitle(int resId) {
            TextView myText = ((TextView)FindViewById(Resource.Id.title));
            myText.SetText(resId);
            myText.SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.White));
            myText.SetTypeface(Typeface.SansSerif, TypefaceStyle.Bold);
            myText.SetTextSize(ComplexUnitType.Dip, 20);
        }
    }
}