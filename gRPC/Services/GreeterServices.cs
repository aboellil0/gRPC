using Grpc.Core;

namespace gRPC.Services
{
    public class GreeterServices : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterServices> logger;

        public GreeterServices(ILogger<GreeterServices> logger)
        {
            this.logger = logger;
        }

        public override 
    }
}
