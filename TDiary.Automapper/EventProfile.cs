using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.Extensions;
using TDiary.Grpc.Protos;

namespace TDiary.Automapper
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, AuditData>()
                .ForMember(ad => ad.CreatedAt, o => o.MapFrom(e => Timestamp.FromDateTime(e.CreatedAt.AsUtc())))
                .ForMember(ad => ad.CreatedAtUtc, o => o.MapFrom(e => Timestamp.FromDateTime(e.CreatedAtUtc.AsUtc())))
                .ForMember(ad => ad.ModifiedAt, o => o.MapFrom(e => Timestamp.FromDateTime(e.ModifiedtAt.AsUtcNullMinimum())))
                .ForMember(ad => ad.ModifiedAtUtc, o => o.MapFrom(e => Timestamp.FromDateTime(e.ModifiedAtUtc.AsUtcNullMinimum())))
                .ForMember(ad => ad.TimeZone, o => o.MapFrom(e => e.TimeZone))
                .ReverseMap()
                .ForMember(ad => ad.CreatedAt, o => o.MapFrom(e => e.CreatedAt.ToDateTime()))
                .ForMember(ad => ad.CreatedAtUtc, o => o.MapFrom(e => e.CreatedAtUtc.ToDateTime()))
                .ForMember(ad => ad.ModifiedtAt, o => o.MapFrom(e => e.ModifiedAt.ToDateTime()))
                .ForMember(ad => ad.ModifiedAtUtc, o => o.MapFrom(e => e.ModifiedAtUtc.ToDateTime()))
                .ForMember(ad => ad.TimeZone, o => o.MapFrom(e => e.TimeZone))
                .ForAllOtherMembers(ad => ad.Ignore());

            CreateMap<EventData, Event>()
                .ForMember(e => e.Id, o => o.MapFrom(ed => Guid.Parse(ed.Id)))
                .ForMember(e => e.Entity, o => o.MapFrom(ed => ed.Entity))
                .ForMember(e => e.EventType, o => o.MapFrom(ed => ed.EventType))
                .ForMember(e => e.Version, o => o.MapFrom(ed => ed.Version))
                .ForMember(e => e.CreatedAt, o => o.MapFrom(ed => ed.AuditData.CreatedAt.ToDateTime()))
                .ForMember(e => e.CreatedAtUtc, o => o.MapFrom(ed => ed.AuditData.CreatedAtUtc.ToDateTime()))
                .ForMember(e => e.ModifiedtAt, o => o.MapFrom(ed => ed.AuditData.ModifiedAt.ToNullMinimumDateTime()))
                .ForMember(e => e.ModifiedAtUtc, o => o.MapFrom(ed => ed.AuditData.ModifiedAtUtc.ToNullMinimumDateTime()))
                .ForMember(e => e.TimeZone, o => o.MapFrom(ed => ed.AuditData.TimeZone))
                .ReverseMap()
                .ForMember(ed => ed.Id, o => o.MapFrom(e => e.Id))
                .ForMember(ed => ed.Entity, o => o.MapFrom(e => e.Entity))
                .ForMember(ed => ed.EventType, o => o.MapFrom(e => e.EventType))
                .ForMember(ed => ed.Version, o => o.MapFrom(e => e.Version))
                .ForMember(ed => ed.AuditData, o => o.MapFrom(e => e))
                .ForMember(ed => ed.Data, o => o.MapFrom(e => e.Data))
                .ForAllOtherMembers(ad => ad.Ignore());
        }
    }
}
