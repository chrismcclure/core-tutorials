using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalRProject.Hubs;

namespace SignalRProject
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddSignalR();

        }

        [System.Obsolete]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //this is for local host. 
            app.UseCors(builder =>
            {
                //Don't do this in production, just some examples.
                builder.WithOrigins("null")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();

            });

            app.UseSignalR(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
