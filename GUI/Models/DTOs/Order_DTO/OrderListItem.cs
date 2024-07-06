namespace GUI.Models.DTOs.Order_DTO;

public class OrderListItem
{
    public Guid id { get; set; }
    public string? code { get; set; }
    public string? description { get; set; }
    public string status { get; set; }
    public string? nameCustomer { get; set; }
    public string paymentMethodName { get; set; }
    public decimal? amountShip { get; set; }
    public decimal? amountDiscount { get; set; }
    public decimal? totalAmount { get; set; }
    public string? products { get; set; }
    public string? ReasionCancel { get; set; }
    public List<string>? ImageForCancelOrder { get; set; }
}