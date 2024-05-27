using AutoMapper;
using Azure.Core;
using DATN_ACV_DEV.Controllers.Property;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using DATN_ACV_DEV.Model_DTO.Property_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/GetListProduct")]
    [ApiController]
    public class GetListProductController : ControllerBase, IBaseController<GetListProductRequest, GetListProductResponse>
    {
        private readonly DBContext _context;
        private GetListProductRequest _request;
        private BaseResponse<GetListProductResponse> _res;
        private GetListProductResponse _response;
        private string _apiCode = "GetListProduct";
        private readonly IMapper _mapper;
        private TbProduct _Produst;
        public GetListProductController(DBContext context, IMapper mapper)
        {
            _context = context;
            _res = new BaseResponse<GetListProductResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new GetListProductResponse();
            _mapper = mapper;
        }

        public void AccessDatabase()
        {
            
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);
        }

        public void GenerateObjects()
        {
            List<HomePageModel> lstProduct = new List<HomePageModel>();
            var Model = _context.TbProducts.Where(p => p.IsDelete == false
                        && (!string.IsNullOrEmpty(_request.Name) ? p.Name.Contains(_request.Name) : true)
                        && (_request.CategoryID.HasValue ? p.CategoryId >= _request.CategoryID : true)
                        && (_request.PriceFrom.HasValue ? p.Price >= _request.PriceFrom : true)
                        && (_request.PriceTo.HasValue ? p.Price <= _request.PriceTo : true)
                        && (_request.FromDate.HasValue ? p.CreateDate >= _request.FromDate : true)
                        && (_request.ToDate.HasValue ? p.CreateDate <= _request.ToDate : true)
                        && (_request.Status.HasValue ? p.Status == _request.Status : true)).OrderByDescending(d => d.CreateDate);
            var category = _context.TbCategories.Where(c => c.Id == _request.CategoryID && c.IsDelete == false).Distinct();
            var image = _context.TbImages.Where(c => Model.Select(x => x.ImageId).Contains(c.Id)).Distinct();
            Model.ToList().ForEach(m =>
            {
                m.Category = category.Where(c => c.Id == m.CategoryId).FirstOrDefault();
                m.tb_Image = image.Where(c => c.Id == m.ImageId).FirstOrDefault();
            });
            _response.TotalCount = Model.Count();
            var query = _mapper.Map<List<HomePageModel>>(Model);
            _response.LstProduct = query;
            _res.Data = _response;
        }

        public void PreValidation()
        {
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<GetListProductResponse> Process(GetListProductRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                //PreValidation();
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
