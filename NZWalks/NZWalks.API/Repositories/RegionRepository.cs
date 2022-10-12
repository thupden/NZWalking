using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private NZWalksDbContext nzWalksDbContext;

        public RegionRepository(NZWalksDbContext nzWalksDbContext)
        {
            this.nzWalksDbContext = nzWalksDbContext;
        }
        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            return await nzWalksDbContext.Regions.ToListAsync();
        }
    }
}
