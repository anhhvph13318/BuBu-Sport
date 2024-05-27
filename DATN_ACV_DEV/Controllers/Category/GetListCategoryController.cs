using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/GetListCategory")]
    [ApiController]
    public class GetListCategoryController : ControllerBase, IBaseController<GetListCategoryRequest, GetListCategoryResponse>
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private GetListCategoryRequest _request;
        private BaseResponse<GetListCategoryResponse> _res;
        private GetListCategoryResponse _response;
        private string _apiCode = "GetListCategory";
        public GetListCategoryController(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _res = new BaseResponse<GetListCategoryResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new GetListCategoryResponse();
        }

        public void AccessDatabase()
        {
            List<CategoryDTO> LstCategory = new List<CategoryDTO>();
            var Model = _context.TbCategories.Where(c => c.IsDelete == false
                        && (!string.IsNullOrEmpty(_request.Name) ? c.Name.Contains(_request.Name) : true)
                        && (_request.Status.HasValue ? c.Status == _request.Status : true)
                        && (_request.FromDate.HasValue ? c.CreateDate >= _request.FromDate : true)
                        && (_request.ToDate.HasValue ? c.CreateDate <= _request.ToDate : true)
                        ).OrderByDescending(d => d.CreateDate);
            var Image = _context.TbImages.Where(c => Model.Select(a => a.ImageId).Contains(c.Id)).Distinct();
            _response.TotalCount = Model.Count();
            Model.ToList().ForEach(c =>
            {
                c.Image = Image.Where(i => i.Id == c.ImageId).FirstOrDefault();
            });
            var query = _mapper.Map<List<CategoryDTO>>(Model);
            if (_request.Limit == null)
            {
                LstCategory = query.Skip(_request.OffSet.Value).Take(Utility.Utility.LimitDefault).ToList();
            }
            else
            {
                LstCategory = query.Skip(_request.OffSet.Value).Take(_request.Limit.Value).ToList();
            }
            _response.LstCategory = LstCategory;
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
        public BaseResponse<GetListCategoryResponse> Process(GetListCategoryRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
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
