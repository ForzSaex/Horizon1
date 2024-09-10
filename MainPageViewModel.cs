using Android.Graphics.Drawables;
using Android.Content.PM;

using Microsoft.Maui.ApplicationModel;
using Android.Graphics;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.ComponentModel;

namespace Horizon.MainPageViewModel
{
    internal class MainPageViewModel
    {
        public ObservableCollection<ListApp> Apps { get; set; } = new ObservableCollection<ListApp>();

        public MainPageViewModel()
        {

            
            var pm = Android.App.Application.Context.PackageManager;
            var apps = pm.GetInstalledApplications(PackageInfoFlags.MatchAll);
            var userApps = new List<AppInfoModel>();

            foreach (var packageInfo in apps)
            {
                // Filtra aplicativos do sistema
                if ((packageInfo.Flags & ApplicationInfoFlags.System) == 0)
                {
                    var appInfo = new AppInfoModel
                    {
                        AppName = packageInfo.LoadLabel(pm),
                        PackageName = packageInfo.PackageName,
                        AppIcon = packageInfo.LoadIcon(pm)
                    };
                    userApps.Add(appInfo);
                }
            }

            // Exibe os aplicativos filtrados
            foreach (var app in userApps)
            {
                Apps.Add(new ListApp
                {
                    AppName = app.AppName,
                    PackageName = app.PackageName,
                    AppIcon = ConvertDrawableToImageSource(app.AppIcon),
                });

                // Checagem de depuração
                Console.WriteLine($"Adicionando: {app.AppName} - {app.PackageName} - {app.AppIcon}");

                /*
                // Se desejar abrir o aplicativo
                var launchIntent = pm.GetLaunchIntentForPackage(app.PackageName);
                if (launchIntent != null)
                {
                    // Inicia o aplicativo
                    Android.App.Application.Context.StartActivity(launchIntent);
                }
                else
                {
                    Console.WriteLine($"Não foi possível abrir o aplicativo: {app.AppName}");
                }
                */

               
            }
            Console.WriteLine($"Total de apps: {Apps.Count()}");
        }

        public ImageSource ConvertDrawableToImageSource(Drawable drawable)
        {
            if (drawable is BitmapDrawable bitmapDrawable)
            {
                var bitmap = bitmapDrawable.Bitmap;
                var stream = new MemoryStream();
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, stream);
                stream.Seek(0, SeekOrigin.Begin);
                return ImageSource.FromStream(() => stream);
            }
            return null;
        }




    }
    public class Layout
    {
        public static Grid openMenu { get; set; }
        public static Button Button { get; set; }
        public static AbsoluteLayout absoluteLayout { get; set; }
        public static Grid Menu { get; set; }
    }
    public class Preferences
    {
        private const string stockData = "true";
        public static bool isStockData
        {
            get => Microsoft.Maui.Storage.Preferences.Get(stockData, true);
            set => Microsoft.Maui.Storage.Preferences.Set(stockData, value);
        }

    }
    public class ListApp
    {
        public string AppName { get; set; }
        public string PackageName { get; set; }
        public ImageSource AppIcon { get; set; }

    }
}
