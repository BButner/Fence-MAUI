using System.Collections.ObjectModel;
using System.Collections.Specialized;
using fence_maui.Models;
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

        Loaded += OnLoaded;
    }

    public bool Connected
    {
        get => mConnected;
        set
        {
            mConnected = value;
            OnPropertyChanged();
        }
    }

    public string GrpcTarget { get; private set; }

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

            Connected = true;
            GrpcTarget = mGrpcService.Channel.Target;
            OnPropertyChanged( nameof( GrpcTarget ) );

            var config = await mGrpcService.GetConfig();

            Monitors = new ObservableCollection<Monitor>( config.Monitors );

            BindingContext = this;

            mCursorLocationReader = await mGrpcService.GetCursorLocationStream();

            mGrpcService.ConnectionStatusObservable.Subscribe( status =>
            {
                Dispatcher.Dispatch( async () =>
                {
                    Connected = status == ConnectionStatus.CONNECTED;

                    if( status == ConnectionStatus.CONNECTED )
                    {
                        mCursorLocationReader = await mGrpcService.GetCursorLocationStream();
                        GenerateMonitors();
                        Monitors.CollectionChanged += OnMonitorsChanged;
                    }
                    else
                    {
                        Monitors.CollectionChanged -= OnMonitorsChanged;
                    }
                } );
            } );

            GenerateMonitors();

            Monitors.CollectionChanged += OnMonitorsChanged;
        }
        catch( Exception ex )
        {
            Console.WriteLine( "Exception on connecting on MainPage..." );
            Console.WriteLine( ex );
        }
    }

    private void
        OnMonitorsChanged( object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs ) =>
        GenerateMonitors();

    private void GenerateMonitors()
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

        AbsoluteLayoutDisplays.Children.Add(
            new Controls.Cursor( mCursorLocationReader, topOffset, leftOffset, factor ) );
    }

    private ObservableCollection<Monitor> mMonitors = new();
    private GrpcService mGrpcService;
    private IAsyncStreamReader<CursorLocation> mCursorLocationReader;
    private bool mConnected;
}