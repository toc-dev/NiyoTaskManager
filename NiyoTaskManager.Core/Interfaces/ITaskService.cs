using NiyoTaskManager.Data.DTOs.TaskManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Core.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskBindingDTO>> FetchAllTasksAsync();
        Task<TaskBindingDTO> FetchTaskAsync(string id);
        Task<TaskBindingDTO> CreateTaskAsync(NewTaskDTO model);
        Task UpdateTaskAsync(TaskUpdateDTO model);
        Task DeleteTaskAsync(string id);
    }
}
