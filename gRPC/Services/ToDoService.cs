using Azure;
using gRPC.data;
using gRPC.Models;
using gRPC.Protos;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace gRPC.Services
{
    public class ToDoService : todoit.todoitBase
    {
        private readonly ILogger<ToDoService> logger;
        private readonly AppDbContext _context;

        public ToDoService(ILogger<ToDoService> logger, AppDbContext context)
        {
            this.logger = logger;
            this._context = context;
            logger.LogInformation("klkjhgfhjklkjhgfdfghjkljhgfdghj");
        }

        public override async Task<CreateToDoRes> CreateToDo(CreateToDoReq req, ServerCallContext callContext)
        {
            logger.LogInformation("Task is Running");
            try
            {

                if (req.Title == string.Empty || req.Description == string.Empty)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "you must supply a valied object"));
                }

                logger.LogInformation($"title = {req.Title} Description = {req.Description}");

                var ToDoItem = new Todo
                {
                    Name = req.Title,
                    Description = req.Description,
                };

                await _context.AddAsync(ToDoItem);
                await _context.SaveChangesAsync();


                return await Task.FromResult(new CreateToDoRes
                {
                    Id = ToDoItem.Id,
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while creating ToDo item.");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while creating the ToDo item."));
            }
        }

        public override async Task<ReadTodoRes> ReadTodo(ReadTodoReq req, ServerCallContext callContext)
        {
            try
            {
                if (req.Id <= 0)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Please add the id"));
                }

                var todoitem = await _context.Todos.FirstOrDefaultAsync(t => t.Id == req.Id);

                if (todoitem != null)
                {
                    return await Task.FromResult(new ReadTodoRes
                    {
                        Id = todoitem.Id,
                        Title = todoitem.Name,
                        Description = todoitem.Description,
                        Status = todoitem.Status,
                    });
                }
                throw new RpcException(new Status(StatusCode.NotFound, "i cannot find item with this id"));
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Error while creating ToDo item.");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while creating the ToDo item."));
            }
        }


        public override async Task<UpdateToDoRes> UpdateToDo(UpdateToDoReq req, ServerCallContext callContext)
        {
            if (req.Id <=0 )
            {
                throw new RpcException(new Status(StatusCode.DataLoss,"not found"));
            }

            var itemToEdit = await _context.Todos.FirstOrDefaultAsync(t=>t.Id == req.Id);
            if (itemToEdit == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "there is no somthing with this id"));
            }

            if (req.Title == string.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Title"));
            }

            if (req.Description == string.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Title"));
            }

            itemToEdit.Name = req.Title;
            itemToEdit.Description = req.Description;
            itemToEdit.Status = req.Status;
            await _context.SaveChangesAsync();

            return await Task.FromResult(new UpdateToDoRes { Id = itemToEdit.Id });
        }


        public override async Task<GetAllRes> ListToDo(GetAllReq req,ServerCallContext callContext)
        {
            var response = new GetAllRes();
            var items = await _context.Todos.ToListAsync();

            foreach (var item in items)
            {
                response.ToDo.Add(new ReadTodoRes
                {
                    Id = item.Id,
                    Title = item.Name,
                    Description = item.Description,
                    Status = item.Status,
                });
            }

            return await Task.FromResult(response);
        }


        public override async Task<DeleteToDoRes> DeleteToDo(DeleteToDoReq req,ServerCallContext callContext)
        {
            if (req.Id <= 0)
            {
                throw new RpcException(new Status(StatusCode.DataLoss, "id more than 1 please"));
            }

            var item = await _context.Todos.FirstOrDefaultAsync(e=>e.Id == req.Id);
            if (item == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,$"there is no id == {req.Id}"));
            }

            _context.Todos.Remove(item);
            await _context.SaveChangesAsync();

            return await Task.FromResult(new DeleteToDoRes { Id = item.Id});
        }
    }
}
