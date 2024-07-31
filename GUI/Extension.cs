using GUI.Models.DTOs.Order_DTO;
using Newtonsoft.Json;

namespace GUI
{
    public static class Extension
    {

        private const string ORDER_KEY = "TEMP_ORDERS";
        private const string CURRENT_ORDER = "CURRENT_ORDER";

        public static List<OrderDetail> GetTempOrders(this ISession session)
        {
            var json = session.GetString(ORDER_KEY);
            if (string.IsNullOrEmpty(json)) return new();
            return JsonConvert.DeserializeObject<List<OrderDetail>>(json) ?? throw new NullReferenceException();
        }

        public static List<OrderDetail> SaveTempOrder(this ISession session, OrderDetail order)
        {
            var orders = session.GetTempOrders();
            orders.Add(order);
            session.SetString(ORDER_KEY, JsonConvert.SerializeObject(orders));
            
            return orders.OrderByDescending(e => e.TempOrderCreatedTime).ToList();
        }

        public static OrderDetail? GetOrderFromList(this ISession session, string id)
        {
            var orders = session.GetTempOrders();
            return orders.FirstOrDefault(e => e.Id == Guid.Parse(id));
        }

        public static OrderDetail GetCurrentOrder(this ISession session, bool clearFirst = false)
        {
            var json = session.GetString(CURRENT_ORDER);
            if (string.IsNullOrEmpty(json) || clearFirst)
            {
                var order = new OrderDetail()
                {
                    ShippingInfo = new ShippingInfo(),
                    PaymentInfo = new PaymentInfo(),
                    Customer = new CustomerInfo(),
                    Items = new List<OrderItem>()
                };

                session.SaveCurrentOrder(order);
                return order;
            }

            return JsonConvert.DeserializeObject<OrderDetail>(json ?? throw new NullReferenceException()) ?? throw new NullReferenceException();
        }

        public static void SaveCurrentOrder(this ISession session, OrderDetail order)
        {
            session.SetString(CURRENT_ORDER, JsonConvert.SerializeObject(order));
        }
    }
}
