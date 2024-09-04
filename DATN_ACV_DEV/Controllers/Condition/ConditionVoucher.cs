using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.GHN_DTO;
using DATN_ACV_DEV.Model_DTO.Voucher_DTO;

namespace DATN_ACV_DEV.Controllers.Condition
{
    public class ConditionVoucher
    {
        public static void CreateVoucher_C01(DBContext context, string voucherCode, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            var voucher = context.TbVouchers.Where(a => a.Code == voucherCode && a.EndDate > DateTime.Now).FirstOrDefault();
            if (voucher != null)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_CODE_DUPLICATE, conCO3Field));
                throw aCV_Exception;
            }
        }
        public static void CreateVoucher_C02(DBContext context, string voucherCode, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            if (voucherCode.Length > 10)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_CODE_LENGHT, conCO3Field));
                throw aCV_Exception;
            }
        }
        public static void CreateVoucher_C03(DBContext context, DateTime EndDate, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            if (EndDate < DateTime.Now)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_END_DATE_EXITS, conCO3Field));
                throw aCV_Exception;
            }
        }
        public static void CreateVoucher_C04(DBContext context, string unit, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            if (unit != "%" && unit != "vnd")
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_UNIT_EXITS, conCO3Field));
                throw aCV_Exception;
            }
        }
        public static void CreateVoucher_C05(DBContext context, Guid VoucherId, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            var voucher = context.TbVouchers.Where(c => c.Id == VoucherId).FirstOrDefault();
            if (voucher == null)
            {
                aCV_Exception = new ACV_Exception();
                //To-do: Lay thong message text tu message code
                aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_NOTFOUND, conCO3Field));
                throw aCV_Exception;
            }
        }
        public static void CreateVoucher_C06(DBContext context, CreateVoucherRequest req, string apiCode, string con03, string conCO3Field)
        {
            ACV_Exception aCV_Exception;
            // var voucher = context.TbVouchers.Where(c => c.Id == VoucherId).FirstOrDefault();
            //if (req.ProductID == null && req.CategoryID == null)
            //{
            //    aCV_Exception = new ACV_Exception();
            //    //To-do: Lay thong message text tu message code
            //    aCV_Exception.Messages.Add(Message.CreateErrorMessage(apiCode, con03, Utility.Utility.VOUCHER_CAN_PRO_OR_CATE, conCO3Field));
            //    throw aCV_Exception;
            //}
        }
    }
}
