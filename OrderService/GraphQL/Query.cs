using System.Security.Claims;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.GraphQL
{
    public class Query
    {
        [Authorize(Roles = new[] { "MANAGER" })]
        public IQueryable<Order> GetOrder([Service] Latihan5Context context) =>
            context.Orders.Include(o => o.OrderDetails);

        [Authorize]
        public IQueryable<Order> GetOrdersByToken([Service] Latihan5Context context, ClaimsPrincipal claimsPrincipal)
        {
            var username = claimsPrincipal.Identity.Name;
            var user = context.Users.Where(o => o.Username == username).FirstOrDefault();
            if (user != null)
            {
                var orders = context.Orders.Where(o => o.UserId == user.Id).Include(o => o.OrderDetails);
                return orders.AsQueryable();
            }
            return new List<Order>().AsQueryable();
        }
    }
}
