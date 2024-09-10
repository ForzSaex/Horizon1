using Android.Graphics.Drawables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horizon
{
    public interface IAppInfoService
    {
        List<AppInfoModel> GetInstalledApps();
    }

    public class AppInfoModel
    {
        public string AppName { get; set; }
        public string PackageName { get; set; }
        public Drawable AppIcon { get; set; }
    }
}
