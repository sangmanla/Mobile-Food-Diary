using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using SQLite;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.App;
using FFImageLoading;
using FFImageLoading.Work;
using Android.Support.Design.Widget;

namespace mealdiary {
    public class PageMainFragment : MyFragment {
        public static PickedMeal meal;
        private static List<PickedMeal> list;
        private static List<PickedMeal> upList;
        private static List<PickedMeal> downList;
        private static LinearLayout.LayoutParams para1 = null, para2 = null;
        public override View GetCurrentView(LayoutInflater inflater, ViewGroup container) {
            return inflater.Inflate(Resource.Layout.MainFragment, container, false); ;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState) {
            base.OnViewCreated(view, savedInstanceState);

            GetDataList();
        }

        private void GetDataList() {
            list = RearangeList(GetNameDistributed(getAllForOneMonth()));
            ShowTop5RecordsForOneMonth(list);
            ShowFrq5RecordsForOneMonth(list);
        }

        private void ShowTop5RecordsForOneMonth(List<PickedMeal> allList) {
            allList = allList.OrderByDescending(x => x.Rate).ToList();
            allList = allList.Count > 5 ? allList.GetRange(0, 5) : allList;
            ListView listView = FindViewById<ListView>(Resource.Id.mainList1);
            upList = MyUtil.GetDataWithNoDataProcessing(allList);
            MainListAdaptor adaptor = new MainListAdaptor(view, upList);
            adaptor.type = true;
            listView.Adapter = adaptor;
            listView.ItemClick += OnListItemClick1;
            UpdateLayout(para1, allList, listView);
        }

        private void ShowFrq5RecordsForOneMonth(List<PickedMeal> allList) {
            allList = allList.OrderByDescending(x => x.Count).ToList();
            allList = allList.Count > 5 ? allList.GetRange(0, 5) : allList;
            ListView listView = FindViewById<ListView>(Resource.Id.mainList2);
            downList = MyUtil.GetDataWithNoDataProcessing(allList);
            MainListAdaptor adaptor = new MainListAdaptor(view, downList);
            adaptor.type = false;
            listView.Adapter = adaptor;
            listView.ItemClick += OnListItemClick2;
            UpdateLayout(para2, allList, listView);
        }

        protected void OnListItemClick1(object sender, AdapterView.ItemClickEventArgs e) {
            OnListItemClick(sender, e, upList);
        }
        protected void OnListItemClick2(object sender, AdapterView.ItemClickEventArgs e) {
            OnListItemClick(sender, e, downList);
        }

        protected void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e, List<PickedMeal> list) {
            ListView listView = sender as ListView;

            for(int i = 0; i < listView.ChildCount; i++) listView.GetChildAt(i).SetBackgroundColor(Color.Transparent);
            e.View.SetBackgroundColor(Color.LightGray);

            meal = list[e.Position];

            // custom dialog
            if (dialog == null) dialog = new Dialog(view.Context, Resource.Style.Dialog);
            dialog.SetContentView(Resource.Layout.ViewInformation);
            dialog.FindViewById<TextView>(Resource.Id.viewComments).Text = meal.Comments;
            dialog.FindViewById<TextView>(Resource.Id.viewFoodName).Text = meal.Name;
            dialog.FindViewById<RatingBar>(Resource.Id.viewRatingBar).Rating = meal.Rate;
            dialog.FindViewById<TextView>(Resource.Id.viewMealType).Text = meal.CreateDate.ToString("yyyy-MM-dd") + " / " + MyUtil.GetType(meal);

            FFImageLoading.Views.ImageViewAsync imageView = dialog.FindViewById<FFImageLoading.Views.ImageViewAsync>(Resource.Id.viewImageView);
            if (!string.IsNullOrWhiteSpace(meal.ImageName)) {
                ImageService.Instance
                        .LoadFile(System.IO.Path.Combine(PageRegisterFragment.documentsDirectory, meal.ImageName))
                        .DownSampleInDip(width: 250)
                        .LoadingPlaceholder("loading.png", FFImageLoading.Work.ImageSource.CompiledResource)
                        .ErrorPlaceholder("error.png", FFImageLoading.Work.ImageSource.CompiledResource)
                        .Into(imageView);
            } else ImageService.Instance.LoadCompiledResource("noImage.png").Into(imageView);

            dialog.FindViewById<LinearLayout>(Resource.Id.firstLineInViewInformation).LayoutParameters 
                = new TableRow.LayoutParams(LinearLayout.LayoutParams.MatchParent, 0);
            dialog.CancelEvent += closeDialog;
            dialog.Show();
        }

