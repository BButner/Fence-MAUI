using System.Diagnostics;
using CommandLine;
using fence_maui.Models;
using fence_maui.Services;
using Microsoft.Extensions.Logging;

namespace fence_maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts( fonts =>
            {
                fonts.AddFont( "OpenSans-Regular.ttf", "OpenSansRegular" );
                fonts.AddFont( "OpenSans-Semibold.ttf", "OpenSansSemibold" );
            } );

// #if DEBUG
        builder.Logging.AddDebug();
// #endif

        var arguments = new Arguments();
        Parser.Default.ParseArguments<Arguments>( Environment.GetCommandLineArgs() )
            .WithParsed( parsed => arguments = parsed );

        var configService = new ConfigService( arguments.Config );

        builder.Services.AddSingleton( configService );
        builder.Services.AddSingleton<GrpcService>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}