using System.Security.Claims;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.GraphQL
{
    public class Query
    {
        [Authorize(Roles = new[] { "ADMIN" })] // dapat diakses kalau sudah login
        public IQueryable<UserData> GetUsers([Service] Latihan5Context context) =>
            context.Users.Select(p => new UserData()
            {
                Id = p.Id,
                FullName = p.FullName,
                Email = p.Email,
                Username = p.Username
            });

        [Authorize]
        public IQueryable<Profile> GetProfiles([Service] Latihan5Context context, ClaimsPrincipal claimsPrincipal)
        {
            var userName = claimsPrincipal.Identity.Name;

            // check admin role ?
            var adminRole = claimsPrincipal.Claims.Where(o => o.Type == ClaimTypes.Role && o.Value == "ADMIN").FirstOrDefault();
            var user = context.Users.Where(o => o.Username == userName).FirstOrDefault();
            if (user != null)
            {
                if (adminRole != null)
                {
                    return context.Profiles;
                }
                var profiles = context.Profiles.Where(o => o.UserId == user.Id);
                return profiles.AsQueryable();
            }


            return new List<Profile>().AsQueryable();
        }

        [Authorize(Roles = new[] { "MANAGER" })] // dapat diakses kalau sudah login
        public IQueryable<CourierData> GetCouriers([Service] Latihan5Context context) =>
            context.Couriers.Select(c => new CourierData()
            {
                Id = c.Id,
                CourierName = c.CourierName,
                PhoneNumber = c.PhoneNumber
            });
    }
}
