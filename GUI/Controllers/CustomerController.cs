using GUI.Controllers.Shared;
using GUI.Models.DTOs.Customer_DTO;
using GUI.Models.DTOs.Customer_DTO.Views;
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
//using DATN_ACV_DEV.Entity;

namespace GUI.Controllers
{
    public class CustomerController : ControllerSharedBase
    {
        private HttpService httpService;
        //DBContext dBContext;
        public CustomerController(IOptions<CommonSettings> settings)
        {
            _settings = settings.Value;
            httpService = new();
            //dBContext = new DBContext();
        }
        public async Task<ActionResult> Index(string s)
        {
            var obj = new GetListCustomerRequest();
            var model = new IndexObject();
            obj.Name = string.IsNullOrEmpty(s) ? "" : s;
            var URL = _settings.APIAddress + "api/GetListCustomer/Process";
            var param = JsonConvert.SerializeObject(obj);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<GetListCustomerResponse>>(res) ?? new();

            model.Data = result.Data;

            return View(model);
        }

        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateCustomerRequest obj)
        {
            try
            {
                var URL = _settings.APIAddress + "api/CreateCustomer/Process";
                var param = JsonConvert.SerializeObject(obj);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<GetListCustomerResponse>>(res) ?? new();
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
        public async Task<ActionResult> Edit(Guid id)
        {
            var URL = _settings.APIAddress + "api/DetailCustomer/Process";
            var req = new DetailCustomerRequest() { Id = id };
            var param = JsonConvert.SerializeObject(req);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<DetailCustomerResponse>>(res) ?? new();
            var model = result.Data;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditCustomerRequest user)
        {
            try
            {
                var URL = _settings.APIAddress + "api/EditCustomer/Process";
                var param = JsonConvert.SerializeObject(user);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                var result = JsonConvert.DeserializeObject<BaseResponse<DetailCustomerResponse>>(res) ?? new();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            var URL = _settings.APIAddress + "api/DetailCustomer/Process";
            var req = new DetailCustomerRequest() { Id = id };
            var param = JsonConvert.SerializeObject(req);
            var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
            var result = JsonConvert.DeserializeObject<BaseResponse<DetailCustomerResponse>>(res) ?? new();
            var model = result.Data;
            return View(model);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmDelete(Guid id)
        {
            try
            {
                var URL = _settings.APIAddress + "api/DeleteCustomer/Process";
                var req = new DetailCustomerRequest() { Id = id };
                var param = JsonConvert.SerializeObject(req);
                var res = await httpService.PostAsync(URL, param, HttpMethod.Post, "application/json");
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
