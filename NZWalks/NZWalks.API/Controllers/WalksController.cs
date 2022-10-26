using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController: Controller
    {
        private readonly IWalkRepository walksRepository;
        private readonly IMapper mapper;
        private readonly IRegionRepository regionRepository;
        private readonly IwalkDifficultyRepository walkDifficultyRepository;

        public WalksController(
            IWalkRepository walksRepository, 
            IMapper mapper, 
            IRegionRepository regionRepository,
            IwalkDifficultyRepository walkDifficultyRepository) 
        {
            this.walksRepository = walksRepository;
            this.mapper = mapper;
            this.regionRepository = regionRepository;
            this.walkDifficultyRepository = walkDifficultyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalksAsync()
        {
            //Fetch data from database
            var walks = await walksRepository.GetAllAsync();

            //convert domain walks to dto walks
            var walksDTO = mapper.Map<List<Models.DTO.Walk>>(walks);

            //return response
            return Ok(walksDTO);
        }

       [HttpGet]
       [Route("{id:guid}")]
       [ActionName("GetWalkAsync")]
       public async Task<IActionResult> GetWalkAsync(Guid id)
        {
            //Get walk domain oject from database
            var walkDomain = await walksRepository.GetAsync(id);

            //convert domain object to DTO
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            //return DTO
            return Ok(walkDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync([FromBody] Models.DTO.AddWalksRequest addWalkRequest)
        {
            //Validate the incoming request
            if(!(await ValidateAddWalkAsync(addWalkRequest)))
            {
                return BadRequest(ModelState);
            }
            //Convert DTO to Domain Object
            var walkDomain = mapper.Map<Models.Domain.Walk>(addWalkRequest);

            //Pass domain object to Repository to persist this
            walkDomain = await walksRepository.AddAsync(walkDomain);

            //Convert the Domain object back to DTO
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);

            //Send DTO response back to Client

            return CreatedAtAction(nameof(GetWalkAsync), new { id = walkDTO.Id }, walkDTO);

        }


        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id, [FromBody] Models.DTO.UpdateWalkRequest updateWalkdRequest)
        {
            //validate the incoming request
            if(!(await ValidateUpdateWalkAsync(updateWalkdRequest)))
            {
                return BadRequest(ModelState);
            }
            //Convert DTO to Domain Object
            var walkDomain = mapper.Map<Models.Domain.Walk>(updateWalkdRequest);
            //Padd details to repository
            walkDomain = await walksRepository.UpdateAsync(id, walkDomain);
            //handle null (not found)
            if (walkDomain == null)
                return NotFound();
            //convert domain to dto
            var walkDTO = mapper.Map<Models.DTO.Walk>(walkDomain);
            //return response
            return Ok(walkDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAsync(Guid id)
        {
            var walk = await walksRepository.DeleteAsync(id);
            if(walk==null)
            {
                return NotFound();
            }
            else
            {
                var walkDTO = mapper.Map<Models.DTO.Walk>(walk);
                return Ok(walkDTO);
            }

        }

        //Validation for post method
        #region Private methods
        private async Task<bool> ValidateAddWalkAsync(Models.DTO.AddWalksRequest addWalkRequest)
        {
            if(addWalkRequest == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest),
                    $"{nameof(addWalkRequest)} cannor be empty.");
                return false;
            }

            if(string.IsNullOrWhiteSpace(addWalkRequest.Name))
            {
                ModelState.AddModelError(nameof(addWalkRequest.Name),
                    $"{nameof(addWalkRequest.Name)} name cannot be empty or white space");
            }
            if (addWalkRequest.Length<=0)
            {
                ModelState.AddModelError(nameof(addWalkRequest.Length),
                    $"{nameof(addWalkRequest.Length)} length should be greater then zero");
            }

            if (string.IsNullOrWhiteSpace(addWalkRequest.Name))
            {
                ModelState.AddModelError(nameof(addWalkRequest.Name),
                    $"{nameof(addWalkRequest.Name)} name cannot be empty or white space");
            }

            var region = await regionRepository.GetAsync(addWalkRequest.RegionId);
            if(region==null)
            {
                    ModelState.AddModelError(nameof(addWalkRequest.RegionId),
                        $"{nameof(addWalkRequest.RegionId)} is invalid.");
            }

            var walkDifficulty = await walkDifficultyRepository.GetAsync(addWalkRequest.WalkDifficultyId);

            if(walkDifficulty==null)
            {
                    ModelState.AddModelError(nameof(addWalkRequest.WalkDifficultyId),
                        $"{nameof(addWalkRequest.WalkDifficultyId)} is invalid");
            }
            if(ModelState.ErrorCount>0)
            {
                return false;
            }
            return true;

        }

        //validation of update method
        private async Task<bool> ValidateUpdateWalkAsync(Models.DTO.UpdateWalkRequest updateWalkRequest)
        {

            {
                if (updateWalkRequest == null)
                {
                    ModelState.AddModelError(nameof(updateWalkRequest),
                        $"{nameof(updateWalkRequest)} cannor be empty.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(updateWalkRequest.Name))
                {
                    ModelState.AddModelError(nameof(updateWalkRequest.Name),
                        $"{nameof(updateWalkRequest.Name)} name cannot be empty or white space");
                }
                if (updateWalkRequest.Length <= 0)
                {
                    ModelState.AddModelError(nameof(updateWalkRequest.Length),
                        $"{nameof(updateWalkRequest.Length)} length should be greater then zero");
                }

                var region = await regionRepository.GetAsync(updateWalkRequest.RegionId);
                if (region == null)
                {
                    ModelState.AddModelError(nameof(updateWalkRequest.RegionId),
                        $"{nameof(updateWalkRequest.RegionId)} is invalid.");
                }

                var walkDifficulty = await walkDifficultyRepository.GetAsync(updateWalkRequest.WalkDifficultyId);

                if (walkDifficulty == null)
                {
                    ModelState.AddModelError(nameof(updateWalkRequest.WalkDifficultyId),
                        $"{nameof(updateWalkRequest.WalkDifficultyId)} is invalid");
                }
                if (ModelState.ErrorCount > 0)
                {
                    return false;
                }
                return true;

            }
        }

        #endregion
    }
}
