using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using todozulu.Common.Models;
using todozulu.Common.Responses;
using todozulu.Functions.Entitis;

namespace todozulu.Functions
{
    public static class TodoApi
    {
        [FunctionName(nameof(CreateTodo))]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "Azure.WebJobs.Extensions.Storage")] CloudTable todoTable,
            ILogger log)
        {


            log.LogInformation("Receive a new todo");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            if (string.IsNullOrEmpty(todo?.TaskDescription))
            {

                return new BadRequestObjectResult(new Response
                {

                    IsSuccess = false,
                    Message = "the requestmust have a task description"
                });

            }

            TodoEntity todoEntity = new TodoEntity
            {

                CreatedTime = DateTime.UtcNow,
                ETag = "*",
                IsCompleted = false,
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                TaskDescription = todo.TaskDescription
            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await todoTable.ExecuteAsync(addOperation);


            string message = "New todo store in table";
            log.LogInformation(message);
            return new OkObjectResult(new Response
            {

                IsSuccess = true,
                Message = message,
                Result = todoEntity

            });



        }

    }
}