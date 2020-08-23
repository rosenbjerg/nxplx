using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Models;

namespace NxPlx.Core.Services
{
    public class SessionService
    {
        private const string CacheKey = "session";
        private readonly IDistributedCache _distributedCache;

        public SessionService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        
        public Task<Session?> FindSession(string token, CancellationToken cancellationToken = default) => _distributedCache.GetObjectAsync<Session>($"{CacheKey}:{token}", cancellationToken);

        public async Task<SessionDto[]> FindSessions(int userId, CancellationToken cancellationToken = default)
        {
            var sessions = await _distributedCache.GetObjectAsync<List<string>>($"{CacheKey}:{userId}", cancellationToken);
            if (sessions == null)
                return new SessionDto[0];

            var foundSessions = await Task.WhenAll(sessions.Select(async token => (token, session: await FindSession(token, cancellationToken))));
            return foundSessions.Where(s => s.session != null).Select(s => new SessionDto
            {
                Token = s.token,
                UserAgent = s.session!.UserAgent
            }).ToArray()!;
        }

        public async Task AddSession(int userId, string token, Session session, TimeSpan validity, CancellationToken cancellationToken = default)
        {
            var sessions = await _distributedCache.GetObjectAsync<List<string>>($"{CacheKey}:{userId}", cancellationToken);
            sessions ??= new List<string>();
            await _distributedCache.SetObjectAsync($"{CacheKey}:{token}", session, new DistributedCacheEntryOptions
            {
                SlidingExpiration = validity
            }, cancellationToken);
            
            sessions.Add(token);
            await _distributedCache.SetObjectAsync($"{CacheKey}:{userId}", sessions, cancellationToken: cancellationToken);
        }
        
        public async Task RemoveSession(int userId, string token, CancellationToken cancellationToken = default)
        {
            var sessions = await _distributedCache.GetObjectAsync<List<string>>($"{CacheKey}:{userId}", cancellationToken);
            await _distributedCache.RemoveAsync(token, cancellationToken);
            
            if (sessions != null && sessions.Remove(token))
                await _distributedCache.SetObjectAsync($"{CacheKey}:{userId}", sessions, cancellationToken: cancellationToken);
        }
    }
}