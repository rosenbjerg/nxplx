using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Models;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Services.Database;

namespace NxPlx.Services.Index
{
    internal static class IndexingHelperFunctions
    {
        internal static async Task DownloadImages(IDetailsApi detailsApi, IEnumerable<(string, string)> imageDownloads)
        {
            foreach (var (size, url) in imageDownloads
                .Distinct()
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount * 10))
            {
                await detailsApi.DownloadImage(size, url);
            }
        }

        internal static IEnumerable<(string, string)> AccumulateImageDownloads(List<SeriesDetails> details, List<Network> networks, List<ProductionCompany> productionCompanies)
        {
            var imageDownloads = new List<(string, string)>();
            
            foreach (var seriesDetails in details)
            {
                if (!string.IsNullOrEmpty(seriesDetails.PosterPath))
                    imageDownloads.AddRange(ImageInMultipleSizes(seriesDetails.PosterPath, "w342", "w500"));
                if (!string.IsNullOrEmpty(seriesDetails.BackdropPath))
                    imageDownloads.Add(("w1280", seriesDetails.BackdropPath));
                foreach (var season in seriesDetails.Seasons)
                {
                    imageDownloads.AddRange(ImageInMultipleSizes(season.PosterPath, "w342", "w500"));
                    imageDownloads.AddRange(season.Episodes.Select(episode => ("w300", episode.StillPath)));
                }
            }

            imageDownloads.AddRange(networks
                .Where(network => !string.IsNullOrEmpty(network.LogoPath))
                .Select(network => ("w154", network.LogoPath)));
            
            imageDownloads.AddRange(productionCompanies
                .Where(productionCompany => !string.IsNullOrEmpty(productionCompany.LogoPath))
                .Select(productionCompany => ("w154", productionCompany.LogoPath)));
            
            return imageDownloads;
        }
        internal static IEnumerable<(string, string)> AccumulateImageDownloads(List<FilmDetails> details, List<ProductionCompany> productionCompanies, List<MovieCollection> movieCollections)
        {
            var imageDownloads = new List<(string, string)>();
            
            foreach (var filmDetails in details)
            {
                if (!string.IsNullOrEmpty(filmDetails.PosterPath))
                    imageDownloads.AddRange(ImageInMultipleSizes(filmDetails.PosterPath, "w342", "w500"));
                if (!string.IsNullOrEmpty(filmDetails.BackdropPath))
                    imageDownloads.Add(("w1280", filmDetails.BackdropPath));
            }

            imageDownloads.AddRange(productionCompanies
                .Where(productionCompany => !string.IsNullOrEmpty(productionCompany.LogoPath))
                .Select(productionCompany => ("w154", productionCompany.LogoPath)));

            foreach (var movieCollection in movieCollections)
            {
                if (!string.IsNullOrEmpty(movieCollection.PosterPath))
                    imageDownloads.AddRange(ImageInMultipleSizes(movieCollection.PosterPath, "w342", "w500"));
                if (!string.IsNullOrEmpty(movieCollection.BackdropPath))
                    imageDownloads.Add(("w1280", movieCollection.BackdropPath));
            }

            return imageDownloads;
        }

        private static IEnumerable<(string, string)> ImageInMultipleSizes(string url, params string[] sizes)
        {
            return sizes.Select(size => (size, url));
        }

        internal static Task<List<T>> GetUniqueNew<T>(this IEnumerable<T> entities)
            where T : EntityBase
            => GetUniqueNew(entities, e => e.Id);
        internal static async Task<List<T>> GetUniqueNew<T, TKey>(this IEnumerable<T> entities, Expression<Func<T, TKey>> keySelector)
            where T : class
        {
            var keySelectorFunc = keySelector.Compile();
            var unique = GetUnique(entities, keySelectorFunc);
            return await GetNew(unique, keySelector);
        }
        
        internal static Task<List<T>> GetNew<T>(this IEnumerable<T> entities)
            where T : EntityBase
            => GetNew(entities, e => e.Id);

        private static async Task<List<T>> GetNew<T, TKey>(this IEnumerable<T> entities, Expression<Func<T, TKey>> keySelector)
            where T : class
        {
            List<TKey> unique;
            using (var ctx = new MediaContext())
            {
                unique = await ctx.Set<T>().Select(keySelector).ToListAsync();
            }

            var uniqueHashset = unique.ToHashSet();

            var keySelectorFunc = keySelector.Compile();
            return entities.Where(e => !uniqueHashset.Contains(keySelectorFunc(e))).ToList();
        }

        private static List<T> GetUnique<T, TKey>(this IEnumerable<T> entities, Func<T, TKey> keySelector)
            where T : class
        {
            var unique = new Dictionary<TKey, T>();
            foreach (var entity in entities)
            {
                if (entity == null) 
                    continue;
                var key = keySelector(entity);
                unique[key] = entity;
            }

            return unique.Values.ToList();
        }
    }
}