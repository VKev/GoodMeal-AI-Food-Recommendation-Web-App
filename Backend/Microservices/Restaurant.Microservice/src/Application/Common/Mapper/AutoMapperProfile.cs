using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Foods.Commands;
using Application.Foods.Queries;
using Application.RestaurantRatings.Commands;
using Application.RestaurantRatings.Queries;
using Application.Restaurants.Commands;
using Application.Restaurants.Queries;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Restaurant
            CreateMap<CreateRestaurantCommand, Restaurant>();
            CreateMap<Restaurant, CreateRestaurantCommand>();
            CreateMap<Restaurant, GetRestaurantResponse>();
            CreateMap<UpdateRestaurantCommand, Restaurant>();
            //Food
            CreateMap<CreateFoodCommand, Food>();
            CreateMap<Food, CreateFoodCommand>();
            CreateMap<Food, GetFoodResponse>();
            CreateMap<UpdateFoodCommand, Food>();
            //RestaurantRating
            CreateMap<CreateRatingCommand, RestaurantRating>();
            CreateMap<RestaurantRating, CreateRatingCommand>();
            CreateMap<RestaurantRating, GetRatingResponse>();
            CreateMap<UpdateRatingCommand, RestaurantRating>();
            
        }
        
    }
}