namespace GUI.Models.DTOs.Address
{
    public class GetListAddressDeliveryResponse
    {
        public GetListAddressDeliveryResponse()
        {
            listAddress = new List<AddressDeliveryDTO>();
        }
        public List<AddressDeliveryDTO> listAddress { get; set; }
    }
    public class AddressDeliveryDTO
    {
        public Guid id { get; set; }
        public string address { get; set; }
        public string status { get; set; }
        public string? receiverName { get; set; }
        public string? receiverphone { get; set; }
    }
}
