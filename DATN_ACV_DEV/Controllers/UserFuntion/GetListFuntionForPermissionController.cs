using AutoMapper;
using DATN_ACV_DEV.Controllers.Condition;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Cart_DTO;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.Funtion_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Permission_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/GetListFuntionForPermission")]
    [ApiController]
    public class GetListFuntionForPermissionController : ControllerBase, IBaseController<GetListFuntionForPermissionRequest, List<GetListFuntionForPermissionResponse>>
    {
        private readonly DBContext _context;
        private GetListFuntionForPermissionRequest _request;
        private BaseResponse<List<GetListFuntionForPermissionResponse>> _res;
        private List<GetListFuntionForPermissionResponse> _response;
        private string _apiCode = "GetListFuntionForPermission";
        private TbCart _Cart;
        private TbCartDetail _CartDetail;
        private TbProduct _product;
        private bool checkCart = false;
        private bool checkProduct = false;
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        public GetListFuntionForPermissionController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<List<GetListFuntionForPermissionResponse>>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new List<GetListFuntionForPermissionResponse>();
        }

        public void AccessDatabase()
        {
            var model = _context.TbFuntionForPermissions.Select(x => new GetListFuntionForPermissionResponse
            {
                funtionForPermissionId = x.Id,
                nameFuntion = x.Funtion.Name,
                lstPermission = _context.TbPermissions.Where(c => c.Id == x.Permission.Id)
                .Select(s => new GetListPermissionDTO
                {
                    id = s.Id,
                    name = s.Name
                }).ToList(),
            }).ToList();
            _response = model;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            _request.Authorization(_context, _apiCode);
        }
        public void GenerateObjects()
        {

            _res.Data = _response;
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
        public BaseResponse<List<GetListFuntionForPermissionResponse>> Process(GetListFuntionForPermissionRequest request)
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
