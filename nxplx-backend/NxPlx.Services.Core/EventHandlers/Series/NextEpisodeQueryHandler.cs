using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NxPlx.Application.Core;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Events;
using NxPlx.Application.Models.Events.Series;
using NxPlx.Infrastructure.Database;
using IMapper = AutoMapper.IMapper;

namespace NxPlx.Core.Services.EventHandlers.Series
{
    public class NextEpisodeQueryHandler : IEventHandler<NextEpisodeQuery, NextEpisodeDto?>, IEventHandler<NextEpisodeByFileIdQuery, NextEpisodeDto?>
    {
        private readonly DatabaseContext _context;
        private readonly IEventDispatcher _dispatcher;
        private readonly IMapper _mapper;

        public NextEpisodeQueryHandler(DatabaseContext context, IEventDispatcher dispatcher, IMapper mapper)
        {
            _context = context;
            _dispatcher = dispatcher;
            _mapper = mapper;
        }
        
        public async Task<NextEpisodeDto?> Handle(NextEpisodeQuery @event, CancellationToken cancellationToken = default)
        {
            return @event.Mode.ToLower() switch
            {
                "leastrecent" => await LongestSinceLastWatch(@event.SeriesId, @event.SeasonNo, @event.EpisodeNo),
                "random" => await Random(@event.SeriesId, @event.SeasonNo, @event.EpisodeNo, true),
                "random_in_season" => await Random(@event.SeriesId, @event.SeasonNo, @event.EpisodeNo, false),
                _ => await Default(@event.SeriesId, @event.SeasonNo, @event.EpisodeNo)
            };
        }

        public async Task<NextEpisodeDto?> Handle(NextEpisodeByFileIdQuery @event, CancellationToken cancellationToken = default)
        {
            var episodeFile = await _context.EpisodeFiles.FirstOrDefaultAsync(ef => ef.Id == @event.FileId, cancellationToken);
            if (episodeFile?.SeriesDetailsId == null) return null;
            var newEvent = new NextEpisodeQuery(episodeFile.SeriesDetailsId.Value, episodeFile.SeasonNumber, episodeFile.EpisodeNumber, @event.Mode);
            return await Handle(newEvent, cancellationToken);
        }
        
        private async Task<NextEpisodeDto> Random(int seriesId, int? seasonNo, int? episodeNo, bool allSeasons)
        {
            var available = await _context.EpisodeFiles.Where(ef => 
                    ef.SeriesDetailsId == seriesId
                    && (allSeasons || seasonNo == null || ef.SeasonNumber == seasonNo)
                    && !(ef.SeasonNumber == seasonNo && ef.EpisodeNumber == episodeNo))
                .Select(ef => ef.Id)
                .ToListAsync();
            var selectedIndex = new Random().Next(0, available.Count - 1);
            return await _context.EpisodeFiles
                .Where(ef => ef.Id == available[selectedIndex])
                .ProjectTo<NextEpisodeDto>(_mapper.ConfigurationProvider)
                .SingleAsync();
        }

        private async Task<NextEpisodeDto> LongestSinceLastWatch(int seriesId, int? seasonNo, int? episodeNo)
        {
            var available = await _context.EpisodeFiles.Where(ef =>
                    ef.SeriesDetailsId == seriesId &&
                    (seasonNo == null || ef.SeasonNumber == seasonNo &&
                        (episodeNo == null || ef.EpisodeNumber != episodeNo)))
                .Select(ef => new { ef.Id, ef.EpisodeNumber, ef.SeasonNumber})
                .ToListAsync();
            var availableIds = available.Select(ef => ef.Id).ToList();

            var currentUser = await _dispatcher.Dispatch(new CurrentUserQuery());
            var progress = await _context.WatchingProgresses.AsNoTracking()
                .Where(wp => wp.UserId == currentUser.Id && availableIds.Contains(wp.FileId))
                .ToDictionaryAsync(wp => wp.FileId);

            var selected = available
                .Select(ef => (ef, progress.TryGetValue(ef.Id, out var wp) ? wp.LastWatched : DateTime.MinValue))
                .OrderBy(pair => pair.Item2)
                .ThenBy(pair => pair.ef.SeasonNumber)
                .ThenBy(pair => pair.ef.EpisodeNumber)
                .Select(pair => pair.ef)
                .First();

            return await _context.EpisodeFiles
                .Where(ef => ef.Id == selected.Id)
                .ProjectTo<NextEpisodeDto>(_mapper.ConfigurationProvider)
                .SingleAsync();
        }

        private async Task<NextEpisodeDto> Default(int seriesId, int? seasonNo, int? episodeNo)
        {
            return await _context.EpisodeFiles.Where(ef =>
                    ef.SeriesDetailsId == seriesId &&
                    (seasonNo == null || ef.SeasonNumber > seasonNo ||
                     ef.SeasonNumber == seasonNo && (episodeNo == null || ef.EpisodeNumber > episodeNo)))
                .OrderBy(episode => episode.SeasonNumber)
                .ThenBy(episode => episode.EpisodeNumber)
                .ProjectTo<NextEpisodeDto>(_mapper.ConfigurationProvider)
                .FirstAsync();
        }
    }
}