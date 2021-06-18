using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FoodDiary.API.Data;
using FoodDiary.API.Dtos;
using FoodDiary.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDiary.API.Controllers
{
    [Authorize]
    [Route("api/{userId}/weights")]
    [ApiController]
    public class WeightController : ControllerBase
    {
        private readonly IWeightRepository _weightRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        
        public WeightController (IWeightRepository weightRepo, IUserRepository userRepo, IMapper mapper)
        {
            _weightRepo = weightRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWeight(int userId, WeightForCreationDto weightForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            weightForCreationDto.UserId = userId;
            var weight = _mapper.Map<Weight>(weightForCreationDto);
            var user = await _userRepo.GetUser(userId);
            user.Weights.Add(weight);

            if (await _userRepo.SaveAll())
            {
                return StatusCode(201);
            }
            return BadRequest("Could not add the weight");
        }

        [HttpDelete("{weightId}")]
        public async Task<IActionResult> DeleteWeight(int userId, int weightId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var meal = await _weightRepo.GetWeight(weightId);

            if (meal == null)
                return BadRequest($"Weight with id: {weightId} is not in the databse");

            if (userId != meal.UserId)
                return Unauthorized();

            _weightRepo.Delete(meal);

            if (await _weightRepo.SaveAll())
                return Ok();

            return BadRequest($"Failed to delete the weight id: {weightId}");
        }

        [HttpGet]
        public async Task<IActionResult> GetWeights(int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var weights = await _weightRepo.GetWeights(userId);
            var weightsToReturn = _mapper.Map<IEnumerable<WeightForListDto>>(weights);

            return Ok(weightsToReturn);
        }

        [HttpGet("{weightId}")]
        public async Task<IActionResult> GetWeight(int userId, int weightId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var weight = await _weightRepo.GetWeight(weightId);

            if (weight == null)
                return BadRequest($"There is not weight id: {weightId} in the database");
            
            var weightToReturn = _mapper.Map<WeightForListDto>(weight);

            return Ok(weightToReturn);
        }

        [HttpPut("{weightId}")]
        public async Task<IActionResult> UpdateWeight(int userId, int weightId, WeightForUpdateDto weightForUpdateDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var weightFromRepo = await _weightRepo.GetWeight(weightId);

            _mapper.Map(weightForUpdateDto, weightFromRepo);

            if (await _weightRepo.SaveAll())
                return Ok();

            return BadRequest($"Updating weight {weightId} failed on save");   
        } 


    }
}