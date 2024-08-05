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

        public static List<OrderDetail> SaveTempOrder(this ISession session, OrderDetail order, out bool alreadySave)
        {
            var orders = session.GetTempOrders();
            var existOrder = orders.FirstOrDefault(e => e.Id == order.Id);
            alreadySave = false;
            if (existOrder is not null)
            {
                alreadySave = true;
                orders.Remove(existOrder);
            }
            order.Id = Guid.NewGuid();
            orders.Add(order);

            session.SetString(ORDER_KEY, JsonConvert.SerializeObject(orders));

            return orders.OrderByDescending(e => e.TempOrderCreatedTime).ToList();
        }

        public static List<OrderDetail> RemoveTempOrder(this ISession session, string id, out bool alreadyRemove)
        {
            alreadyRemove = false;
            var orders = session.GetTempOrders();
            var order = orders.FirstOrDefault(e => e.Id == Guid.Parse(id));
            if (order is not null)
            {
                orders.Remove(order);
                session.SetString(ORDER_KEY, JsonConvert.SerializeObject(orders));
                alreadyRemove = true;
            }

            return orders;
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
                    Id = Guid.NewGuid(),
                    Code = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    ShippingInfo = new ShippingInfo(),
                    PaymentInfo = new PaymentInfo(),
                    Customer = new CustomerInfo(),
                    Items = new List<OrderItem>()
                };

                session.SaveCurrentOrder(order);
                return order;
            }

            return JsonConvert.DeserializeObject<OrderDetail>(json) ?? throw new NullReferenceException();
        }

        public static void SaveCurrentOrder(this ISession session, OrderDetail order)
        {
            session.SetString(CURRENT_ORDER, JsonConvert.SerializeObject(order));
        }
    }
}
