using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class WalkDifficultyRepository : IwalkDifficultyRepository
    {
        private NZWalksDbContext nzWalksDbContext;
        public WalkDifficultyRepository(NZWalksDbContext nzWalksDbContext)
        {
            this.nzWalksDbContext = nzWalksDbContext;
        }

        public async Task<WalkDifficulty> AddAsync(WalkDifficulty walkDifficulty)
        {
            walkDifficulty.Id = Guid.NewGuid();
            await nzWalksDbContext.AddAsync(walkDifficulty);
            await nzWalksDbContext.SaveChangesAsync();
            return walkDifficulty;
        }

        public async Task<WalkDifficulty> DeleteAsync(Guid id)
        {
            var walkDifficulty = await nzWalksDbContext.WalkDifficulty.FirstOrDefaultAsync(x => x.Id == id);
            if(walkDifficulty==null)
            {
                return null;
            }
            nzWalksDbContext.WalkDifficulty.Remove(walkDifficulty);
            await nzWalksDbContext.SaveChangesAsync();
            return walkDifficulty;
        }

        public async Task<IEnumerable<WalkDifficulty>> GetAllAsync()
        {
            return await nzWalksDbContext.WalkDifficulty.ToListAsync();
        }

        public async Task<WalkDifficulty> GetAsync(Guid id)
        {
            return await nzWalksDbContext.WalkDifficulty.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<WalkDifficulty> UpdateAsync(Guid id, WalkDifficulty walkDifficulty)
        {
            var existingWalkDifficulty = await nzWalksDbContext.WalkDifficulty.FirstOrDefaultAsync(x => x.Id == id);
            if(existingWalkDifficulty==null)
            {
                return null;
            }
          
            existingWalkDifficulty.Code = walkDifficulty.Code;
            await nzWalksDbContext.SaveChangesAsync();
            return existingWalkDifficulty;
        }
    }
}
