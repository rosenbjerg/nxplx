using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models.Dto.Models;
using NxPlx.Services.Database;
using NxPlx.Services.Database.Models;
using Red.Interfaces;

namespace NxPlx.WebApi.Routers
{
    public static class OverviewRoutes
    {
        public static void Register(IRouter router)
        {
            router.Get("/", async (req, res) =>
            {
                var container = new ResolveContainer();
                var dtoMapper = container.Resolve<IDatabaseMapper>();
                using var ctx = new MediaContext();
                var seriesDetails = await ctx.SeriesDetails.ToListAsync();
                var filmDetails = await ctx.FilmDetails.ToListAsync();

                var overview = new List<OverviewElementDto>(filmDetails.Count + seriesDetails.Count);
                overview.AddRange(dtoMapper.MapMany<DbSeriesDetails, OverviewElementDto>(seriesDetails));
                overview.AddRange(dtoMapper.MapMany<DbFilmDetails, OverviewElementDto>(filmDetails));
                
                return await res.SendJson(overview);
            });
        }
    }
}