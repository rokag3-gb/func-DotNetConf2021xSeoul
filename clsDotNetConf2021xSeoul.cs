using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotNetConf2021xSeoul
{
    public static class clsDotNetConf2021xSeoul
    {
        [FunctionName("Order")]
        public static async Task<IActionResult> Order([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                log.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} : Http trigger 펑션 {req.Host.ToString()} 이 시작되었습니다.");

                NETX_parameter Param = new NETX_parameter();
                Param.ADD("@MemberNo", DataType.Int, 0, "1066370");
                Param.ADD("@ProductNo", DataType.Int, 0, "132456");
                Param.ADD("@OrderQty", DataType.Int, 0, "1");
                //Param.ADD("@IsCancel", DataType.Bit, 0, "0");
                Param.ADD("@RegHost", DataType.VarChar, 100, req.Host.ToString());
                Param.ADD("@RegAPIPath", DataType.VarChar, 200, req.Path.ToString());

                NETX mNETX = new NETX();
                mNETX.쿼리ExecuteNonQuery("DotNet2021xSeoul.dbo.USP_T_ORDERS", Param.GetDS());

                string name = req.Query["name"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;

                //string responseMessage = string.IsNullOrEmpty(name)
                //    ? "이 http 트리거 펑션이 성공적으로 실행되었습니다. 개인화된 응답을 위해 쿼리 문자열 또는 요청 본문에 이름을 전달할 수 있습니다." // "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                //    : $"안녕하세요, {name}님. 이 http 트리거 펑션이 성공적으로 실행되었습니다." // $"Hello, {name}. This HTTP triggered function executed successfully."
                //    ;

                //string responseMessage = string.IsNullOrEmpty(name)
                //    ? "이 http 트리거 펑션이 성공적으로 실행되었습니다. 개인화된 응답을 위해 쿼리 문자열 또는 요청 본문에 이름을 전달할 수 있습니다."
                //    : $"안녕하세요, {name}님. 이 http 트리거 펑션이 성공적으로 실행되었습니다."
                //    ;

                string responseMessage = "데이터베이스에 값을 정상적으로 기록했습니다.";

                responseMessage = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} : {req.Host.ToString()}, {req.Path.ToString()}, {responseMessage}";
                
                return new OkObjectResult(responseMessage);
            }
            catch (Exception Ex)
            {
                return new OkObjectResult(Ex.Message);
            }
            //finally
            //{
            //}
        }

        [FunctionName("OrderCancel")]
        public static async Task<IActionResult> OrderCancel([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} : Http trigger 펑션 {req.Host.ToString()} 이 시작되었습니다.");
            
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "이 http 트리거 펑션이 성공적으로 실행되었습니다. 개인화된 응답을 위해 쿼리 문자열 또는 요청 본문에 이름을 전달할 수 있습니다." // "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"안녕하세요, {name}님. 이 http 트리거 펑션이 성공적으로 실행되었습니다." // $"Hello, {name}. This HTTP triggered function executed successfully."
            //    ;
            string responseMessage = string.IsNullOrEmpty(name)
                ? "이 http 트리거 펑션이 성공적으로 실행되었습니다. 개인화된 응답을 위해 쿼리 문자열 또는 요청 본문에 이름을 전달할 수 있습니다."
                : $"안녕하세요, {name}님. 이 http 트리거 펑션이 성공적으로 실행되었습니다."
                ;
            responseMessage = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} : {req.Host.ToString()}, {req.Path.ToString()}, {responseMessage}";
            //HttpRequest.Host = localhost:7071
            //HttpRequest.Path = /api/FuncDotNet2021xSeoul_Order
            return new OkObjectResult(responseMessage);
        }
    }
}