        private void closeDialog(object sender, EventArgs e) {
            dialog.Dismiss();
        }

        private static void UpdateLayout(LinearLayout.LayoutParams parame, List<PickedMeal> allList, ListView listView) {
            if (parame == null && allList.Count != 5) {
                parame = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                parame.Height = 360 * allList.Count;
                listView.LayoutParameters = parame;
            }
        }


        private static List<PickedMeal> RearangeList(List<Meal> newList) {
            List<PickedMeal> pickedMealList = new List<PickedMeal>();
            foreach (Meal meal in newList) {
                PickedMeal selectedPickedMeal = null;
                foreach (PickedMeal pMeal in pickedMealList) {
                    if (pMeal.Name == meal.Name) {
                        selectedPickedMeal = pMeal;
                        break;
                    }
                }
                if (selectedPickedMeal == null) {
                    // 메인은 통계적인 정보를 보여주는 건데, 개별 아이템에 날짜/type 정보를 보여주기 애매해서 일단 첫번째 데이터로 뿌려줌
                    pickedMealList.Add(new PickedMeal(meal.Name, meal.Rate, meal.ImageName, meal.CreateDate, meal.Comments, meal.Type));
                } else {
                    selectedPickedMeal.AddRate(meal.Rate);
                    selectedPickedMeal.AddCount();
                    selectedPickedMeal.SetComments(meal.Comments); // 제일 긴걸로 넣어준다.
                }
            }
            return pickedMealList;
        }

        private static List<Meal> GetNameDistributed(Task<List<Meal>> result) {
            List<Meal> newList = new List<Meal>();
            foreach (Meal meal in result.Result) {
                String[] names = meal.Name.Split(',');
                String[] newNames = new string[names.Length];
                for (int i = 0; i < names.Length; i++) newNames[i] = names[i].Trim();
                foreach (string newName in newNames) {
                    Meal newMeal = meal.CopyMeal();
                    newMeal.Name = newName;
                    newList.Add(newMeal);
                }
            }

            return newList;
        }

        private static Task<List<Meal>> getAllForOneMonth() {
            string sql = "SELECT * " +
                "FROM MEAL WHERE datetime(CREATEDATE/10000000 - 62135596800, 'unixepoch') >= DATETIME('NOW', '-1 MONTH') " +
                "OR CREATEDATE IS NULL " +
                "ORDER BY CREATEDATE DESC ";
            return (new SQLiteAsyncConnection(MyUtil.DB_PATH)).QueryAsync<Meal>(sql);
        }
    }

    public class PickedMeal : Meal {
        
        public PickedMeal() { }
        public PickedMeal(string name, float rate, string imageName, DateTime createdate, string comments, int type) {
            Name = name;
            Rate = rate;
            ImageName = imageName;
            CreateDate = createdate;
            Comments = comments;
            Type = type;
        }
        public new float Rate { get; set; }
        public int Count { get; set; } = 1;
        public void AddRate(int rate) {
            Rate = (Rate * Count + rate);
            Rate /= Count + 1;
        }
        public void AddCount() {
            if (Count == 0) Count = 1;
            Count++;
        }

        internal void SetComments(string comments) {
            if(String.IsNullOrWhiteSpace(comments) && this.Comments.Length < comments.Length) this.Comments = comments;
        }
    }
}