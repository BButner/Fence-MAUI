using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using fence_maui.Models;
using FenceHostServer;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Balancer;

namespace fence_maui.Services
{
    public class GrpcService
    {
        public GrpcService( ConfigService config )
        {
            mConfig = config.Config;
        }

        public async Task Connect()
        {
            var channel = GrpcChannel.ForAddress( BuildAddress() );
            mClient = new FenceManager.FenceManagerClient( channel );

            await channel.ConnectAsync();

            var _ = Task.Run( async () =>
            {
                while( true )
                {
                    await channel.WaitForStateChangedAsync( channel.State );

                    Console.WriteLine( $"State changed to {channel.State}" );
                    if( channel.State != ConnectivityState.Ready )
                    {
                        mConnectionStatusSubject.OnNext( ConnectionStatus.DISCONNECTED );
                        Console.WriteLine( "reconnecting..." );
                        while( channel.State != ConnectivityState.Ready )
                        {
                            await channel.ConnectAsync();
                            await Task.Delay( 1000 );
                        }

                        mConnectionStatusSubject.OnNext( ConnectionStatus.CONNECTED );
                        Console.WriteLine( "reconnected" );
                    }
                }
            } );
        }

        public async Task<ConfigResponse> GetConfig()
        {
            return await mClient.GetConfigAsync( new Empty() );
        }

        public async Task<IAsyncStreamReader<CursorLocation>> GetCursorLocationStream()
        {
            return mClient.GetCursorLocationStream( new Empty() ).ResponseStream;
        }

        public IObservable<ConnectionStatus> ConnectionStatusObservable => mConnectionStatusSubject;

        private string BuildAddress() =>
            $"{mConfig.GrpcHost}:{mConfig.GrpcPort}";

        private Config mConfig;
        private FenceManager.FenceManagerClient mClient;
        private Subject<ConnectionStatus> mConnectionStatusSubject = new();
    }

    public enum ConnectionStatus
    {
        CONNECTED,
        DISCONNECTED
    }
}