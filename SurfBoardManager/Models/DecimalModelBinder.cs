using System.Globalization;
using System.Web.Mvc;

namespace SurfBoardManager.Models
{
    public class DecimalModelBinder : DefaultModelBinder
    {
        //public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        //{
        //    var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        //    return valueProviderResult == null ? base.BindModel(controllerContext, bindingContext) : Decimal.Parse(valueProviderResult.AttemptedValue, NumberStyles.Currency);
        //    // of course replace with your custom conversion logic
        //}
    }
}
