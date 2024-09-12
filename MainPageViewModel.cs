#if ANDROID
using Android.Graphics.Drawables;
using Android.Content.PM;
using Microsoft.Maui.ApplicationModel;
using AndroidX.Core.Graphics.Drawable;
using Android.Graphics;
#endif
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

#if ANDROID
namespace Horizon.MainPageViewModel
{
    internal class MainPageViewModel
    {
        public ObservableCollection<ListApp> Apps { get; set; } = new ObservableCollection<ListApp>();

        public MainPageViewModel()
        {

                HashSet<string> allowedApps = new HashSet<string>
                {
                    "Calendário", "Câmera", "Chrome", "Contatos", "Facebook", "Galaxy Store",
                    "Galaxy Themes", "Galeria", "Gmail", "Google", "Google Play Store", "Maps",
                    "Meet", "Meus Arquivos", "OneDrive", "Relógio", "Telefone", "Youtube"
                };

                HashSet<string> filteredApps = new HashSet<string>
                {
                    "Centro de conteúdo de teclado", "CoolEUKor", "ChocoEUKo", "Horizon",
                    "MyTube edge", "Recent Files", "Samsung Editing Assets", "SamsungSans, Calculator Panel, Calendar"
                };
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
                        AppIcon = DrawableImageSource.ConvertDrawableToImageSource(packageInfo.LoadIcon(pm))
                    };
                    if (!filteredApps.Contains(packageInfo.LoadLabel(pm)))
                    {
                        userApps.Add(appInfo);
                    }
                    
                    
                }
                else if (allowedApps.Contains(packageInfo.LoadLabel(pm)))
                {
                    var appInfo = new AppInfoModel
                    {
                        AppName = packageInfo.LoadLabel(pm),
                        PackageName = packageInfo.PackageName,
                        AppIcon = DrawableImageSource.ConvertDrawableToImageSource(packageInfo.LoadIcon(pm))
                    };
                    userApps.Add(appInfo);
                }
            }

            userApps = userApps.OrderBy(app => app.AppName).ToList();

            // Exibe os aplicativos filtrados
            foreach (var app in userApps)
            {
                Apps.Add(new ListApp
                {
                    AppName = app.AppName,
                    PackageName = app.PackageName,
                    AppIcon = app.AppIcon,
                    openapp = new Command(() =>
                    {
                        var launchIntent = Android.App.Application.Context.PackageManager.GetLaunchIntentForPackage(app.PackageName);
                        if (launchIntent != null)
                        {
                            Android.App.Application.Context.StartActivity(launchIntent);
                        }
                        else
                        {
                            Console.WriteLine($"Não foi possível abrir o aplicativo: {app.AppName}");
                        }
                    }),
                });
            }
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
        public Command openapp { get; set; }

    }
    public class DrawableImageSource
    {
        public static ImageSource ConvertDrawableToImageSource(Drawable drawable)
        {
            if (drawable is AdaptiveIconDrawable adaptiveIcon)
            {
                // Crie um Bitmap com as dimensões do ícone
                var bitmap = Android.Graphics.Bitmap.CreateBitmap(100, 100, Android.Graphics.Bitmap.Config.Argb8888);
                var canvas = new Canvas(bitmap);

                // Desenhe o AdaptiveIconDrawable no Canvas
                adaptiveIcon.SetBounds(0, 0, canvas.Width, canvas.Height);
                adaptiveIcon.Draw(canvas);

                // Converta o Bitmap para um StreamImageSource para ser usado no MAUI Image
                return ImageSource.FromStream(() =>
                {
                    var ms = new MemoryStream();
                    bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms;
                });
            }
            else if (drawable is BitmapDrawable bitmapDrawable)
            {
                var bitmap = bitmapDrawable.Bitmap;
                var stream = new MemoryStream();
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, stream);
                stream.Seek(0, SeekOrigin.Begin);

                // Retorna o ImageSource diretamente a partir do stream
                return ImageSource.FromStream(() => new MemoryStream(stream.ToArray()));
            }
            return null;
        }
    }
}
#endif