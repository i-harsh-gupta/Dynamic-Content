namespace DynamicContent
{
    public class RouteConfig
    {
        public static IEndpointRouteBuilder Use(IEndpointRouteBuilder route)
        {
            route.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action}/{id?}");

            route.MapControllerRoute("DynamicPage", "{*pageUrl}", new { controller = "Post", action = "DynamicPage" });

            return route;

        }
    }
}
