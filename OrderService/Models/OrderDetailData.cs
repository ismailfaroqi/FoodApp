namespace OrderService.Models
{
    public record OrderDetailData
    (
        int? Id,
        int? OrderId,
        int FoodId,
        int Quantity
    );
}
