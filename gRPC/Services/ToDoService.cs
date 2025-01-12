using gRPC.Models;
using gRPC.Protos;

namespace gRPC.Services
{
    public class ToDoService : ToDoIt.ToDoItBase
    {
        private readonly Logger<ToDoService> logger;

        public ToDoService(Logger<ToDoService> logger)
        {
            this.logger = logger;
        }
    }
}
