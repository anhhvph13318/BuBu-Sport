using GUI.Models.DTOs.Order_DTO;
using Newtonsoft.Json;

namespace GUI
{
    public static class Extension
    {

        private static string ORDER_KEY = "temp_orders";
        private const string CURRENT_ORDER = "CURRENT_ORDER";

        public static IList<OrderDetail> GetTempOrders(this ISession session)
        {
            var json = session.GetString(ORDER_KEY);
            if (string.IsNullOrEmpty(json)) return Array.Empty<OrderDetail>();
            return JsonConvert.DeserializeObject<IList<OrderDetail>>(json ?? throw new NullReferenceException()) ?? throw new NullReferenceException();
        }

        public static void SaveTempOrder(this ISession session, OrderDetail order)
        {
            var orders = session.GetTempOrders();
            orders.Add(order);
            session.SetString(ORDER_KEY, JsonConvert.SerializeObject(orders));
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
