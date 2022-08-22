using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SampleProject.Model;
using SampleProject.Abstraction;

namespace SampleProject
{
    public class SampleFunction
    {
        private readonly IServiceSample _serviceSample;

        public SampleFunction(IServiceSample serviceSample)
        {
            _serviceSample = serviceSample;
        }

        [FunctionName("SampleFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "samplefunction")]
            HttpRequest req,
            ILogger log)
        {
            try
            {
                string stringRequest = await new StreamReader(req.Body).ReadToEndAsync();
                SampleFunctionRequest request = JsonConvert.DeserializeObject<SampleFunctionRequest>(stringRequest);

                if (string.IsNullOrEmpty(request.Field1))
                {
                    log.LogInformation("Invalid request object");
                    return new BadRequestResult();
                }

                await _serviceSample.Insert();

                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogInformation($"Exception = {ex.Message}");
                return new StatusCodeResult(500);
            }
        }
    }
}

