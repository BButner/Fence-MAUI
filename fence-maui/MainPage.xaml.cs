using System.Collections.ObjectModel;
using fence_maui.Services;
using FenceHostServer;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Maui.Controls.Shapes;
using Monitor = FenceHostServer.Monitor;

namespace fence_maui;

public partial class MainPage : ContentPage
{
    public MainPage( ConfigService config, GrpcService grpc )
    {
        InitializeComponent();

        mGrpcService = grpc;

        // Console.WriteLine( config.Config );

        // var channel = GrpcChannel.ForAddress( "http://localhost:50052" );
        // var client = new FenceManager.FenceManagerClient( channel );
        //
        // var config = client.GetConfig( new Empty() );
        //
        // Monitors = new ObservableCollection<Monitor>( config.Monitors );
        //
        // BindingContext = this;
        //
        // var reader = client.GetCursorLocationStream( new Empty() )
        //     .ResponseStream;
        //
        // Monitors.CollectionChanged += ( _, _ ) => GenerateMonitors( reader );
        //
        // GenerateMonitors( reader );

        Loaded += OnLoaded;
    }

    public ObservableCollection<Monitor> Monitors
    {
        get => mMonitors;
        set
        {
            mMonitors = value;
            OnPropertyChanged();
        }
    }

    private async void OnLoaded( object sender, EventArgs e )
    {
        try
        {
            await mGrpcService.Connect();
        }
        catch( Exception ex )
        {
            Console.WriteLine( ex );
        }
    }

    private void GenerateMonitors( IAsyncStreamReader<CursorLocation> reader )
    {
        AbsoluteLayoutDisplays.Children.Clear();

        double factor = 0.125;

        var lowestLeft = Monitors.Min( m => m.Left );
        var lowestTop = Monitors.Min( m => m.Top );

        double leftOffset = lowestLeft < 0 ? lowestLeft : 0;
        double topOffset = lowestTop < 0 ? lowestTop : 0;

        double maxWidth = Monitors.Max( m => ( m.Width + ( m.Left - leftOffset ) ) * factor );
        double maxHeight = Monitors.Max( m => ( m.Height + ( m.Top - topOffset ) ) * factor );

        AbsoluteLayoutDisplays.WidthRequest = maxWidth;
        AbsoluteLayoutDisplays.HeightRequest = maxHeight;

        foreach( var monitor in Monitors )
        {
            var monitorControl = new Controls.Monitor( monitor );

            AbsoluteLayout.SetLayoutBounds( monitorControl,
                new Rect(
                    ( monitor.Left - leftOffset ) * factor,
                    ( monitor.Top - topOffset ) * factor,
                    monitor.Width * factor,
                    monitor.Height * factor
                ) );

            AbsoluteLayoutDisplays.Children.Add( monitorControl );
        }

        AbsoluteLayoutDisplays.Children.Add( new Controls.Cursor( reader, topOffset, leftOffset, factor ) );
    }

    int count = 0;

    private ObservableCollection<Monitor> mMonitors = new();
    private string mCoordinates = "0,0";
    private GrpcService mGrpcService;
}