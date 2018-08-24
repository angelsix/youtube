using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using System;

namespace AndroidLifecycle
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        //
        // A good reference starting point for the actibity life cycle is here
        // https://developer.android.com/reference/android/app/Activity
        // 
        // Events on application creation
        // =========================================
        // ~~~~~~~~~~~~~~~~ OnCreate
        // ~~~~~~~~~~~~~~~~ OnStart
        // ~~~~~~~~~~~~~~~~ OnPostCreate
        // ~~~~~~~~~~~~~~~~ OnStateNotSaved
        // ~~~~~~~~~~~~~~~~ OnResume
        // ~~~~~~~~~~~~~~~~ OnPostResume
        // 
        // 
        // Events on back button press
        // =========================================
        // ~~~~~~~~~~~~~~~~ Finish
        // ~~~~~~~~~~~~~~~~ OnPause
        // ~~~~~~~~~~~~~~~~ OnStop
        // ~~~~~~~~~~~~~~~~ OnDestroy
        // 
        // 
        // Events on re-open after back button
        // =========================================
        // ~~~~~~~~~~~~~~~~ OnCreate
        // ~~~~~~~~~~~~~~~~ OnStart
        // ~~~~~~~~~~~~~~~~ OnPostCreate
        // ~~~~~~~~~~~~~~~~ OnStateNotSaved
        // ~~~~~~~~~~~~~~~~ OnResume
        // ~~~~~~~~~~~~~~~~ OnPostResume
        // 
        // 
        // Events on Home / Menu button press
        // =========================================
        // ~~~~~~~~~~~~~~~~ OnUserLeaveHint
        // ~~~~~~~~~~~~~~~~ OnPause
        // ~~~~~~~~~~~~~~~~ OnSaveInstanceState
        // ~~~~~~~~~~~~~~~~ OnStop
        // 
        // 
        // Events on re-open after Home / Menu
        // =========================================
        // ~~~~~~~~~~~~~~~~ OnStateNotSaved
        // ~~~~~~~~~~~~~~~~ OnRestart
        // ~~~~~~~~~~~~~~~~ OnStart
        // ~~~~~~~~~~~~~~~~ OnResume
        // ~~~~~~~~~~~~~~~~ OnPostResume
        // 
        // 
        // Instant Kill
        // =========================================
        // No guaranteed calls
        // 
        // 
        // DONT USE DISPOSE or JavaFinalize (Slide 44 onwards)
        // Use OnDestroy instead
        // https://www.slideshare.net/Xamarin/advanced-memory-management-on-ios-and-android-mark-probst-and-rodrigo-kumpera
        //

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnCreate");

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
        }

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnCreate 2");

            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnStart()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnStart");
            base.OnStart();
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnPostCreate");
            base.OnPostCreate(savedInstanceState);
        }

        public override void OnPostCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnPostCreate 2");
            base.OnPostCreate(savedInstanceState, persistentState);
        }

        public override void OnStateNotSaved()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnStateNotSaved");
            base.OnStateNotSaved();
        }

        protected override void OnResume()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnResume");
            base.OnResume();
        }

        protected override void OnPostResume()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnPostResume");
            base.OnPostResume();
        }

        protected override void OnUserLeaveHint()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnUserLeaveHint");
            base.OnUserLeaveHint();
        }

        protected override void OnPause()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnPause");
            base.OnPause();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnSaveInstanceState");
            base.OnSaveInstanceState(outState);
        }

        public override void OnSaveInstanceState(Bundle outState, PersistableBundle outPersistentState)
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnSaveInstanceState 2");
            base.OnSaveInstanceState(outState, outPersistentState);
        }

        protected override void OnStop()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnStop");
            base.OnStop();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnRestart");
            base.OnRestart();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ OnDestroy");
            base.OnDestroy();
        }

        public override void Finish()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~ Finish");
            base.Finish();
        }

        #region App Specific Code

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        #endregion
    }
}

