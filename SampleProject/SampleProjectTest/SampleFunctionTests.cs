using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using SampleProject;
using SampleProject.Abstraction;
using SampleProject.Model;
using Xunit;

namespace SampleProjectTest
{
    public class SampleFunctionTests
    {
        /// <summary>
        /// 1
        /// Objects.
        /// </summary>
        private readonly SampleFunction _sampleFunction;
        private readonly ILogger<SampleFunction> _logger;
        private readonly IServiceSample _serviceSample;

        /// <summary>
        /// 2
        /// Mock request object. The test functions will use the following model in the request.
        /// </summary>
        private readonly SampleFunctionRequest mockRequest = new SampleFunctionRequest()
        {
            Field1 = "Test field",
            Field2 = true,
            Field3 = 0
        };

        public SampleFunctionTests()
        {
            /*
             * In this part, NSubstitute creates a fake object for the logger.
             * This fake logger object will handle the requests in the SampleFunction.
             */
            _logger = Substitute.For<ILogger<SampleFunction>>();

            /*
             * In this part, NSubstitute creates a fake object for IServiceSample.
             * In the following part, we decide what to return for the specific requests.
             * In this test, we call a task without returning an object. It is possible to define a specific response in the returns pharantes. 
             */
            _serviceSample = Substitute.For<IServiceSample>();
            _serviceSample.Insert().Returns(Task.CompletedTask);
            _serviceSample.Update().Returns(Task.CompletedTask);
            _serviceSample.Delete().Returns(Task.CompletedTask);

            // This instance will be used in the test cases. It uses the mock interface as a parameter.
            _sampleFunction = new SampleFunction(serviceSample: _serviceSample);
        }

        /// <summary>
        /// All test cases should have a fact attribute just on the top.
        /// Test case's name should give you information about what you are testing.
        /// In this test case, we try to test the request object. Because of that, I used null HttpRequest instead of mock request.
        /// Also we have a mock logger in the test case. It means the function will not throw an exception because of null/invalid logger objects.
        /// </summary>
        [Fact]
        public async Task SampleFunction_WithInvalidRequestObject_ShouldReturn_BadRequest()
        {
            var response = await _sampleFunction.Run(MockRequest(new SampleFunctionRequest()), _logger);

            var responseObject = response as BadRequestResult;

            Assert.NotNull(responseObject);
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)responseObject!.StatusCode);
        }

        /// <summary>
        /// In this case, I just updated my mock object for this test case and threw an exception from the insert task.
        /// If you compare this case with the previous one, this one uses a valid request object.
        /// It is possible to update mock objects in the test case like this one.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SampleFunction_WithServiceInsertException_ShouldReturn_ServerError()
        {
            _serviceSample.Insert().Returns(Task.FromException(new Exception("Sample Exception")));

            var response = await _sampleFunction.Run(MockRequest(mockRequest), _logger);

            var responseObject = response as StatusCodeResult;

            Assert.NotNull(responseObject);
            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)responseObject!.StatusCode);
        }

        /// <summary>
        /// If you put all mocks as expected, the function should return an ok response.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SampleFunction_WithValidRequestAndMocks_ShouldReturn_Success()
        {
            var response = await _sampleFunction.Run(MockRequest(mockRequest), _logger);

            var responseObject = response as OkResult;

            Assert.NotNull(responseObject);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)responseObject!.StatusCode);
        }

        /// <summary>
        /// This function, creates a mock HttpRequest object for the function call. 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static HttpRequest MockRequest(object body)
        {
            var json = JsonConvert.SerializeObject(body);

            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Body = memoryStream;
            request.ContentType = "application/json";

            return request;
        }
    }
}