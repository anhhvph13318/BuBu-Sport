using DATN_ACV_DEV.Entity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DATN_ACV_DEV
{
    public class VoucherConversation : ValueConverter<VoucherUnit, string>
    {
        public VoucherConversation()
            : base(
                 v => v.ToString(),
                 d => (VoucherUnit)Enum.Parse(typeof(VoucherUnit), d)
            )
        {
        }
    }
}
