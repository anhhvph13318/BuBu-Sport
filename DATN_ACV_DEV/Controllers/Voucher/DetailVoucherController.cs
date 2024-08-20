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
    [Route("api/DetailVoucher")]
    [ApiController]
    public class DetailVoucherController : ControllerBase, IBaseController<DetailVoucherRequest, DetailVoucherResponse>
    {
        private readonly DBContext _context;
        private DetailVoucherRequest _request;
        private BaseResponse<DetailVoucherResponse> _res;
        private DetailVoucherResponse _response;
        private string _apiCode = "DetailVoucher";
        private string _conC01 = "C01";
        private string _conC01Field = "Id";
        private TbVoucher _Voucher;
        public DetailVoucherController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<DetailVoucherResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new DetailVoucherResponse();
        }

        public void AccessDatabase()
        {

            //try
            //{
            //    _Voucher = _context.TbVouchers.Where(p => p.Id == _request.ID && p.EndDate >= DateTime.Now).FirstOrDefault();
            //    var product = _context.TbProducts.Where(p => p.Id == _Voucher.ProductId && p.IsDelete == false).FirstOrDefault();
            //    var category = _context.TbCategories.Where(p => p.Id == _Voucher.CategoryId && p.IsDelete == false).FirstOrDefault();
            //    var groupCustomerName = _context.TbGroupCustomers.Where(p => p.Id == _Voucher.CategoryId && p.IsDelete == false).FirstOrDefault();
            //    if (_Voucher != null)
            //    {
            //        _response.voucherDetail.Id = _Voucher.Id;
            //        _response.voucherDetail.Name = _Voucher.Name;
            //        _response.voucherDetail.Code = _Voucher.Code;
            //        _response.voucherDetail.Discount = _Voucher.Discount;
            //        _response.voucherDetail.Description = _Voucher.Description;
            //        _response.voucherDetail.Quantity = _Voucher.Quantity;
            //        _response.voucherDetail.StartDate = _Voucher.StartDate;
            //        _response.voucherDetail.EndDate = _Voucher.EndDate;
            //        _response.voucherDetail.Type = _Voucher.Type;
            //        _response.voucherDetail.Unit = _Voucher.Unit;
            //        _response.voucherDetail.ProductName = product != null ? product.Name : "";
            //        _response.voucherDetail.CategoryName = category != null ? category.Name : "";
            //        _response.voucherDetail.GroupCustomerName = groupCustomerName != null ? groupCustomerName.Name : "";
            //    }
            //}
            //catch (Exception)
            //{
            //    _res.Status = StatusCodes.Status400BadRequest.ToString();
            //}
            //_res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            throw new NotImplementedException();
        }

        public void PreValidation()
        {
            // vouchert không tồn tại
            Condition.ConditionVoucher.CreateVoucher_C05(_context, _request.ID, _apiCode, _conC01, _conC01Field);
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<DetailVoucherResponse> Process(DetailVoucherRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                PreValidation();
                //GenerateObjects();
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
