using DATN_ACV_DEV.FileBase;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DATN_ACV_DEV.Model_DTO.GHN_DTO
{
    public class GHNInfoOrderResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<Datum> data { get; set; }
    }
    public class Log
    {
        public string status { get; set; }
        public DateTime updated_date { get; set; }
    }
    public class Datum
    {
        public int shop_id { get; set; }
        public int client_id { get; set; }
        public string return_name { get; set; }
        public string return_phone { get; set; }
        public string return_address { get; set; }
        public string return_ward_code { get; set; }
        public int return_district_id { get; set; }
        public string from_name { get; set; }
        public string from_phone { get; set; }
        public string from_address { get; set; }
        public string from_ward_code { get; set; }
        public int from_district_id { get; set; }
        public int deliver_station_id { get; set; }
        public string to_name { get; set; }
        public string to_phone { get; set; }
        public string to_address { get; set; }
        public string to_ward_code { get; set; }
        public int to_district_id { get; set; }
        public int weight { get; set; }
        public int length { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int converted_weight { get; set; }
        public int service_type_id { get; set; }
        public int service_id { get; set; }
        public int payment_type_id { get; set; }
        public int custom_service_fee { get; set; }
        public int cod_amount { get; set; }
        public object cod_collect_date { get; set; }
        public object cod_transfer_date { get; set; }
        public bool is_cod_transferred { get; set; }
        public bool is_cod_collected { get; set; }
        public int insurance_value { get; set; }
        public int order_value { get; set; }
        public int pick_station_id { get; set; }
        public string client_order_code { get; set; }
        public int cod_failed_amount { get; set; }
        public string cod_failed_collect_date { get; set; }
        public string required_note { get; set; }
        public string content { get; set; }
        public string note { get; set; }
        public string employee_note { get; set; }
        public string coupon { get; set; }
        public string _id { get; set; }
        public string order_code { get; set; }
        public string version_no { get; set; }
        public string updated_ip { get; set; }
        public int updated_employee { get; set; }
        public int updated_client { get; set; }
        public string updated_source { get; set; }
        public DateTime updated_date { get; set; }
        public int updated_warehouse { get; set; }
        public string created_ip { get; set; }
        public int created_employee { get; set; }
        public int created_client { get; set; }
        public string created_source { get; set; }
        public DateTime created_date { get; set; }
        public string status { get; set; }
        public int pick_warehouse_id { get; set; }
        public int deliver_warehouse_id { get; set; }
        public int current_warehouse_id { get; set; }
        public int return_warehouse_id { get; set; }
        public int next_warehouse_id { get; set; }
        public DateTime leadtime { get; set; }
        public DateTime order_date { get; set; }
        public string soc_id { get; set; }
        public string finish_date { get; set; }
        public List<string> tag { get; set; }
        public List<Log> log { get; set; }
    }
}
