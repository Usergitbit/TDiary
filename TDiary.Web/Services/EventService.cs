using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDiary.Common.Models.Entities;
using TDiary.Common.ServiceContracts;
using TDiary.Grpc.Protos;

namespace TDiary.Web.Services
{
    public class EventService : IEventService
    {
        private readonly EventProto.EventProtoClient eventClient;
        private readonly IMapper mapper;
        private readonly AuthenticationStateProvider authenticationStateProvider;

        public EventService(EventProto.EventProtoClient eventClient, IMapper mapper, AuthenticationStateProvider authenticationStateProvider)
        {
            this.eventClient = eventClient;
            this.mapper = mapper;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public Task Add(Guid userId, Event eventEntity)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Event>> Get(Guid userId, DateTime lastEventDateUtc)
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var claims = user.Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "id").Value;
            userId = Guid.Parse(userIdClaim);

            var reply = await eventClient.GetEventsAsync(new GetEventsRequest
            {
                LastEventDateUtc = Timestamp.FromDateTime(DateTime.UtcNow.AddYears(-100))
            });
            var events = mapper.Map<List<Event>>(reply.EventData);

            return events;
        }
    }
}
