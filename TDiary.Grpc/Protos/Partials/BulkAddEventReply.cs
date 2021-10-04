using System;
using System.Collections.Generic;
using System.Text;

namespace TDiary.Grpc.Protos
{
    public partial class BulkAddEventReply
    {
        public BulkAddEventReply(ResultCode resultCode) : base()
        {
            ResultCode = resultCode;
        }

        public BulkAddEventReply(ErrorInformation errorInformation) : base()
        {
            ErrorInfo = errorInformation;
        }

        partial void OnConstruction()
        {
            ResultCode = ResultCode.Ok;
        }
    }
}
