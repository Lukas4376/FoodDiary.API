using System;
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
    [Route("api/{userId}/meals")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly IMealRepository _mealRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public MealController(IMealRepository mealRepo, IUserRepository userRepo, IMapper mapper)
        {
            _mealRepo = mealRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMeal(int userId, MealForCreationDto mealForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            mealForCreationDto.UserId = userId;
            var meal = _mapper.Map<Meal>(mealForCreationDto);
            meal.Created = DateTime.Now;
            var user = await _userRepo.GetUser(userId);
            user.Meals.Add(meal);

            if (await _userRepo.SaveAll())
            {
                return StatusCode(201);
            }
            return BadRequest("Could not add the meal");
        }

        [HttpDelete("{mealId}")]
        public async Task<IActionResult> DeleteMeal(int userId, int mealId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var meal = await _mealRepo.GetMeal(mealId);

            if (meal == null)
                return BadRequest($"Meal with id: {mealId} is not in the databse");

            if (userId != meal.UserId)
                return Unauthorized();

            _mealRepo.Delete(meal);

            if (await _mealRepo.SaveAll())
                return Ok();

            return BadRequest($"Failed to delete the meal id: {mealId}");
        }

        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetMealsByDate(int userId, string date)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var parsedDate = DateTime.Parse(date);
            var meals = await _mealRepo.GetMealsByDate(parsedDate, userId);
            var mealsToReturn = _mapper.Map<IEnumerable<MealsForListDto>>(meals);

            return Ok(mealsToReturn);
        }

        [HttpGet("{mealId}")]
        public async Task<IActionResult> GetMeal(int userId, int mealId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var meal = await _mealRepo.GetMeal(mealId);
            
            if (meal == null)
                return BadRequest($"There is not meal id: {mealId} in the database");
            
            var mealToReturn = _mapper.Map<MealToReturnDto>(meal);

            return Ok(mealToReturn);
        }

        [HttpPut("{mealId}")]
        public async Task<IActionResult> UpdateMeal(int userId, int mealId, MealForUpdateDto mealForUpdateDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var mealFromRepo = await _mealRepo.GetMeal(mealId);
            mealForUpdateDto.Created = DateTime.Now;

            _mapper.Map(mealForUpdateDto, mealFromRepo);

            if (await _mealRepo.SaveAll())
                return Ok();

            return BadRequest($"Updating meal {mealId} failed on save");   
        }      
    }
}