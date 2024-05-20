using NiyoTaskManager.Core.Interfaces;
using NiyoTaskManager.Data.DTOs.TaskManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Core.Implementations
{
    public class TaskService : ITaskService
    {
        public Task<TaskBindingDTO> CreateTaskAsync(NewTaskDTO model)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTaskAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<TaskBindingDTO>> FetchAllTasksAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TaskBindingDTO> FetchTaskAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTaskAsync(TaskUpdateDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
