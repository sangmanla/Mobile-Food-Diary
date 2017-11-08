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
using static Android.App.ActivityManager;

/**
 * 초기에 slide로 화면 띄우는 시도 하려고 만들었던 클래스인데, 
 * 좌측 상단 네비게이션 넣으면서 안쓰게 됨. 나중에 쓰려고 남겨둠.
 */ 
namespace mealdiary {
    [Activity(Label = "SwipeManager")]
    public class SwipeActivity : Activity, GestureDetector.IOnGestureListener {
        private GestureDetector _gestureDetector;
        public static bool direction; // false : left, true : right
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            if (direction) OverridePendingTransition(Resource.Animation.trans_right_in, Resource.Animation.trans_right_out); 
            else OverridePendingTransition(Resource.Animation.trans_left_in, Resource.Animation.trans_left_out);

            _gestureDetector = new GestureDetector(this);
        }

        public bool OnDown(MotionEvent e) {
            return true;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY) {
            //float distanceX = e2.GetX() - e1.GetX();
            //float distanceY = e2.GetY() - e1.GetY();
            //if (Math.Abs(distanceX) > Math.Abs(distanceY) && Math.Abs(distanceX) > MyUtil.SWIPE_DISTANCE_THRESHOLD
            //    && Math.Abs(velocityX) > MyUtil.SWIPE_VELOCITY_THRESHOLD) {

            //    int index = (this is RegisterActivity) ? 0 : ((this is MainActivity) ? 1 : 2);
            //    direction = distanceX > 0;

            //    if (distanceX > 0) index = (index + 2) % 3;
            //    else index = (index + 1) % 3;

            //    switch (index) {
            //        case 0:
            //            StartActivity(typeof(RegisterActivity));
            //            Finish();
            //            break;
            //        case 1:
            //            StartActivity(typeof(MainActivity));
            //            Finish();
            //            break;
            //        default:
            //            StartActivity(typeof(ListActivity));
            //            Finish();
            //            break;
            //    }
                
            //    return true;
            //}
            return false;
        }

        public void OnLongPress(MotionEvent e) {
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY) {
            return false;
        }

        public void OnShowPress(MotionEvent e) {
        }

        public bool OnSingleTapUp(MotionEvent e) {
            return false;
        }

        public override bool OnTouchEvent(MotionEvent e) {
            _gestureDetector.OnTouchEvent(e);
            return false;
        }
    }
}