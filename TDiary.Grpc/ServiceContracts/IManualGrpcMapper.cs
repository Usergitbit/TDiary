using System;
using System.Collections.Generic;
using System.Text;
using TDiary.Common.Models.Entities;
using TDiary.Grpc.Protos;

namespace TDiary.Grpc.ServiceContracts
{
    public interface IManualGrpcMapper
    {
        EventData Map(Event eventEntity);
        Event Map(EventData eventData);
    }
}
