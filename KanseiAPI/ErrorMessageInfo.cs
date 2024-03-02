using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanseiAPI
{
    public class ErrorMessageInfo
    {
        public bool isSuccess { get; set; }
        public bool isErrorEx { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
        public string error_code { get; set; }
    }
}
