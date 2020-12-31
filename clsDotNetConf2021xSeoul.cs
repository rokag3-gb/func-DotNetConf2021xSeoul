using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace DotNetConf2021xSeoul
{
    public static class clsDotNetConf2021xSeoul
    {
        [FunctionName("Order")]
        public static async Task<IActionResult> Order(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log
            )
        {
            //string resourceId = "subscriptions/f743325c-9857-4f13-bff9-bc1976f25693/resourceGroups/RG-JW-DotNetConf2021xSeoul/providers/Microsoft.Web/sites/func-DotNetConf2021xSeoul";
            //string ResourceInfo = GetResourceInfo(resourceId);

            try
            {
                log.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} : Http trigger 펑션 {req.Host.ToString()} 이 시작되었습니다.");
                
                NETX mNETX = new NETX();
                mNETX.Add("@MemberNo", DataType.Int, 0, 1066370);
                mNETX.Add("@ProductNo", DataType.Int, 0, 132456);
                mNETX.Add("@OrderQty", DataType.Int, 0, 1);
                //mNETX.Add("@IsCancel", DataType.Bit, 0, "0");
                mNETX.Add("@Region", DataType.VarChar, 100, _Region);
                mNETX.Add("@Host", DataType.VarChar, 100, req.Host);
                mNETX.Add("@APIPath", DataType.VarChar, 200, req.Path);

                mNETX.ExecuteNonQuery("DotNet2021xSeoul.dbo.USP_T_ORDERS", mNETX.GetDS());

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

                System.Security.Claims.ClaimsIdentity Claim = req.GetAppServiceIdentity();

                Version ver = Environment.Version; // .NET framework version 이 구해진다.
                
                responseMessage = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}, " +
                    $"Host = {req.Host.ToString()}, " +
                    $"Path = {req.Path.ToString()}, " +
                    //$"PathBase = {req.PathBase.ToString()}, " +
                    //$"ClaimsIdentity = {Claim.Name}, " +
                    $"Method = {req.Method}, " +
                    $"Body = {requestBody}, " +
                    $"responseMessage = {responseMessage}, " +
                    //$"ResourceInfo = {ResourceInfo}, " +
                    $"APIVersion = {String.Format("v{0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision)}";

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
        public static async Task<IActionResult> OrderCancel(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log
            )
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

        //private static readonly string _Region = "Korea Central";
        private static readonly string _Region = "West US";

        /// <summary>
        /// 특정 resourceId 에 대한 정보를 가져옵니다. https://docs.microsoft.com/ko-kr/rest/api/resources/resources/getbyid
        /// </summary>
        /// <param name="resourceId">리소스 이름 및 리소스 종류를 포함 하는 리소스의 정규화 된 ID입니다. /Subscriptions/{guid}/resourceGroups/{resource-group-name}/{resource-provider-namespace}/{resource-type}/{resource-name} 형식을 사용 합니다.</param>
        /// <returns></returns>
        private static string GetResourceInfo(string resourceId)
        {
            string Url = $"https://management.azure.com/{resourceId}?api-version=2020-06-01";

            string Ret = string.Empty;

            try
            {
                System.Net.HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);

                req.Timeout = 10000; // 10sec
                req.ReadWriteTimeout = 10000; // 10sec
                req.Method = "GET";
                req.ContentType = "application/json";
                req.Accept = "application/json";
                req.Headers.Add("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjVPZjlQNUY5Z0NDd0NtRjJCT0hIeEREUS1EayIsImtpZCI6IjVPZjlQNUY5Z0NDd0NtRjJCT0hIeEREUS1EayJ9.eyJhdWQiOiJodHRwczovL21hbmFnZW1lbnQuY29yZS53aW5kb3dzLm5ldC8iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC82ZDVhYzhlZS0zODYyLTQ0NTItOTNlNy1hODM2YzJkOTc0MmIvIiwiaWF0IjoxNjA5MTQzMzU2LCJuYmYiOjE2MDkxNDMzNTYsImV4cCI6MTYwOTE0NzI1NiwiYWNyIjoiMSIsImFpbyI6IkFVUUF1LzhTQUFBQUM1Q1dlajFEKzZpbjcyTkpSS1huK25BYkt4UHRIM2RJaFRwTGs0SmJBT25FRUVpdFBBd2JaS0ZqZG4xSFM0OVY2TVhzdGczY3Mwb21reU9GSHV1RmhnPT0iLCJhbXIiOlsicHdkIiwibWZhIl0sImFwcGlkIjoiN2Y1OWE3NzMtMmVhZi00MjljLWEwNTktNTBmYzViYjI4YjQ0IiwiYXBwaWRhY3IiOiIyIiwiZmFtaWx5X25hbWUiOiJLaW0iLCJnaXZlbl9uYW1lIjoiSnVuZ3dvbyIsImdyb3VwcyI6WyI4ZDViMjkyZS1kZDgyLTRkNjItODYwZS0xZjg3MDA2Mzc2OWEiLCI5YTk4NmZlZC1lOTM1LTRhNzMtYTdhMS05YzdkYmFjYWU3NGYiLCI3NzZlZTY4MS0wOWI2LTRkMGMtYmVhOS1kZTI5ZWM2YTAwODMiLCIwM2Y0Mzc3OC0xZGViLTRiNWMtODVmYy0zOGExZDc5ZTRiZmIiLCJmZjk0YmIxMS1jODBlLTQ1ZjctYjVhYi1hNDM2OWEwMTdkNzYiLCJkMTM5M2JjMS05ODFmLTRkODEtYTA5YS0wNzM4YWI1NTIwZjYiXSwiaXBhZGRyIjoiMS4yMDkuMTcuMzUiLCJuYW1lIjoiSnVuZ3dvbyBLaW0iLCJvaWQiOiJjZGVhYWQ5Yy1hZGZmLTQwZGItYTFkNy1lZWY3NGNlMDA4Y2EiLCJwdWlkIjoiMTAwMzIwMDBFQTFEMjAxRCIsInJoIjoiMC5BU3NBN3NoYWJXSTRVa1NUNTZnMnd0bDBLM09uV1gtdkxweENvRmxRX0Z1eWkwUXJBRFEuIiwic2NwIjoidXNlcl9pbXBlcnNvbmF0aW9uIiwic3ViIjoiRE1iTGhlb0xZaXc1V1VzR29pWkpHckpudXY1dm9kNUhtcVZKbjhMVkxHdyIsInRpZCI6IjZkNWFjOGVlLTM4NjItNDQ1Mi05M2U3LWE4MzZjMmQ5NzQyYiIsInVuaXF1ZV9uYW1lIjoianVuZ3dvby5raW1AY2xvdWRtdC5jby5rciIsInVwbiI6Imp1bmd3b28ua2ltQGNsb3VkbXQuY28ua3IiLCJ1dGkiOiJNUUtVX2ZocVdVMmQtVEJoOUt5NEFRIiwidmVyIjoiMS4wIiwid2lkcyI6WyI2MmU5MDM5NC02OWY1LTQyMzctOTE5MC0wMTIxNzcxNDVlMTAiLCJiNzlmYmY0ZC0zZWY5LTQ2ODktODE0My03NmIxOTRlODU1MDkiXSwieG1zX3RjZHQiOjE1NDMyMzU5Njh9.bkugvwPCt5BlbXDoU8zpB5hzVTuIdax6HSaBPpzXfyzvvkSn4BJOumDmrlTx9_awmIzM7wrF9ec8dNYRCPE9a2-UJPqlG5b7ATlndv2I11RPgy3RFb3IAF7Sa6Pbtk7IgahppFIrHNYpBDjNyEPEo3pC2cWjuPE6PMPgZNrJIvT5QKN7Emc3ONKlHUuKCMo9OoJcLpLcNrvEOitWTv1VpZhX_YbT4eP6XcA1hHf4yLu0rLVnfgYxxjprLHq1VyZhPGdY8mwpMbW9ZffmpMltXRuXrseqW8u8hm3IasZgMCE1nB7eug1hWJUBWl_uBOfF1I-yuAgJaV9tty3sVjXH0w");

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                using (StreamReader resStream = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                {
                    Ret += resStream.ReadToEnd() + "\r\n";
                }
            }
            catch (WebException ex)
            {
                using (WebResponse res = ex.Response)
                {
                    HttpWebResponse hwres = (HttpWebResponse)res;

                    if (res == null)
                    {
                        Ret += "-- Exception --\r\n" + ex.Message.ToString();
                    }
                    else
                    {
                        using (StreamReader resStream = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                        {
                            Ret += "-- Exception --\r\n" + resStream.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                Ret += "\r\n";
            }

            return Ret;
        }
    }
}