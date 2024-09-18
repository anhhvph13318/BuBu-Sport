using GUI.Models.DTOs.Order_DTO;
using System.ComponentModel.DataAnnotations;

namespace GUI.Models.DTOs.Customer_DTO.Views
{
	public class CustomerInfoModel
	{
        public PersonalInfo Info { get; set; }
        public List<OrderDetail> Orders { get; set; }
        public string Password { get; set; }
    }

	public class PersonalInfo
	{
		public string Phone { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Email { get; set; }
		public int? Sex { get; set; }
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
		[DataType(DataType.Date)]
		public DateTime? Dob { get; set; }
    }

	public class OrderInfo
	{
        public Guid Id { get; set; }
        public string Code { get; set; }
        public decimal Total { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
