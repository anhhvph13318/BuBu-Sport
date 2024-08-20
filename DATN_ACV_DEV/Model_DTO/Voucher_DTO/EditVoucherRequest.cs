using DATN_ACV_DEV.FileBase;
using System.Diagnostics.SymbolStore;

namespace DATN_ACV_DEV.Model_DTO.Voucher_DTO
{
    public class EditVoucherRequest: CreateVoucherRequest
    {
        public Guid ID { get; set; }
    }
}
