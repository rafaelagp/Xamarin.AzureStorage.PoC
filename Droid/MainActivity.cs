using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Permissions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace AzureStoragePoC.Droid
{
    [Activity(Label = "AzureStoragePoC.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            UserDialogs.Init(this);

            LoadApplication(new App());
            GetDefaultDirectories();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void GetDefaultDirectories()
        {
            System.Diagnostics.Debug.WriteLine($"Pictures Path = {GetExternalFilesDir(Environment.DirectoryPictures)}");
            System.Diagnostics.Debug.WriteLine($"Videos Path = {GetExternalFilesDir(Environment.DirectoryMovies)}");
        }
    }
}
