using NiyoTaskManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Data.DTOs.TaskManagement
{
    public class NewTaskDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; }
    }
}
