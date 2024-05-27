using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System;
using DATN_ACV_DEV.Controllers.Customer;
using DATN_ACV_DEV.Controllers.Account;
using DATN_ACV_DEV.Model_DTO.Account_DTO;
using DATN_ACV_DEV.Model_DTO.Customer_DTO;
using Microsoft.Extensions.Caching.Memory;

namespace DATN_ACV_DEV.Controllers.Authentication
{
    [Route("api/RegiterAndForgetAccount")]
    [ApiController]
    public class RegiterAndForgetAccountController : ControllerBase, IBaseController<CreatedAccountRequest, LoginResponse>
    {
        public IConfiguration _Configuration { get; }
        private readonly DBContext _context;
        private CreatedAccountRequest _request;
        private BaseResponse<LoginResponse> _res;
        private LoginResponse _response;
        private string _apiCode = "RegiterAndForgetAccount";
        private TbUser _User;
        private string bodymail;
        private string coderesgiter;
        private static string storedValue;

        public RegiterAndForgetAccountController(DBContext context, IConfiguration configuration)
        {
            _context = context;
            _res = new BaseResponse<LoginResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new LoginResponse();
            _Configuration = configuration;


        }
        public void AccessDatabase()
        {
            throw new NotImplementedException();
        }
        public string GenerateRandomString(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] randomArray = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomArray[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomArray);
        }

        public void CheckAuthorization()
        {
            if (_request.Key == 1)
            {
                Random random = new Random();
                bodymail = GenerateRandomString(10, random);


                #region MyRegion
                StoreValue(bodymail, TimeSpan.FromMinutes(5));

                // Retrieve the value after the expiration time
                string retrievedValue = RetrieveValue();
                #endregion

                string fromMail = "joyeuse.zues@gmail.com";
                string fromPassword = "lfamsrxuyzqwiglw";

                MailMessage message = new MailMessage();
                message.From = new MailAddress(fromMail);
                message.Subject = "[ĐĂNG KÝ THÀNH CÔNG] THÔNG TIN ĐĂNG NHẬP WEBSITE bán đồ gia dụng ACV";
                message.To.Add(new MailAddress(_request.Email));
                message.Body = "<html><body>" + "[WEBSITE bán đồ gia dụng ACV] Đây là mã đăng ký đăng nhập của bạn vui lòng không chia sẻ cho bất kỳ ai hay tổ chức nào : " + bodymail + " </body></html>";
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true
                };

                smtpClient.Send(message);
            }
        }
        public void StoreValue(string valueToStore, TimeSpan expirationTime)
        {
            // Store the value
            storedValue = valueToStore;

            // Set up a timer to clear the stored value after the expiration time
            Timer timer = new Timer(ClearValue, null, expirationTime, Timeout.InfiniteTimeSpan);

            Console.WriteLine($"Value '{valueToStore}' stored for {expirationTime.TotalMinutes} minutes.");
        }

        public void ClearValue(object state)
        {
            // Clear the stored value
            storedValue = null;

            Console.WriteLine("Value cleared.");
        }

        public string RetrieveValue()
        {
            // Retrieve the value
            string retrievedValue = storedValue;

            // Return the retrieved value
            return retrievedValue;
        }
        public void GenerateObjects()
        {
            string retrievedValue = RetrieveValue();
            if (_request.Code == retrievedValue && _request.Key == 0)
            {
                var id = new CreateAccountController(_context).Process(_request);
                if (id != null && id.Data.ID != null)
                {
                    _response.Message = "Đăng ký thành công";
                }
            }
            if (_request.Code == retrievedValue && _request.Key == 2)
            {
                var checkphone = _context.TbAccounts.Where(c => c.PhoneNumber == _request.PhoneNumber).Select(c => c.Id).FirstOrDefault();
                if (checkphone != null && checkphone != Guid.Empty)
                {
                    EditAccountRequest _requestEdit = new EditAccountRequest();
                    _requestEdit.Password = _request.Password;
                    _requestEdit.ID = checkphone;
                    _requestEdit.PhoneNumber = _request.PhoneNumber;
                    _requestEdit.Email = _request.Email;
                    _requestEdit.AccountCode = "KHBR";
                    var id = new EditAccountController(_context).Process(_requestEdit);
                    if (id != null && id.Data.ID != null)
                    {
                        _response.Message = "Mật khẩu mới cập nhật thành công";
                    }
                }
            }
            if (_request.Code != retrievedValue && (_request.Key == 0 || _request.Key == 2))
            {
                _response.Message = "Mã đăng ký không đúng, vui lòng kiểm tra lại !!!";
            }
            if (_request.Key == 1)
            {
                _response.Message = "Đã có một đoạn mã được gửi vào mail bạn đã đăng ký, vui lòng kiểm tra mail và nhập mã để hoàn thành thao tác !!!";
            }
            _res.Data = _response;
        }

        public void PreValidation()
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<LoginResponse> Process(CreatedAccountRequest request)
        {
            try
            {
                _request = request;
                CheckAuthorization();
                //PreValidation();
                GenerateObjects();
                //PostValidation();
                //AccessDatabase();
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
