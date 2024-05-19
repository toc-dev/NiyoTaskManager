using NiyoTaskManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Data.DTOs.TaskManagement
{
    public class TaskBindingDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public NiyoUser? User { get; set; }
    }
}
