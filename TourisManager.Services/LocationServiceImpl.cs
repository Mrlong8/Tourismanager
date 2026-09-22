using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TourisManager.Core.Entities;
using TourisManager.Data;

namespace TourisManager.Services
{
    public class LocationServiceImpl : LocationService
    {
        private readonly AppDbContext _db;

        public LocationServiceImpl(AppDbContext db)
        {
            _db = db;
        }

        public List<Location> GetDataAll()
        {
            return _db.Locations.AsNoTracking().ToList();
        }

    }
}
