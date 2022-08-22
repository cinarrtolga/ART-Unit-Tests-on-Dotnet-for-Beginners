using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SampleProject.Abstraction;
using SampleProject.Services;

[assembly: FunctionsStartup(typeof(SampleProject.Startup))]
namespace SampleProject
{
	public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IServiceSample, ServiceSample>();
        }
    }
}

