namespace GUI.Models.DTOs.Voucher_DTO
{
  public class VoucherList
  {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Discount { get; set; }
    public string Status { get; set; } = string.Empty;
  }

  public class GetListVoucherRequest
  {
    public string? Name { get; set; }
    public string? Code { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
  }
}