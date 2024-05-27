using NiyoTaskManager.Data.DTOs.Auth;
using NiyoTaskManager.Data.DTOs.TaskManagement;
using NiyoTaskManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Core.Interfaces
{
    public interface IMappingService
    {
        UserBindingDTO MapUserToModel(NiyoUser user);
        TaskBindingDTO MapTasks(NiyoTask task);
    }
}
