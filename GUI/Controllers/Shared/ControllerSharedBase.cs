using GUI.Shared.CommonSettings;
using Microsoft.AspNetCore.Mvc;

namespace GUI.Controllers.Shared
{
    public class ControllerSharedBase : Controller
    {
        public CommonSettings _settings = new();
    }
}
