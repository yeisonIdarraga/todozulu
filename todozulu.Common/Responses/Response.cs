using System;
using System.Collections.Generic;
using System.Text;

namespace todozulu.Common.Responses
{
    public class Response
    {
        public bool IsSuccess { get; set; }

        public string TaskDescription { get; set; }

        public bool IsCompleted { get; set; }
    }
}
