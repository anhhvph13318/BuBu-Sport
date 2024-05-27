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
    [Route("api/CreateVoucher")]
    [ApiController]
    public class CreateVoucherController : ControllerBase, IBaseController<CreateVoucherRequest, CreateVoucherResponse>
    {
        private readonly DBContext _context;
        private CreateVoucherRequest _request;
        private BaseResponse<CreateVoucherResponse> _res;
        private CreateVoucherResponse _response;
        private string _apiCode = "CreateVoucher";
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        private string _conC03 = "C03";
        private string _conC04 = "C04";
        private string _conC05 = "C04";
        private string _conC01Field = "Code";
        private string _conC02Field = "Code";
        private string _conC03Field = "EndDate";
        private string _conC04Field = "Unit";
        private string _conC05Field = "ProductId, CategoryId";
        private TbVoucher _Voucher;
        public CreateVoucherController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateVoucherResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateVoucherResponse();
        }

        public void AccessDatabase()
        {
            _context.Add(_Voucher);
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
            _Voucher = new TbVoucher()
            {
                Id = Guid.NewGuid(),
                Name = _request.Name,
                Code = _request.Code,
                Discount = _request.Discount,
                Description = _request.Description,
                Quantity  = _request.Quantity,
                StartDate = _request.StartDate,
                EndDate = _request.EndDate,
                Type = _request.Type,
                Unit = _request.Unit,
                Status = _request.Status,
                ProductId = _request.ProductID,
                CategoryId = _request.CategoryID,
                //Default
                CreateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993"), // Tạm thời gán guid khởi tạo.
                CreateDate = DateTime.Now, // Ngày hiện tại 
            };
        }

        public void PreValidation()
        {
            //check trùng code voucher
            Condition.ConditionVoucher.CreateVoucher_C01(_context, _request.Code, _apiCode, _conC01, _conC01Field);
            //Code không dài quá 10 ký tự
            Condition.ConditionVoucher.CreateVoucher_C02(_context, _request.Code, _apiCode, _conC02, _conC02Field);
            // ngày kết thúc là ngày tương lai
            Condition.ConditionVoucher.CreateVoucher_C03(_context, _request.EndDate, _apiCode, _conC03, _conC03Field);
            // phần trăm giảm giá không được quá 80% 
            // Unit chỉ có 2 option : % và vnd 
            Condition.ConditionVoucher.CreateVoucher_C04(_context, _request.Unit, _apiCode, _conC04, _conC04Field);
            // Voucher chỉ áp dụng "1trong2" sản phẩm hoặc danh mục sản phẩm ( có thể áp dụng thêm cho cả nhóm khách hàng )
            Condition.ConditionVoucher.CreateVoucher_C06(_context, _request, _apiCode, _conC05, _conC05Field);
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateVoucherResponse> Process(CreateVoucherRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
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
