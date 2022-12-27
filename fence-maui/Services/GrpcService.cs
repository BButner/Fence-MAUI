using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                        Console.WriteLine( "reconnecting..." );
                        while( channel.State != ConnectivityState.Ready )
                        {
                            await channel.ConnectAsync();
                            await Task.Delay( 1000 );
                        }

                        Console.WriteLine( "reconnected" );
                    }
                }
            } );
        }

        // private void InitClient()
        // {
        //     var channel = GrpcChannel.ForAddress( BuildAddress() );
        //     var client = new FenceManager.FenceManagerClient( channel );
        // }

        public async Task<ConfigResponse> GetConfig()
        {
            return await mClient.GetConfigAsync( new Empty() );
        }

        public async Task<IAsyncStreamReader<CursorLocation>> GetCursorLocationStream()
        {
            return mClient.GetCursorLocationStream( new Empty() ).ResponseStream;
        }

        private string BuildAddress() =>
            $"{mConfig.GrpcHost}:{mConfig.GrpcPort}";

        private Config mConfig;
        private FenceManager.FenceManagerClient mClient;
    }
}