using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fence_maui.Models;
using FenceHostServer;
using Grpc.Net.Client;

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
            var client = new FenceManager.FenceManagerClient( channel );

            await channel.ConnectAsync();
        }

        // private void InitClient()
        // {
        //     var channel = GrpcChannel.ForAddress( BuildAddress() );
        //     var client = new FenceManager.FenceManagerClient( channel );
        // }

        private string BuildAddress() =>
            $"{mConfig.GrpcHost}:{mConfig.GrpcPort}";

        private Config mConfig;
    }
}