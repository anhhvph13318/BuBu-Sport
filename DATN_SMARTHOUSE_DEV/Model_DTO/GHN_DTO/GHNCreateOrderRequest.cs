using DATN_ACV_DEV.FileBase;

namespace DATN_ACV_DEV.Model_DTO.GHN_DTO
{
    public class GHNCreateOrderRequest : BaseRequest
    {
        public Guid orderId { get; set; }
        public int? serviceId { get; set; }
    }
    public class RequestCreateOrderGHN
    {
        public int payment_type_id { get; set; } = 1;
        public string? note { get; set; }
        public string required_note { get; set; } = "CHOXEMHANGKHONGTHU";
        public string from_name { get; set; } = Utility.Utility.FROM_NAME;
        public string from_phone { get; set; } = Utility.Utility.FROM_PHONE;
        public string from_address { get; set; } = Utility.Utility.FROM_ADDRESS;
        public string from_ward_name { get; set; } = Utility.Utility.FROM_WARD_NAME;
        public string from_district_name { get; set; } = Utility.Utility.FROM_DISTRIC_NAME;
        public string from_province_name { get; set; } = Utility.Utility.FROM_PROVICE_NAME;
        public string? return_phone { get; set; } = Utility.Utility.RETURN_PHONE;
        public string? return_address { get; set; } = Utility.Utility.RETURN_ADDRESS;
        public string? return_district_id { get; set; }
        public string? return_ward_code { get; set; } 
        public string? client_order_code { get; set; }
        public string to_name { get; set; }
        public string to_phone { get; set; }
        public string to_address { get; set; }
        public string to_ward_code { get; set; }
        public int to_district_id { get; set; }
        public int? cod_amount { get; set; }
        public string? content { get; set; }
        public int weight { get; set; }
        public int length { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int? pick_station_id { get; set; }
        public int insurance_value { get; set; }
        public int service_id { get; set; }
        public int service_type_id { get; set; }
        public List<item> items { get; set; }
    }
    public class item
    {
        public string name { get; set; }
        public string? code { get; set; }
        public int quantity { get; set; }
        public int? price { get; set; }
        public int? length { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public int weight { get; set; }
    }
}
