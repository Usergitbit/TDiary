using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
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

        public EventService(EventProto.EventProtoClient eventClient, 
            IMapper mapper,
            AuthenticationStateProvider authenticationStateProvider)
        {
            this.eventClient = eventClient;
            this.mapper = mapper;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task Add(Guid userId, Event eventEntity)
        {

        }

        public async Task<List<Event>> Get(Guid userId, DateTime lastEventDateUtc)
        {
            //sync flow:
            // if access token OK => start SYNC process => log last sync date
            // if access token exception => 3. if sts online => go to login
            //                                 if sts offline => skip and check last sync date to show warning if it was a long time ago

            // repository flow
            // if online => sync
            // return data from cache
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
