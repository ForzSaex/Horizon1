using Android.Content.PM;
using Android.App;

[assembly: Dependency(typeof(Horizon.Platforms.AppInfoService))]
namespace Horizon.Platforms
{
    public class AppInfoService : IAppInfoService
    {
        public List<AppInfoModel> GetInstalledApps()
        {
            var apps = new List<AppInfoModel>();
            var context = Android.App.Application.Context;
            PackageManager packageManager = context.PackageManager;

            if (context == null)
            {
                Console.WriteLine("Contexto da aplicação não está disponível.");
                return new List<AppInfoModel>();
            }
            var pm = Android.App.Application.Context?.PackageManager;
            if (pm == null)
            {
                Console.WriteLine("PackageManager não está disponível.");
                return new List<AppInfoModel>(); 
            }
            var packages = pm.GetInstalledApplications(PackageInfoFlags.MetaData);
            

            foreach (var packageInfo in packages)
            {
                var appInfo = new AppInfoModel
                {
                    AppName = pm.GetApplicationLabel(packageInfo).ToString(),
                    PackageName = packageInfo.Name
                };
                apps.Add(appInfo);
            }
            return apps;
        }
    }
}
