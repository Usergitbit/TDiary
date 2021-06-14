using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Api.Protos;
using TDiary.Common.Models.Entities;

namespace TDiary.Api.Mappings
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EventData, Event>()
                .ForMember(e => e.Id, o => o.MapFrom(ed => Guid.Parse(ed.Id)))
                .ForMember(e => e.Entity, o => o.MapFrom(ed => ed.Entity))
                .ForMember(e => e.EventType, o => o.MapFrom(ed => ed.EventType))
                .ForMember(e => e.Version, o => o.MapFrom(ed => ed.Version))
                .ForMember(e => e.LocallyCreatedAt, o => o.MapFrom(ed => ed.AuditData.LocallyCreatedAt.ToDateTime()))
                .ForMember(e => e.LocallyCreatedAtUtc, o => o.MapFrom(ed => ed.AuditData.LocallyCreatedAtUtc.ToDateTime()))
                .ForMember(e => e.LocallyModifiedAt, o => o.MapFrom(ed => ed.AuditData.LocallyModifiedAt.ToDateTime()))
                .ForMember(e => e.LocallyModifiedAtUtc, o => o.MapFrom(ed => ed.AuditData.LocallyModifiedAtUtc.ToDateTime()))
                .ForMember(e => e.TimeZone, o => o.MapFrom(ed => TimeZoneInfo.FromSerializedString(ed.AuditData.TimeZone)));



        }
    }
}
