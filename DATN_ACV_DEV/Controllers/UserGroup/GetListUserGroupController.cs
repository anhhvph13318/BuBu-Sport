using AutoMapper;
using DATN_ACV_DEV.Controllers.Condition;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Cart_DTO;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using DATN_ACV_DEV.Model_DTO.UserGroup_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/GetListUserGroup")]
    [ApiController]
    public class GetListUserGroupController : ControllerBase, IBaseController<GetListUserGroupRequest, GetListUserGroupResponse>
    {
        private readonly DBContext _context;
        private GetListUserGroupRequest _request;
        private BaseResponse<GetListUserGroupResponse> _res;
        private GetListUserGroupResponse _response;
        private string _apiCode = "GetListUserGroup";
        private TbUserGroup _userGroup;
        private bool checkCart = false;
        private bool checkProduct = false;
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        public GetListUserGroupController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<GetListUserGroupResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new GetListUserGroupResponse();
        }

        public void AccessDatabase()
        {
            var model = _context.TbUserGroups.Where(c => c.IsDelete == false
                                                && (!string.IsNullOrEmpty(_request.searchKey) ? c.Name.Contains(_request.searchKey)
                                                || c.Code.Contains(_request.searchKey) : true))
                .Select(s => new GetListUserGroupDTO
                {
                    id = s.Id,
                    name = s.Name,
                    code = s.Code,
                    description = s.Description
                }).ToList();
            _response.lstUserGroup = model;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);
        }
        public void GenerateObjects()
        {

        }

        public void PreValidation()
        {
            //#region Sản phẩm không tồn tại 
            //ConditionCart.AddToCart_C01(_context, _request, _apiCode, _conC01, _conC01Field);
            //#endregion

            //#region Số lượng sản phẩm không đủ 
            //ConditionCart.AddToCart_C02(_context, _request, _apiCode, _conC02, _conC02Field);
            //#endregion
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<GetListUserGroupResponse> Process(GetListUserGroupRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                // PreValidation();
                //GenerateObjects();
                //PostValidation();
                AccessDatabase();
            }
            catch (ACV_Exception ex0)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
                _res.Messages = ex0.Messages;
            }
            catch (Exception ex)
            {
                _res.Status = StatusCodes.Status500InternalServerError.ToString();
                _res.Messages.Add(Message.CreateErrorMessage(_apiCode, _res.Status, ex.Message, string.Empty));
            }
            return _res;

        }
    }
}
