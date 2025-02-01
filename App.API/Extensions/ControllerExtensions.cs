using App.API.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Extensions
{
    public static class ControllerExtensions 
    {
        public static IServiceCollection AddControllersWithFiltersExt(this IServiceCollection services) 
        {
            services.AddScoped(typeof(NotFoundFilter<,>));
            services.AddControllers(options =>
            {
                options.Filters.Add<FluentValidationFilter>();
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;  //close .net default nullable validator message
            });
            return services;

        }
    }
}
