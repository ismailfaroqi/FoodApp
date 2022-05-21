using System.Security.Claims;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FoodService.Models;

namespace FoodService.GraphQL
{
    public class Query
    {
        [Authorize]
        public IQueryable<Food> GetFoods([Service] Latihan5Context context, ClaimsPrincipal claimsPrincipal)
        {
            var userName = claimsPrincipal.Identity.Name;

            // check buyer role ?
            var buyerRole = claimsPrincipal.Claims.Where(o => o.Type == ClaimTypes.Role && o.Value == "BUYER").FirstOrDefault();
            var food = context.Users.Where(o => o.Username == userName).FirstOrDefault();
            if (food != null)
            {
                if (buyerRole != null)
                    return context.Foods;

                var foods = context.Foods.Where(o => o.Id == food.Id);
                return foods.AsQueryable();
            }
            return new List<Food>().AsQueryable();
        }
    }
}
