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
    [Route("api/GetListVoucher")]
    [ApiController]
    public class GetListVoucherController : ControllerBase, IBaseController<GetListVoucherRequest, GetListVoucherResponse>
    {
        private readonly DBContext _context;
        private GetListVoucherRequest _request;
        private readonly IMapper _mapper;
        private BaseResponse<GetListVoucherResponse> _res;
        private GetListVoucherResponse _response;
        private string _apiCode = "GetListVoucher";
        private TbVoucher _Voucher;
        public GetListVoucherController(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _res = new BaseResponse<GetListVoucherResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new GetListVoucherResponse();
        }

        public void AccessDatabase()
        {
            //List<VoucherDTO> lstVoucher = new List<VoucherDTO>();
            //var Model = _context.TbVouchers.Where(x => x.EndDate >= DateTime.Now
            //           && (!string.IsNullOrEmpty(_request.Name) ? x.Name.Contains(_request.Name) : true)
            //           && (!string.IsNullOrEmpty(_request.Code) ? x.Code.Contains(_request.Code) : true)
            //           && (!string.IsNullOrEmpty(_request.Type) ? x.Type.Contains(_request.Type) : true)
            //           && (!string.IsNullOrEmpty(_request.Unit) ? x.Unit.Contains(_request.Unit) : true)
            //           && (_request.FromDate.HasValue ? x.CreateDate >= _request.FromDate : true)
            //           && (_request.ToDate.HasValue ? x.CreateDate <= _request.ToDate : true)
            //           && (_request.CategoryId.HasValue ? x.CategoryId == _request.CategoryId : true)
            //           && (_request.ProductId.HasValue ? x.ProductId == _request.ProductId : true)
            //           && (_request.GroupCustomerId.HasValue ? x.GroupCustomerId == _request.GroupCustomerId : true)
            //           ).OrderByDescending(d => d.CreateDate);
            //var product = _context.TbProducts.Where(p => Model.Select(c => c.ProductId).Contains(p.Id) && p.IsDelete == false).Distinct();
            //var Category = _context.TbCategories.Where(p => Model.Select(c => c.CategoryId).Contains(p.Id) && p.IsDelete == false).Distinct();
            ////var GroupCustomerName = _context.TbGroupCustomers.Where(p => Model.Select(c => c.GroupCustomerId).Contains(p.Id) && p.IsDelete == false).Distinct();
            //Model.ToList().ForEach(v =>
            //{
            //    v.product = product.Where(c => c.Id == v.ProductId).FirstOrDefault();
            //    v.category = Category.Where(c => c.Id == v.CategoryId).FirstOrDefault();
            //    //v.groupCustomer = GroupCustomerName.Where(c => c.Id == v.GroupCustomerId).FirstOrDefault();
            //});
            //_response.TotalCount = Model.Count();
            //var query = _mapper.Map<List<VoucherDTO>>(Model);
            //if (_request.Limit == null)
            //{
            //    lstVoucher = query.Skip(_request.OffSet.Value).Take(Utility.Utility.LimitDefault).ToList();
            //}
            //else
            //{
            //    lstVoucher = query.Skip(_request.OffSet.Value).Take(_request.Limit.Value).ToList();
            //}
            //_response.LstVoucher = lstVoucher;
            //_res.Data = _response;
        }
        public void CheckAuthorization()
        {
            if (_request.LoginType)
            {
                _request.Authorization(_context, _apiCode);
            }
            else
            {
                _request.AuthorizationCustomer(_context, _apiCode);
            }
        }

        public void GenerateObjects()
        {
        }

        public void PreValidation()
        {
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<GetListVoucherResponse> Process(GetListVoucherRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                //PreValidation();
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
