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

namespace FoodDiary {
    public class PageRegisterFragment : MyFragment {
        public static bool dataChanged = true;

        string imagePath = "";
        int opt_choice = 3;
        Android.Net.Uri uri;
        public static string documentsDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        int[] opt = { Resource.Id.morningBtn, Resource.Id.firstSnackBtn, Resource.Id.LunchBtn, Resource.Id.dinnerBtn };

        public override View GetCurrentView(LayoutInflater inflater, ViewGroup container) {
            return inflater.Inflate(Resource.Layout.RegisterFragment, container, false); ;
        }

        public override void LinkEvent() {
            view.FindViewById<ImageView>(Resource.Id.myImageView).Click += imageChoose_OnClick;
            view.FindViewById<Button>(Resource.Id.saveBtn).Click += saveBtn_OnClick;
            view.FindViewById<Button>(Resource.Id.cancelBtn).Click += cancelBtn_OnClick; 
            foreach(int id in opt) view.FindViewById<Button>(id).Click += choose_option;
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
            if (String.IsNullOrWhiteSpace(name)) {
                Toast.MakeText(this.Context, "Meal name is necessary.", ToastLength.Short).Show();
                return;
            } else {
                String comment = FindViewById<EditText>(Resource.Id.comments).Text;
                saveMealInfo(name, rate, opt_choice, comment);
            }
        }

        private async void saveMealInfo(string name, int rate, int type, string comment) {
            string message = await insertUpdateData(new Meal(name, rate, type, saveImage(), comment));
            if ("Success" == message) {
                Toast.MakeText(this.Context, "Saved successfully!", ToastLength.Short).Show();
                dataChanged = true;
                MyUtil.GoHome(this);
            } else {
                Toast.MakeText(this.Context, message, ToastLength.Short).Show();
            }
        }

        private string saveImage() {
            string jpgFilename = null;

            // 용량 줄이기 위해서 파일 압축 후 복사
            if (uri != null) {
                Stream stream = view.Context.ContentResolver.OpenInputStream(uri);
                Bitmap bitmap = BitmapFactory.DecodeStream(stream);
                MemoryStream memStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, memStream);
                byte[] picData = memStream.ToArray();
                jpgFilename = GenerateFileName();
                File.WriteAllBytes(System.IO.Path.Combine(documentsDirectory, jpgFilename), picData);
            }

            return jpgFilename;
        }

        private string GenerateFileName() {
            return DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N");
        }

        public void cancelBtn_OnClick(object sender, EventArgs e) {
            MyUtil.GoHome(this);
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data) {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok) {
                FFImageLoading.Views.ImageViewAsync imageView = view.FindViewById<FFImageLoading.Views.ImageViewAsync>(Resource.Id.myImageView);
                uri = data.Data;
                imagePath = UriHelper.GetPathFromUri(view.Context, data.Data);

                ImageService.Instance
                    .LoadFile(imagePath)
                    .DownSample(width:100)
                    .LoadingPlaceholder("loading.png", FFImageLoading.Work.ImageSource.CompiledResource)
                    .ErrorPlaceholder("error.png", FFImageLoading.Work.ImageSource.CompiledResource)
                    .Into(imageView);

                FindViewById<TextView>(Resource.Id.myImageViewText).Text = "";
            }
        }

        public void imageChoose_OnClick(object sender, EventArgs e) {
            Intent imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 0);
        }

        private async Task<string> insertUpdateData(Meal data) {
            try {
                var db = new SQLiteAsyncConnection(MyUtil.DB_PATH);
                if (await db.InsertAsync(data) != 0)
                    await db.UpdateAsync(data);
                return "Success";
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }
    }
}