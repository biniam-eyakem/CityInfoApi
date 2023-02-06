using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name,
            string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            /* If no filter or search-query is provided return the full result. */
            // if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(searchQuery))
            // {
            //     return await GetCitiesAsync();
            // }

            // collection to construct query on without executing request to database
            var collection = _context.Cities as IQueryable<City>;
            if (!string.IsNullOrEmpty(name))
            {
                collection = collection.Where(c => c.Name.ToLower() == name.Trim().ToLower());
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(c => c.Name.ToLower().Contains(searchQuery) ||
                (c.Description != null && c.Description.ToLower().Contains(searchQuery)));
            }

            var totalItemsCount = await collection.CountAsync();
            var paginationMetaData = new PaginationMetadata(
                    totalItemsCount, pageSize, pageNumber
                );

            var collectionToreturn = await collection
                .OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToreturn, paginationMetaData);
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await this._context.Cities
                    .Include(c => c.PointsofInterest)
                    .OrderBy(c => c.Name)
                    .FirstOrDefaultAsync(c => c.Id == cityId);
            }

            return await this._context.Cities
                .OrderBy(c => c.Name)
                .FirstOrDefaultAsync(c => c.Id == cityId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await this._context.PointsOfInterest
                .Where(p => p.CityId == cityId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await this._context.PointsOfInterest
                .OrderBy(p => p.Name)
                .FirstOrDefaultAsync(p => p.CityId == cityId && p.Id == pointOfInterestId);
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await this._context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointsofInterest.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await this._context.SaveChangesAsync() >= 0);
        }

        public async Task UpdatePointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var existongPointOfInterest = await GetPointOfInterestForCityAsync(cityId, pointOfInterest.Id);
            if (existongPointOfInterest != null)
            {
                existongPointOfInterest.Name = pointOfInterest.Name;
                existongPointOfInterest.Description = pointOfInterest.Description;
            }
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }
        public async Task<bool> CityNameMatchCityId(string? cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }
    }
}
