using System.Security.Claims;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.GraphQL
{
    public class Mutation
    {
        [Authorize]
        public async Task<OrderData> BuyFoodAsync(
        OrderData input,
        ClaimsPrincipal claimsPrincipal,
        [Service] Latihan5Context context)
        {
            using var transaction = context.Database.BeginTransaction();
            var userName = claimsPrincipal.Identity.Name;
            try
            {
                var user = context.Users.Where(o => o.Username == userName).FirstOrDefault();
                if (user != null)
                {
                    // EF
                    var order = new Order
                    {
                        Code = Guid.NewGuid().ToString(), // generate random chars using GUID
                        UserId = user.Id,
                        CourierId = input.CourierId
                    };

                    foreach (var item in input.Details)
                    {
                        var detail = new OrderDetail
                        {
                            OrderId = order.Id,
                            FoodId = item.FoodId,
                            Quantity = item.Quantity
                        };
                        order.OrderDetails.Add(detail);
                    }
                    context.Orders.Add(order);
                    context.SaveChanges();
                    await transaction.CommitAsync();


                    /*input.Id = order.Id;
                    input.Code = order.Code;*/
                }
                else
                    throw new Exception("user was not found");
            }
            catch (Exception err)
            {
                transaction.Rollback();
            }
            return input;
        }

        //Update 
        [Authorize(Roles = new[] { "MANAGER" })]
        public async Task<Order> UpdateOrderAsync(
            UpdateCourier input,
            [Service] Latihan5Context context)
        {
            var order = context.Orders.Where(o => o.Id == input.Id).Include(o => o.OrderDetails).FirstOrDefault();
            if (order != null)
            {
                order.CourierId = input.CourierId;

                context.Orders.Update(order);
                await context.SaveChangesAsync();
            }
            return await Task.FromResult(order);
        }

        //Delete
        [Authorize(Roles = new[] { "MANAGER" })]
        public async Task<Order> DeleteOrderByIdAsync(
            int id,
            [Service] Latihan5Context context)
        {
            var order = context.Orders.Where(o => o.Id == id).Include(o => o.OrderDetails).FirstOrDefault();
            if (order != null)
            {
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
            }
            return await Task.FromResult(order);
        }
    }
}
