using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using CityInfo.API.Interface_Repo;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _cityInfoContext;

        public CityInfoRepository(CityInfoContext cityInfoContext)
        {
            _cityInfoContext= cityInfoContext ?? throw new ArgumentNullException(nameof(_cityInfoContext ));
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _cityInfoContext.CityInfos.
                OrderBy(c=>c.Name).ToListAsync();
        }

        public async Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            //as we are using pagination no matter what
            //if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(searchQuery))
            //{
            //    return await GetCitiesAsync();
            //}

            var collection = _cityInfoContext.CityInfos as IQueryable<City>;

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if(!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(c => c.Name.Contains(searchQuery) ||
                (c.Description != null && c.Description.Contains(searchQuery)));
            }
            //counting all the items in the collection
            var totalItemCount = await collection.CountAsync();

            //we pass through that totalItemCount and the pageSize and the pageNumber
            var paginationMetaData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            var collectionToReturn =  await collection.OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetaData);
        }

        public async Task<City?> GetCity(int cityId, bool includePointOfInterest)
        {
            if (includePointOfInterest) 
            {
                return await _cityInfoContext.CityInfos.
                    Include( c=> c.PointsOfInterest).
                    Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
            return await _cityInfoContext.CityInfos.
                Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _cityInfoContext.PointsOfInterest.
                Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _cityInfoContext.PointsOfInterest.
                Where(p => p.CityId == cityId).ToListAsync();
        }
        public async Task<bool> IfCityExistsAsync(int cityId)
        {
            return await _cityInfoContext.CityInfos.AnyAsync(c =>c.Id == cityId);
        }

        public async Task AddPointOfInterestForCityAsync(int cityId,  PointOfInterest pointOfInterest)
        {
            var city = await GetCity(cityId,false);
            if (city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _cityInfoContext.SaveChangesAsync() >= 0);
        }
        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _cityInfoContext.PointsOfInterest.Remove(pointOfInterest);
        }
    }
}
