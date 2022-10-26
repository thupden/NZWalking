using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private NZWalksDbContext nzWalksDbContext;
        public WalkRepository(NZWalksDbContext nZWalksDbContext) 
        {
            this.nzWalksDbContext = nZWalksDbContext;
        }
        
        public async Task<Walk> AddAsync(Walk walk)
        {
            //Assign new ID
            walk.Id = Guid.NewGuid();

            //adding walk to the database
            await nzWalksDbContext.Walks.AddAsync(walk);
            await nzWalksDbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk> DeleteAsync(Guid id)
        {
            var walk = await nzWalksDbContext.Walks.FindAsync(id);
            if(walk==null)
            {
                return null;
            }
            nzWalksDbContext.Walks.Remove(walk);
            await nzWalksDbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<IEnumerable<Walk>> GetAllAsync()
        {
            return await 
                nzWalksDbContext
                .Walks
                .Include(x=>x.Region)
                .Include(x=>x.WalkDifficulty)
                .ToListAsync();
        }

        public async Task<Walk> GetAsync(Guid id)
        {
            return await nzWalksDbContext
                .Walks
                .Include(x => x.Region)
                .Include(x => x.WalkDifficulty)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk> UpdateAsync(Guid id, Walk walk)
        {
            var existingWalk = await nzWalksDbContext.Walks.FindAsync(id);
            if (existingWalk == null)
                return null;
            existingWalk.Name = walk.Name;
            existingWalk.Length = walk.Length;
            existingWalk.WalkDifficultyId = walk.WalkDifficultyId;
            existingWalk.RegionId = walk.RegionId;

            await nzWalksDbContext.SaveChangesAsync();
            return existingWalk;

        }
    }
}
