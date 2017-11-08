using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using System.IO;
using SQLite;
using System.Threading.Tasks;
using Java.Interop;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using FFImageLoading;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using System.Globalization;

namespace mealdiary {
    public class PageRegisterFragment : MyFragment {
        public static bool imageUpload = false;
        private Meal meal = null;
        private bool fromList = false;
        readonly string[] Permissions = { Manifest.Permission.ReadExternalStorage,
                                                Manifest.Permission.WriteExternalStorage };

        string imagePath = "";
        int opt_choice = 3;
        Android.Net.Uri uri;
        public static string documentsDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        int[] opt = { Resource.Id.morningBtn, Resource.Id.LunchBtn, Resource.Id.dinnerBtn, Resource.Id.firstSnackBtn};

        public override View GetCurrentView(LayoutInflater inflater, ViewGroup container) {
            return inflater.Inflate(Resource.Layout.RegisterFragment, container, false); ;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState) {
            base.OnViewCreated(view, savedInstanceState);
            meal = null;

            FindViewById<TextView>(Resource.Id.regCreateDate).Text = DateTime.Now.ToString("yyyy-MM-dd");
            if (PageListFragment.fromList) {
                PageListFragment.fromList = false;
                fromList = true;
                meal = PageListFragment.meal;
            }

            SetMealInformationForUpdating();
        }

        private void SetMealInformationForUpdating() {
            if (meal != null) {
                FindViewById<TextView>(Resource.Id.foodName).Text = meal.Name;
                FindViewById<TextView>(Resource.Id.comments).Text = meal.Comments;
                FindViewById<RatingBar>(Resource.Id.ratingBar).Rating = meal.Rate;
                FindViewById<Button>(Resource.Id.saveBtn).Text = "Update";
                FindViewById<Button>(Resource.Id.regCreateDate).Text = meal.CreateDate.ToString("yyyy-MM-dd");
                choose_option(FindViewById<Button>(opt[meal.Type - 1]), null);

                if (!string.IsNullOrWhiteSpace(meal.ImageName)) {
                    
                    FFImageLoading.Views.ImageViewAsync imageView = FindViewById<FFImageLoading.Views.ImageViewAsync>(Resource.Id.myImageView);
                    ImageService.Instance
                            .LoadFile(System.IO.Path.Combine(PageRegisterFragment.documentsDirectory, meal.ImageName))
                            .DownSample(width: 100, height: 100)
                            .LoadingPlaceholder("loading.png", FFImageLoading.Work.ImageSource.CompiledResource)
                            .ErrorPlaceholder("error.png", FFImageLoading.Work.ImageSource.CompiledResource)
                            .Into(imageView);

                    FindViewById<TextView>(Resource.Id.myImageViewText).Text = "";
                }
            }
        }

        public override void LinkEvent() {
            FindViewById<ImageView>(Resource.Id.myImageView).Click += imageChoose_OnClick;
            FindViewById<Button>(Resource.Id.saveBtn).Click += saveBtn_OnClick;
            FindViewById<Button>(Resource.Id.cancelBtn).Click += cancelBtn_OnClick;
            FindViewById<Button>(Resource.Id.regCreateDate).Click += viewDatePicker;
            FindViewById<RelativeLayout>(Resource.Id.registerTableId).Touch += hideSoftKeyboard;
            foreach (int id in opt) FindViewById<Button>(id).Click += choose_option;
        }

        private void hideSoftKeyboard(object sender, View.TouchEventArgs e) {
            MyUtil.HideSoftKeyboard(Activity);
        }

        private void viewDatePicker(object sender, EventArgs e) {
            new DatePickerDialog(this.Context, setCreateDate, DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day).Show();
        }

        private void setCreateDate(object sender, DatePickerDialog.DateSetEventArgs e) {
            FindViewById<Button>(Resource.Id.regCreateDate).Text = e.Date.ToString("yyyy-MM-dd");
        }

        public void choose_option(object sender, EventArgs e) {
            int viewId = ((View)sender).Id;
            foreach (int id in opt) {
                int styleId = viewId == id ? Resource.Drawable.choosen : Resource.Drawable.unchoosen;
                FindViewById<Button>(id).Background = Resources.GetDrawable(styleId, null);
            }

            opt_choice = Array.IndexOf(opt, viewId) + 1;
        }

