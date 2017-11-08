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
using System.IO;
using Java.IO;
using Android.Graphics;
using Android.Support.V7.App;
using Android.Views.InputMethods;
using SQLite;
using System.Threading.Tasks;

namespace mealdiary {
    public class MyUtil {
        public static string DATABASE_DIRECTORY = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        public static string DATABASE_FILE_NAME = "Meal.db3";
        public static string DB_PATH = System.IO.Path.Combine(DATABASE_DIRECTORY, DATABASE_FILE_NAME);

        public static int SWIPE_DISTANCE_THRESHOLD = 50;
        public static int SWIPE_VELOCITY_THRESHOLD = 50;
        public static int thumbnailSize = 1000;
        public static String NO_DATA = "NO DATA";

        public static List<Meal> GetDataWithNoDataProcessing(List<Meal> list) {
            if (list == null || list.Count == 0) {
                Meal meal = new Meal();
                meal.Name = MyUtil.NO_DATA;
                list.Add(meal);
            }
            return list;
        }

        public static List<PickedMeal> GetDataWithNoDataProcessing(List<PickedMeal> list) {
            if (list == null) list = new List<PickedMeal>();

            if (list.Count == 0) {
                PickedMeal meal = new PickedMeal();
                meal.Name = MyUtil.NO_DATA;
                list.Add(meal);
            }
            return list;
        }

        public static Bitmap GetThumbnailBitmap(string imageName, bool register) {
            bool success = false;
            Bitmap bitmap = null;
            while (!success) {
                try {
                    string path = System.IO.Path.Combine(PageRegisterFragment.documentsDirectory, imageName);

                    BitmapFactory.Options options = new BitmapFactory.Options();
                    options.InJustDecodeBounds = true;
                    BitmapFactory.DecodeFile(path, options);
                    if ((options.OutWidth == -1) || (options.OutHeight == -1)) {
                        bitmap = null;
                    }
                    int originalSize = (options.OutHeight > options.OutWidth) ? options.OutHeight : options.OutWidth;
                    BitmapFactory.Options opts = new BitmapFactory.Options();
                    opts.InSampleSize = originalSize / thumbnailSize;
                    bitmap = BitmapFactory.DecodeFile(path, opts);
                    success = true;
                }
                catch (Exception) {
                    success = false;
                    GC.Collect();
                }
            }
            return bitmap;
        }

        public static void GoHome(MyFragment fragment) {
            GoPage(fragment, PAGE.HOME);
        }

        public static void GoRegister(MyFragment fragment) {
            GoPage(fragment, PAGE.REGI);
        }

        public static void GoList(MyFragment fragment) {
            GoPage(fragment, PAGE.LIST);
        }

        public static void GoPage(MyFragment fragment, PAGE page) {
            MyAppCompatActivity.CURRENT_PAGE = page;

            FragmentTransaction ft = fragment.FragmentManager.BeginTransaction();

            if (page == PAGE.HOME) {
                ft.Replace(Resource.Id.page_placeholder, new PageMainFragment(), MyFragment.PAGE_CONTENT);
            } else if (page == PAGE.REGI) {
                ft.Replace(Resource.Id.page_placeholder, new PageRegisterFragment(), MyFragment.PAGE_CONTENT);
            } else if (page == PAGE.LIST) {
                ft.Replace(Resource.Id.page_placeholder, new PageListFragment(), MyFragment.PAGE_CONTENT);
            }
            ft.Commit();
        }
        public static string GetType(Meal item) {
            switch (item.Type) {
                case 1: return "Morning";
                case 2: return "Lunch";
                case 3: return "Dinner";
                default: return "Snack";
            }
        }

        public static void HideSoftKeyboard(Activity act) {
            View view = act.CurrentFocus;
            if (view != null) {
                InputMethodManager imm = (InputMethodManager)act.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
        }

        public async static Task<string> DeleteItem(int id) {
            try {
                var db = new SQLiteAsyncConnection(MyUtil.DB_PATH);
                await db.DeleteAsync(new Meal(id));
                return "Success";
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }
    }

    public enum PAGE { HOME, REGI, LIST };
}