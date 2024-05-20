using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiyoTaskManager.Core.Utilities
{
    public class NiyoAPICustomResponseSchema
    {
        public NiyoAPICustomResponseSchema(List<string> errorMessages)
        {
            Code = 400;
            IsError = true;
            ErrorMessages = errorMessages;
        }
        public NiyoAPICustomResponseSchema(List<string> errorMessages, object result)
        {
            Code = 400;
            IsError = true;
            ErrorMessages = errorMessages;
            Result = result;
        }
        public NiyoAPICustomResponseSchema(string message, object result)
        {
            Code = 200;
            Message = message;
            Result = result;
        }
        public NiyoAPICustomResponseSchema(string message)
        {
            Code = 200;
            Message = message;
        }

        public int Code { get; set; }

        public bool IsError { get; set; }

        public List<string>? ErrorMessages { get; set; }

        public string? Message { get; set; }
        public Object? Result { get; set; }
    }
}
