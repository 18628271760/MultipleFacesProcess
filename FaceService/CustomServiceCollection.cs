using ArcSoft;
using Microsoft.Extensions.DependencyInjection;

namespace Proprietor.InformationService
{
    public static class CustomServiceCollection
    {
        public static IServiceCollection AddArcSoftFaceService(this IServiceCollection services, Arcsoft_Face_Action enginePool)
        {
            services.AddSingleton<IEnginePoor, Arcsoft_Face_Action>(x => enginePool);
            return services;
        }
    }
}
