using CityInfo.API.Entities;

namespace CityInfo.API.Interface_Repo
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<City?> GetCity(int cityId, bool includePointOfInterest);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId,int pointOfInterestId);

    }
}
    