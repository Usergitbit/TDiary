﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDiary.Grpc.Protos
{
    public partial class AddEventReply
    {
        public AddEventReply(ResultCode resultCode) : base()
        {
            ResultCode = resultCode;
        }

        public AddEventReply(ErrorInformation errorInformation) : base()
        {
            ErrorInfo = errorInformation;
        }

        partial void OnConstruction()
        {
            ResultCode = ResultCode.Ok;
        }
    }
}
