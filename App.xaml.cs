using Microsoft.Extensions.Logging;

namespace Horizon
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

#if DEBUG
            // Adicionar logging para bindings
            this.HandlerChanged += (s, e) =>
            {
                var handler = this.Handler;
                if (handler != null)
                {
                    var logger = handler.MauiContext.Services.GetService<ILogger<App>>();
                    logger.LogDebug("Debug de binding ativado.");
                }
            };
#endif
            MainPage = new AppShell();

        }
    }
}
