#if ANDROID
using Android.Graphics.Drawables;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horizon
{
#if ANDROID
    public interface IAppInfoService
    {
        List<AppInfoModel> GetInstalledApps();
    }

    public class AppInfoModel
    {
        public string AppName { get; set; }
        public string PackageName { get; set; }
        public ImageSource AppIcon { get; set; }
    }
#endif
}
