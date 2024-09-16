#if ANDROID
using Android.Graphics.Drawables;
using Android.Content.PM;
using Microsoft.Maui.ApplicationModel;
using AndroidX.Core.Graphics.Drawable;
using Android.Graphics;
#endif
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.Json;
using System.Diagnostics;

#if ANDROID
namespace Horizon.MainPageViewModel
{
    internal class MainPageViewModel
    {
        private Dictionary<string, ImageSource> iconCache = new Dictionary<string, ImageSource>();
        private string cacheDirectory;
        public List<ListApp> userApps = new List<ListApp>();
        private const string AppsKey = "SavedApps";

        public MainPageViewModel()
        {

        cacheDirectory = System.IO.Path.Combine(FileSystem.AppDataDirectory, "AppIcons");
        if (!Directory.Exists(cacheDirectory))
        {
            Directory.CreateDirectory(cacheDirectory);
        }

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
            try
            {

                if (Preferences.Get(AppsKey,string.Empty) != string.Empty)
                {
                    var savedAppsJson = Preferences.Get(AppsKey, string.Empty);
                    List<ListApp>? savedApps = JsonSerializer.Deserialize<List<ListApp>>(savedAppsJson);
                    if (savedApps != null)
                    {
                        foreach (var app in savedApps)
                        {
                            userApps.Add(app);
                        }
                        savedApps = null;
                    }
                }
                else
                {
                    // Se não houver apps salvos, carrega os apps instalados
                    LoadInstalledApps();
                }
            }
            catch(Exception ex)
            {
            Shell.Current.DisplayAlert("Error",ex.ToString(),"Ok");
            }
            void LoadInstalledApps()
            {
                foreach (var packageInfo in apps)
                {
                    // Filtra aplicativos do sistema
                    if (((packageInfo.Flags & ApplicationInfoFlags.System) == 0 && !filteredApps.Contains(packageInfo.LoadLabel(pm))) || allowedApps.Contains(packageInfo.LoadLabel(pm)))
                    {
                        ImageSource appIcon = LoadIconFromCache(packageInfo.PackageName);
                        if (appIcon == null)
                        {
                            var drawableIcon = packageInfo.LoadIcon(pm);
                            appIcon = DrawableImageSource.ConvertDrawableToImageSource(drawableIcon);

                            // Salva o ícone no cache de arquivos para uso futuro
                            SaveIconToCache(packageInfo.PackageName, drawableIcon);
                        }
                        var appInfo = new ListApp
                        {
                            AppName = packageInfo.LoadLabel(pm),
                            PackageName = packageInfo.PackageName,
                            AppIcon = appIcon,
                            openapp = new Command(() =>
                            {
                                var launchIntent = Android.App.Application.Context.PackageManager.GetLaunchIntentForPackage(packageInfo.PackageName);
                                if (launchIntent != null)
                                {
                                    Android.App.Application.Context.StartActivity(launchIntent);
                                }
                            }),
                        };
                        userApps.Add(appInfo);
                    }
                }
                userApps = userApps.OrderBy(app => app.AppName).ToList();
                var appsJson = JsonSerializer.Serialize(userApps);
                Preferences.Set(AppsKey, appsJson);
            }
            

            ImageSource LoadIconFromCache(string packageName)
            {
                var filePath = System.IO.Path.Combine(cacheDirectory, $"{packageName}.png");

                if (File.Exists(filePath))
                {
                    return ImageSource.FromFile(filePath);
                }

                return null;
            }
            void SaveIconToCache(string packageName, Android.Graphics.Drawables.Drawable drawable)
            {
                var filePath = System.IO.Path.Combine(cacheDirectory, $"{packageName}.png");

                if (drawable is BitmapDrawable bitmapDrawable)
                {
                    using (var outputStream = File.Create(filePath))
                    {
                        bitmapDrawable.Bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, outputStream);
                    }
                }
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
    public class UserPreferences
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