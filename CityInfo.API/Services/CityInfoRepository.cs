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
    }
}
