using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Core.Utilities
{
    public class TaskHub: Hub
    {
        public async Task SendTaskUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveTaskUpdate", message);
        }
    }
}
