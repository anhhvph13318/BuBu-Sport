using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Cart_DTO;
using System.Reflection.Emit;

namespace DATN_ACV_DEV.Controllers.Condition
{
    public class ConditionCart
    {
        public static void AddToCart_C01(DBContext context, AddToCartRequest request, string apiCode, string con01, string conCO1Field)
        {
            ACV_Exception aCV_Exception;
            var Model = context.TbProducts.Where(c => c.Id == request.ProductId && c.IsDelete == true && c.Quantity < request.Quantity).FirstOrDefault();
            if (Model != null)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con01, Utility.Utility.PRODUCT_NOTFOUND, conCO1Field));
                throw aCV_Exception;
            };
        }
        public static void AddToCart_C02(DBContext context, AddToCartRequest request, string apiCode, string con02, string conCO2Field)
        {
            ACV_Exception aCV_Exception;
            var Model = context.TbProducts.Where(c => c.Id == request.ProductId && c.IsDelete == false).FirstOrDefault();

            if (Model.Quantity < request.Quantity)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con02, Utility.Utility.CART_ITEM_NOTFOUND, conCO2Field));
                throw aCV_Exception;
            };
        }
        public static void EditCartItem_C01(DBContext context, EditCartRequest request, string apiCode, string con02, string conCO2Field)
        {
            ACV_Exception aCV_Exception;
            var Model = context.TbCartDetails.Where(c => c.Id == request.CartDetaiID).FirstOrDefault();

            if (Model == null)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con02, Utility.Utility.CART_ITEM_NOTFOUND, conCO2Field));
                throw aCV_Exception;
            };
        }
        public static void EditCartItem_C02(DBContext context, EditCartRequest request, string apiCode, string con02, string conCO2Field)
        {
            ACV_Exception aCV_Exception;
            var Model = context.TbCartDetails.Where(c => c.Id == request.CartDetaiID).FirstOrDefault();
            if (Model != null)
            {
                var product = context.TbProducts.Where(p => p.Id == Model.ProductId).FirstOrDefault();
                if (product.Quantity < request.Quantity)
                {
                    aCV_Exception = new ACV_Exception();
                    //To-do: Lay thong message text tu message code
                    aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con02, Utility.Utility.PRODUCT_QUANTITY_IS_NOT_ENOUGH, conCO2Field));
                    throw aCV_Exception;
                };
            }

        }
        public static void DeleteCartItem_C01(DBContext context, Guid? cartDetailId, string apiCode, string con01, string conCO1Field)
        {
            ACV_Exception aCV_Exception;
            var Model = context.TbCartDetails.Where(c => c.Id == cartDetailId).FirstOrDefault();
            if (Model == null)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con01, Utility.Utility.PRODUCT_NOTFOUND, conCO1Field));
                throw aCV_Exception;
            };
        }
    }
}
