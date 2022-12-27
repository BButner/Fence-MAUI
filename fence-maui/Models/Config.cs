using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fence_maui.Models
{
    public class Config
    {
        /// <summary>
        /// Hostname (with http:// or https://) of the target gRPC server
        /// </summary>
        public string GrpcHost { get; set; } = "http://localhost";

        /// <summary>
        /// Port of the target gRPC server
        /// </summary>
        public int GrpcPort { get; set; } = 50051;
    }
}