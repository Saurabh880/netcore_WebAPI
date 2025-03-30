using CityInfo.API.Entities;

namespace CityInfo.API.Interface_Repo
{
    public interface ICityInfoRepository
    {
        Task<IQueryable<City>> GetCitiesAsync();
        Task<City?> GetCity(int cityId);
        Task<IQueryable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId,int pointOfInterestId);

    }
}
