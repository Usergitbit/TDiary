using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Api.Protos;
using TDiary.Common.Models.Entities;
using TDiary.Common.Extensions;

namespace TDiary.Api.Mappings
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {

            CreateMap<Event, AuditData>()
                .ForMember(ad => ad.LocallyCreatedAt, o => o.MapFrom(e => Timestamp.FromDateTime(e.LocallyCreatedAt.AsUtc())))
                .ForMember(ad => ad.LocallyCreatedAtUtc, o => o.MapFrom(e => Timestamp.FromDateTime(e.LocallyCreatedAtUtc.AsUtc())))
                .ForMember(ad => ad.LocallyModifiedAt, o => o.MapFrom(e => Timestamp.FromDateTime(e.LocallyModifiedAt.AsUtcNullMinimum())))
                .ForMember(ad => ad.LocallyModifiedAtUtc, o => o.MapFrom(e => Timestamp.FromDateTime(e.LocallyModifiedAtUtc.AsUtcNullMinimum())))
                .ForMember(ad => ad.TimeZone, o => o.MapFrom(e => e.TimeZone.ToSerializedString()))
                .ForAllOtherMembers(ad => ad.Ignore());

            CreateMap<EventData, Event>()
                .ForMember(e => e.Id, o => o.MapFrom(ed => Guid.Parse(ed.Id)))
                .ForMember(e => e.Entity, o => o.MapFrom(ed => ed.Entity))
                .ForMember(e => e.EventType, o => o.MapFrom(ed => ed.EventType))
                .ForMember(e => e.Version, o => o.MapFrom(ed => ed.Version))
                .ForMember(e => e.LocallyCreatedAt, o => o.MapFrom(ed => ed.AuditData.LocallyCreatedAt.ToDateTime()))
                .ForMember(e => e.LocallyCreatedAtUtc, o => o.MapFrom(ed => ed.AuditData.LocallyCreatedAtUtc.ToDateTime()))
                .ForMember(e => e.LocallyModifiedAt, o => o.MapFrom(ed => ed.AuditData.LocallyModifiedAt.ToDateTime()))
                .ForMember(e => e.LocallyModifiedAtUtc, o => o.MapFrom(ed => ed.AuditData.LocallyModifiedAtUtc.ToDateTime()))
                .ForMember(e => e.TimeZone, o => o.MapFrom(ed => TimeZoneInfo.FromSerializedString(ed.AuditData.TimeZone)))
                .ReverseMap()
                .ForMember(ed => ed.Id, o => o.MapFrom(e => e.Id))
                .ForMember(ed => ed.Entity, o => o.MapFrom(e => e.Entity))
                .ForMember(ed => ed.EventType, o => o.MapFrom(e => e.EventType))
                .ForMember(ed => ed.Version, o => o.MapFrom(e => e.Version))
                .ForMember(ed => ed.AuditData, o => o.MapFrom(e => e))
                .ForAllOtherMembers(ad => ad.Ignore());
        }
    }
}
