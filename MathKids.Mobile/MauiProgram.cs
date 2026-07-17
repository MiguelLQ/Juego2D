using MathKids.Application.DependencyInjection;
using MathKids.Game.DependencyInjection;
using MathKids.Infrastructure.DependencyInjection;
using MathKids.Mobile.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace MathKids.Mobile;
// prueba git
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().UseSkiaSharp().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
        builder.Services.AddMathKidsApplication().AddMathKidsInfrastructure().AddMathKidsGame().AddMathKidsMobile();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
