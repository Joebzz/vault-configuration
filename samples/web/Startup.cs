using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HashiCorp.Vault.DotNetCore.Configuration.Sample.WebApp
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                var testValue1 = _config["my-value"];
                var testValue2 = _config["my-value-2"];
                var testValue3 = _config["my-value-3"];
                await context.Response.WriteAsync($"Hello World! Test Value from Vault 1: {testValue1}; Test Value from Vault 2: {testValue2}; Test Value from Vault 3: {testValue3}; ");
            });
        }
    }
}
