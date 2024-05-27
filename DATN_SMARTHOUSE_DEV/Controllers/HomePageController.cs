using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.HomePage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/HomePage")]
    [ApiController]
    public class HomePageController : ControllerBase, IBaseController<HomePageRequest, HomePageResponse>
    {
        private readonly DBContext _context;
        private HomePageRequest _request;
        private readonly IMapper _mapper;
        private BaseResponse<HomePageResponse> _res;
        private HomePageResponse _response;
        private string _apiCode = "HomePage";
        public HomePageController(DBContext context, IMapper mapper)
        {
            _context = context;
            _res = new BaseResponse<HomePageResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new HomePageResponse();
            _mapper = mapper;
        }

        public void AccessDatabase()
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
            if (_request.Limit == null)
            {
                lstProduct = query.Skip(_request.OffSet.Value).Take(Utility.Utility.LimitDefault).ToList();
            }
            else
            {
                lstProduct = query.Skip(_request.OffSet.Value).Take(_request.Limit.Value).ToList();
            }
            _response.LstProduct = lstProduct;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            throw new NotImplementedException();
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<HomePageResponse> Process(HomePageRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
                //PreValidation(); // validate dữ liệu 
                //GenerateObjects(); // Gán dữ liệu 
                AccessDatabase(); // Lưu xuống DB 
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
