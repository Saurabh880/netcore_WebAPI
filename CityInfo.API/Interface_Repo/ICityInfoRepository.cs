﻿using CityInfo.API.Entities;
using CityInfo.API.Services;

namespace CityInfo.API.Interface_Repo
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery,int pageNumber,int pageSize);
        Task<City?> GetCity(int cityId, bool includePointOfInterest);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId,int pointOfInterestId);
        Task<bool> IfCityExistsAsync(int cityId);

        Task AddPointOfInterestForCityAsync(int cityId ,PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();
        
        void DeletePointOfInterest(PointOfInterest pointOfInterest);

    }
}
    