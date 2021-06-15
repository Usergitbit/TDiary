using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Api.Exceptions;

namespace TDiary.Api.Protos
{
    public partial class AddEventReply
    {
        public AddEventReply(string message) : base()
        {
            Message = message;
        }
        public AddEventReply(ErrorInfo errorInfo) : base()
        {
            ErrorInfo = errorInfo;
        }
    }
}
