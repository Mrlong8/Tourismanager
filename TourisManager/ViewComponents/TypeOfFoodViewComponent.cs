using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TourisManager.Core.Interfaces;
using TourisManager.Services;

namespace TourisManager.ViewComponents
{
    public class TypeOfFoodViewComponent : ViewComponent
    {
        private ITypeOfFoodService _service;

        public TypeOfFoodViewComponent(ITypeOfFoodService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listType = await  _service.GetListTypeFoodsAsync();
            return View(listType);
        }
    }
}
