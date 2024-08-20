using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using DATN_ACV_DEV.Model_DTO.Voucher_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/EditVoucher")]
    [ApiController]
    public class EditVoucherController : ControllerBase, IBaseController<EditVoucherRequest, EditVoucherResponse>
    {
        private readonly DBContext _context;
        private EditVoucherRequest _request;
        private BaseResponse<EditVoucherResponse> _res;
        private EditVoucherResponse _response;
        private string _apiCode = "EditVoucher";
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        private string _conC03 = "C03";
        private string _conC04 = "C04";
        private string _conC05 = "C05";
        private string _conC01Field = "Code";
        private string _conC02Field = "Code";
        private string _conC03Field = "EndDate";
        private string _conC04Field = "Unit";
        private string _conC05Field = "Id";
        private TbVoucher _Voucher;
        public EditVoucherController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<EditVoucherResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new EditVoucherResponse();
        }

        public void AccessDatabase()
        {
            _context.SaveChanges();
            _response.ID = _Voucher.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            try
            {
                _Voucher = _context.TbVouchers.Where(p => p.Id == _request.ID && p.EndDate >= DateTime.Now).FirstOrDefault();
                if (_Voucher != null)
                {
                    //_Voucher.Name = _request.Name ?? _Voucher.Name;
                    //_Voucher.Code = _request.Code ?? _Voucher.Code;
                    //_Voucher.Discount = _request.Discount ?? _Voucher.Discount;
                    //_Voucher.Description = _request.Description ?? _Voucher.Description;
                    //_Voucher.Quantity = _request.Quantity ?? _Voucher.Quantity;
                    //_Voucher.StartDate = _request.StartDate ?? _Voucher.StartDate;
                    //_Voucher.EndDate = _request.EndDate ?? _Voucher.EndDate;
                    //_Voucher.Type = _request.Type ?? _Voucher.Type;
                    //_Voucher.Unit = _request.Unit
                    //_Voucher.Status = _request.Status ?? _Voucher.Status;
                    //_Voucher.ProductId = _request.ProductID ?? _Voucher.ProductId;
                    //_Voucher.CategoryId = _request.CategoryID ?? _Voucher.CategoryId;
                    ////Default
                    //_Voucher.UpdateBy = _request.AdminId ?? Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993");
                    //_Voucher.UpdateDate = DateTime.Now;
                }
            }
            catch (Exception)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
            }
        }

        public void PreValidation()
        {
            // vouchert không tồn tại
            Condition.ConditionVoucher.CreateVoucher_C05(_context, _request.ID, _apiCode, _conC05, _conC05Field);
            if (_request.Code != null)
            {

                //check trùng code voucher
                Condition.ConditionVoucher.CreateVoucher_C01(_context, _request.Code, _apiCode, _conC01, _conC01Field);
                //Code không dài quá 10 ký tự
                Condition.ConditionVoucher.CreateVoucher_C02(_context, _request.Code, _apiCode, _conC02, _conC02Field);
            }
            //if (_request.EndDate != null)
            //{
            //    Condition.ConditionVoucher.CreateVoucher_C03(_context, _request.EndDate.Value, _apiCode, _conC03, _conC03Field);
            //}
            //// ngày kết thúc là ngày tương lai

            //// phần trăm giảm giá không được quá 80% 
            //// Unit chỉ có 2 option : % và vnd 
            //if (_request.Unit != null)
            //{
            //    Condition.ConditionVoucher.CreateVoucher_C04(_context, _request.Unit, _apiCode, _conC04, _conC04Field);
            //}
          
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<EditVoucherResponse> Process(EditVoucherRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                PreValidation();
                GenerateObjects();
                //PostValidation();
                AccessDatabase();
            }
            catch (ACV_Exception ex)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
                _res.Messages = ex.Messages;
            }
            catch (System.Exception ex)
            {
                _res.Status = StatusCodes.Status500InternalServerError.ToString();
                _res.Messages.Add(Message.CreateErrorMessage(_apiCode, _res.Status, ex.Message, string.Empty));
            }
            return _res;

        }
    }
}
