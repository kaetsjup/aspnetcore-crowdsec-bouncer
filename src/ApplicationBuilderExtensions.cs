using Microsoft.AspNetCore.Builder;

namespace AspNetCore.CrowdSec;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCrowdSecBouncer(this IApplicationBuilder app)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));
        app.UseMiddleware<CrowdSecMiddleware>();
        return app;
    }
}