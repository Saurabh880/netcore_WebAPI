using CityInfo.API.Entities;
using CityInfo.API.Interface_Repo;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        Task<IQueryable<City>> ICityInfoRepository.GetCitiesAsync()
        {
            throw new NotImplementedException();
        }

        Task<City?> ICityInfoRepository.GetCity(int cityId)
        {
            throw new NotImplementedException();
        }

        Task<PointOfInterest?> ICityInfoRepository.GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            throw new NotImplementedException();
        }

        Task<IQueryable<PointOfInterest>> ICityInfoRepository.GetPointsOfInterestForCityAsync(int cityId)
        {
            throw new NotImplementedException();
        }
    }
}
