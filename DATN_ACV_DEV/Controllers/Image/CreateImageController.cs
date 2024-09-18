using AutoMapper;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/CreateImage")]
    [ApiController]
    public class CreateImageController : ControllerBase, IBaseController<CreateImageRequest, CreateImageResponse>
    {
        private readonly DBContext _context;
        private CreateImageRequest _request;
        private BaseResponse<CreateImageResponse> _res;
        private CreateImageResponse _response;
        private string _apiCode = "CreateImage";
        private TbImage _Image;
        public CreateImageController(DBContext context)
        {
            _context = context;
            _res = new BaseResponse<CreateImageResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new CreateImageResponse();
        }

        public void AccessDatabase()
        {
            _context.Add(_Image);
            _context.SaveChanges();
            _response.ID = _Image.Id;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {
            throw new NotImplementedException();
        }

        public void GenerateObjects()
        {
            _Image = new TbImage()
            {
                Id = Guid.NewGuid(),
                Url = _request.Url,
                Type = _request.Type,
                ProductId = _request.ProductId,
                //Default
                InAcitve = false,
                CreateDate = DateTime.Now,
                CreateBy = Guid.Parse("9a8d99e6-cb67-4716-af99-1de3e35ba993"),
            };
        }
        [HttpPost("Upload")]
        public async Task<string> UploadImage([FromForm] CreateImageRequest request)
        {
            try
            {
                // Tạo HttpClient để gửi yêu cầu tới Cloudinary
                using (var httpClient = new HttpClient())
                {
                    // Tạo content cho form-data
                    using (var formData = new MultipartFormDataContent())
                    {
                        Random random = new Random();
                        string randomTwoDigits = random.Next(10, 100).ToString(); // Sinh ngẫu nhiên 2 số bất kỳ từ 10 đến 99
                        request.PublicId = "SP" + randomTwoDigits;
                        // Thêm các tham số form-data
                        formData.Add(new StringContent("284335524597493"), "api_key");
                        formData.Add(new StringContent("vanh2204"), "upload_preset");
                        formData.Add(new StringContent(request.PublicId), "public_id");

                        // Thêm file vào form-data
                        using (var fileStream = request.File.OpenReadStream())
                        {
                            var fileContent = new StreamContent(fileStream);
                            formData.Add(fileContent, "file", request.File.FileName);
                        }

                        // Gửi POST request tới Cloudinary
                        var response = await httpClient.PostAsync("https://api.cloudinary.com/v1_1/dnvuz3evz/image/upload?Vanh1", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            var resultContent = await response.Content.ReadAsStringAsync();

                            // Parse JSON để lấy URL của ảnh
                            var jsonResponse = JObject.Parse(resultContent);
                            var imageUrl = jsonResponse["secure_url"]?.ToString(); // Lấy giá trị từ key "secure_url"

                            if (string.IsNullOrEmpty(imageUrl))
                            {
                                throw new Exception("Không thể lấy URL từ Cloudinary.");
                            }

                            // Trả về URL của ảnh đã upload
                            return imageUrl;
                        }
                        else
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            throw new Exception($"Cloudinary error: {errorMessage}");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Log lỗi và ném lại lỗi để trả về client
                // Ví dụ: Bạn có thể sử dụng ILogger để ghi lại lỗi ở đây
                throw new Exception($"Error occurred: {ex.Message}");
            }
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<CreateImageResponse> Process(CreateImageRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
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