        public void saveBtn_OnClick(object sender, EventArgs e) {
            String name = FindViewById<EditText>(Resource.Id.foodName).Text;
            int rate = (int)FindViewById<RatingBar>(Resource.Id.ratingBar).Rating;
            string createDate = FindViewById<Button>(Resource.Id.regCreateDate).Text;
            if (String.IsNullOrWhiteSpace(name)) {
                Toast.MakeText(this.Context, "Meal name is necessary.", ToastLength.Short).Show();
                return;
            } else {
                String comment = FindViewById<EditText>(Resource.Id.comments).Text;
                saveMealInfo(name, rate, opt_choice, comment, createDate);
            }
        }

        private async void saveMealInfo(string name, int rate, int type, string comment, string createDate) {
            if (meal != null) {
                meal.Name = name;
                meal.Rate = rate;
                meal.Type = type;
                meal.Comments = comment;
                meal.CreateDate = DateTime.ParseExact(createDate, "yyyy-MM-dd", CultureInfo.InvariantCulture); ;
                if(uri!=null) meal.ImageName = saveImage(null);
            } else {
                meal = new Meal(name, rate, type, saveImage(null), comment, createDate);
            }
            
            string message = await insertUpdateData(meal);
            if ("Success" == message) {
                Toast.MakeText(this.Context, "Saved successfully!", ToastLength.Short).Show();
                MyUtil.GoHome(this);
            } else {
                Toast.MakeText(this.Context, message, ToastLength.Short).Show();
            }
        }

        private string saveImage(string imageName) {
            string jpgFilename = imageName;

            // 용량 줄이기 위해서 파일 압축 후 복사
            if (uri != null) {
                Stream stream = Context.ContentResolver.OpenInputStream(uri);
                Bitmap bitmap = BitmapFactory.DecodeStream(stream);
                MemoryStream memStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, memStream);
                byte[] picData = memStream.ToArray();
                jpgFilename = (string.IsNullOrWhiteSpace(imageName)?GenerateFileName():imageName);

                string fileFullPath = System.IO.Path.Combine(documentsDirectory, jpgFilename);
                if (File.Exists(fileFullPath)) File.Delete(fileFullPath);
                File.WriteAllBytes(fileFullPath, picData);
            }

            return jpgFilename;
        }

        private string GenerateFileName() {
            return DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N");
        }

        public void cancelBtn_OnClick(object sender, EventArgs e) {
            if (fromList) {
                fromList = false;
                MyUtil.GoList(this);
            }else {
                MyUtil.GoHome(this);
            }
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data) {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok) {
                FFImageLoading.Views.ImageViewAsync imageView = FindViewById<FFImageLoading.Views.ImageViewAsync>(Resource.Id.myImageView);
                uri = data.Data;
                imagePath = UriHelper.GetPathFromUri(Context, data.Data);

                ImageService.Instance
                    .LoadFile(imagePath)
                    .DownSample(width: 100)
                    .LoadingPlaceholder("loading.png", FFImageLoading.Work.ImageSource.CompiledResource)
                    .ErrorPlaceholder("error.png", FFImageLoading.Work.ImageSource.CompiledResource)
                    .Into(imageView);

                FindViewById<TextView>(Resource.Id.myImageViewText).Text = "";
            }
        }

        public void imageChoose_OnClick(object sender, EventArgs e) {
            if (imageUpload) {
                Intent imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 0);
            }else {
                if (ContextCompat.CheckSelfPermission(this.Context, Manifest.Permission.ReadExternalStorage)
                    != Permission.Granted) {
                    if (ShouldShowRequestPermissionRationale(Permissions[0])) {
                        //Explain to the user why we need to read the contacts
                        Snackbar.Make(view, "External stroage access is necessary to choose image file.", Snackbar.LengthIndefinite)
                                .SetAction("OK", v => RequestPermissions(Permissions, 1004))
                                .Show();
                        return;
                    }
                } else {
                    imageUpload = true;
                }
                RequestPermissions(Permissions, 1004);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults) {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (requestCode == 1004) {
                // If request is cancelled, the result arrays are empty.
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted) imageUpload = true;
                else imageUpload = false;
            }
        }

        private async Task<string> insertUpdateData(Meal data) {
            try {
                var db = new SQLiteAsyncConnection(MyUtil.DB_PATH);
                
                if (await db.UpdateAsync(data) == 0)
                    await db.InsertAsync(data);
                return "Success";
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }
    }
}