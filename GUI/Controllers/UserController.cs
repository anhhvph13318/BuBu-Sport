using GUI.Controllers.Shared;
using GUI.Model_DTO.User_DTO;
using GUI.Models.DTOs.User_DTO.Views;
using GUI.Shared.CommonSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Net;
using GUI.Shared;
using Newtonsoft.Json;
using GUI.FileBase;
using NuGet.Protocol;

namespace GUI.Controllers
{
    public class UserController : ControllerSharedBase
    {
        private HttpService httpService;
        public UserController(IOptions<CommonSettings> settings)
        {
            _settings = settings.Value;
            httpService = new();
        }
        // GET: UserController
        public async Task<ActionResult> Index(string s)
        {
            var obj = new GetListUserRequest();
            var model = new IndexObject();
            obj.UserName = string.IsNullOrEmpty(s) ? "" : s;
            var URL = _settings.APIAddress + "api/GetListUser/Process";
            //foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(param))
            //{
            //    if (!string.IsNullOrEmpty(property.GetValue(param)?.ToString()))
            //    {
            //        request.AddParameter(property.Name, property.GetValue(param)?.ToString());

            //    }
            //}
            var param = JsonConvert.SerializeObject(obj);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post,"application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<GetListUserResponse>>(res) ?? new();

            model.Data = result.Data;

            return View(model);
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserRequest user)
        {
            try
            {
                user.Password = "12345678";
                var URL = _settings.APIAddress + "api/CreateUser/Process";
                var param = JsonConvert.SerializeObject(user);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<GetListUserResponse>>(res) ?? new();
                if (result.Status == "400")
                {
                    ModelState.AddModelError("UserName", result.Messages.FirstOrDefault().MessageText);
                    return Empty;
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var URL = _settings.APIAddress + "api/DetailUser/Process";
            var req = new DetailUserRequest() { Id = id };
            var param = JsonConvert.SerializeObject(req);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<DetailUserResponse>>(res) ?? new();
            var model = result.Data;
            return View(model);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserRequest user)
        {
            try
            {
                var URL = _settings.APIAddress + "api/EditUser/Process";
                //var req = new DetailUserRequest() { Id = id };
                var param = JsonConvert.SerializeObject(user);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<DetailUserResponse>>(res) ?? new();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            var URL = _settings.APIAddress + "api/DetailUser/Process";
            var req = new DetailUserRequest() { Id = id };
            var param = JsonConvert.SerializeObject(req);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<DetailUserResponse>>(res) ?? new();
            var model = result.Data;
            return View(model);
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDelete(Guid id)
        {
            try
            {
                var URL = _settings.APIAddress + "api/DeleteUser/Process";
                var req = new DetailUserRequest() { Id = id };
                var param = JsonConvert.SerializeObject(req);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
