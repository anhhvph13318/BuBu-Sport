using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Cart_DTO;
using DATN_ACV_DEV.Model_DTO.GHN_DTO;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using DATN_ACV_DEV.Utility;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Emit;

namespace DATN_ACV_DEV.Controllers.Condition
{
    public class ConditionOrder
    {
        public static void CreateOrder_C03(DBContext context, List<Guid>? request, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            if (request.Count() > 2)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.QUANTITY_VOUCHER_EXCEES_REGULATIONS, conCO3Field));
                throw aCV_Exception;
            }
        }
        public static void CreateOrder_C04(DBContext context, List<Guid>? request, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            var voucher = context.TbVouchers.Where(v => request.Contains(v.Id)).Select(t => t.Type).Distinct();

            if (voucher.Count() == 1 && request.Count() > 1)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_OF_THE_TYPE_SAME, conCO3Field));
                throw aCV_Exception;
            }
        }
        public static void CreateOrder_C02(DBContext context, List<Guid>? request, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            if (request != null)
            {
                foreach (var item in request)
                {
                    var Model = context.TbVouchers.Where(v => v.Id == item && v.EndDate < DateTime.Now).FirstOrDefault();
                    if (Model != null)
                    {
                        aCV_Exception = new ACV_Exception();
                        //To-do: Lay thong message text tu message code
                        aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_NOTFOUND + Model.Code, conCO3Field));
                        throw aCV_Exception;
                    }
                }
            }
        }
        public static void CreateOrder_C05(DBContext context, CreateOrderRequest request, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            var addressCustomer = context.TbAddressDeliveries.Where(a => a.Id == request.AddressDeliveryId && a.IsDelete == false && a.AccountId == request.UserId).FirstOrDefault();
            if (addressCustomer == null)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.ADDRESS_DELIVERY_NOTFOUND, conCO3Field));
                throw aCV_Exception;
            }
        }
        public static void CreateOrder_C06(DBContext context, CreateOrderRequest request, string apiCode, string con03, string conCO3Field)
        {
            //ACV_Exception aCV_Exception;
            //var productId = context.TbCartDetails.Where(c => request.cartDetailID.Contains(c.Id)).Select(a => a.ProductId);
            //var product = context.TbProducts.Where(c => productId.Contains(c.Id));
            //var voucher = context.TbVouchers.Where(v => request.voucherID.Contains(v.Id) && v.Type == Utility.Utility.VOUCHER_DISCOUNT).FirstOrDefault();
            //if (voucher != null)
            //{
            //    var checkVoucher = voucher.ProductId == null ? product.Select(s => s.CategoryId).Contains(voucher.CategoryId.Value) : product.Select(s => s.Id).Contains(voucher.ProductId.Value);
            //    if (!checkVoucher)
            //    {
            //        aCV_Exception = new ACV_Exception();
            //        //To-do: Lay thong message text tu message code
            //        aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_DONT_APPLY_PRODUCT + voucher.Code, conCO3Field));
            //        throw aCV_Exception;
            //    }

            //}
        }
        public static void CreateOrderCouter_C06(DBContext context, CreateOrderCounterRequest request, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            var productId = context.TbCartDetails.Where(c => request.cartDetailID.Contains(c.Id)).Select(a => a.ProductId);
            var product = context.TbProducts.Where(c => productId.Contains(c.Id));
            //var voucher = context.TbVouchers.Where(v => request.voucherCode.Contains(v.Code) && v.Type == Utility.Utility.VOUCHER_DISCOUNT).FirstOrDefault();
            //if (voucher != null)
            //{
            //    var checkVoucher = voucher.ProductId == null ? product.Select(s => s.CategoryId).Contains(voucher.CategoryId.Value) : product.Select(s => s.Id).Contains(voucher.ProductId.Value);
            //    if (!checkVoucher)
            //    {
            //        aCV_Exception = new ACV_Exception();
            //        //To-do: Lay thong message text tu message code
            //        aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_DONT_APPLY_PRODUCT + voucher.Code, conCO3Field));
            //        throw aCV_Exception;
            //    }

            //}
        }






        public static void UpdateStatusOrder_C01(DBContext context, UpdatStatusOrderRequest request, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            var order = context.TbOrders.Where(a => a.Id == request.id).FirstOrDefault();
            if (order == null)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.ORDER_NOTFOUND, conCO3Field));
                throw aCV_Exception;
            }
        }
        public static void UpdateStatusOrder_C02(DBContext context, UpdatStatusOrderRequest request, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            var order = context.TbOrders.Where(a => a.Id == request.id).FirstOrDefault();
            if (order.Status == Utility.Utility.ORDER_STATUS_RETURNS_PRODUCT || order.Status == Utility.Utility.ORDER_STATUS_PARTIAL_REFUND || order.Status == Utility.Utility.ORDER_STATUS_DONE)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.ORDER_STATUS_EXIST, conCO3Field));
                throw aCV_Exception;
            }
        }

        public static void CreateOrderGHN_C01(DBContext context, GHNCreateOrderRequest request, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            var order = context.TbOrders.Where(a => a.Id == request.orderId).FirstOrDefault();
            if (order == null)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.ORDER_NOTFOUND, conCO3Field));
                throw aCV_Exception;
            }
        }
        public static void CreateOrderGHN_C02(DBContext context, GHNCreateOrderRequest request, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            var order = context.TbOrders.Where(a => a.Id == request.orderId).FirstOrDefault();
            if (order.Status != Utility.Utility.ORDER_STATUS_PREPARE_GOODS)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.CREATE_ORDER_GHN_STATUS_EXIST, conCO3Field));
                throw aCV_Exception;
            }
        }

        public static void CreateOrderCouter_C03(DBContext context, string? request, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            var checkVoucherType = context.TbVouchers.Where(c => c.Code == request && c.EndDate > DateTime.Now && c.Quantity > 0).FirstOrDefault();
            if (checkVoucherType != null)
            {
                //if (checkVoucherType.Type != Utility.Utility.VOUCHER_DISCOUNT)
                //{
                //    aCV_Exception = new ACV_Exception();
                //    //To-do: Lay thong message text tu message code
                //    aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_DO_NOT_APPLY_ORDER, conCO3Field));
                //    throw aCV_Exception;
                //}
            }
            else
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_NOTFOUND, conCO3Field));
                throw aCV_Exception;
            }

        }
        //public static void CreateOrderCouter_C04(DBContext context, List<string>? request, string apiCode, string con03, string conCO3Field)
        //{
        //    ACV_Exception aCV_Exception;
        //    var voucher = context.TbVouchers.Where(v => request.Contains(v.Code)).Select(t => t.Type).Distinct();

        //    if (voucher.Count() == 1 && request.Count() > 1)
        //    {
        //        aCV_Exception = new ACV_Exception();
        //        //To-do: Lay thong message text tu message code
        //        aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_OF_THE_TYPE_SAME, conCO3Field));
        //        throw aCV_Exception;
        //    }
        //}

    }
}
