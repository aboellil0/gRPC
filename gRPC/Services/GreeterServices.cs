using Grpc.Core;

namespace gRPC.Services
{
    public class GreeterServices 
    {
        private readonly ILogger<GreeterServices> logger;

        public GreeterServices(ILogger<GreeterServices> logger)
        {
            this.logger = logger;
        }
    }
}
