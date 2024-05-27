using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.AddressDelivery;
using DATN_ACV_DEV.Model_DTO.Cart_DTO;
using System.Reflection.Emit;

namespace DATN_ACV_DEV.Controllers.Condition
{
    public class ConditionAddressDelivery
    {
        public static void AddAddress_C01(DBContext context, CreateAddessDeliveryRequest request, string apiCode, string con01, string conCO1Field)
        {
            ACV_Exception aCV_Exception;          
            if (true)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con01, Utility.Utility.PRODUCT_NOTFOUND, conCO1Field));
                throw aCV_Exception;
            };
        }       
    }
}
