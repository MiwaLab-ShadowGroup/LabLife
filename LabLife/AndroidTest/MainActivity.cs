using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net.Sockets;
using System.Net;

namespace AndroidTest
{
    [Activity(Label = "AndroidTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            CIPC_CS.CLIENT.CLIENT client = new CIPC_CS.CLIENT.CLIENT();
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            EditText edit_text_remoteIP = FindViewById<EditText>(Resource.Id.remoteIP);
            EditText edit_text_remotePort = FindViewById<EditText>(Resource.Id.remotePort);


            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
        }
    }
}

