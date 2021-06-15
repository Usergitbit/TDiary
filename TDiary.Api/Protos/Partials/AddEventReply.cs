using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Api.Exceptions;

namespace TDiary.Api.Protos
{
    public partial class AddEventReply
    {
        public AddEventReply(ResultCode resultCode) : base()
        {
            ResultCode = resultCode;
        }

        public AddEventReply(ErrorInfo errorInfo) : base()
        {
            ErrorInfo = errorInfo;
        }

        partial void OnConstruction()
        {
            ResultCode = ResultCode.Ok;
        }
    }
}
