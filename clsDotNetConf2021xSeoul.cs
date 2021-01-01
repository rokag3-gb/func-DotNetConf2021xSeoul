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
                log.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}" + "\r\n" +
                    " Host {req.Host.ToString()} 에서 Http trigger - Order 이(가) 시작되었습니다.");

                string Body = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(Body);
                
                int MemberNo = Convert.ToInt32(data?["MemberNo"]);
                int ProductNo = Convert.ToInt32(data?["ProductNo"]);
                int OrderQty = Convert.ToInt32(data?["OrderQty"]);

                if (String.IsNullOrEmpty(MemberNo.ToString()) || MemberNo == 0)
                    throw new Exception("request body 에서 MemberNo 값을 추출할 수 없습니다.");

                if (String.IsNullOrEmpty(ProductNo.ToString()) || ProductNo == 0)
                    throw new Exception("request body 에서 ProductNo 값을 추출할 수 없습니다.");

                if (String.IsNullOrEmpty(OrderQty.ToString()) || OrderQty == 0)
                    throw new Exception("request body 에서 OrderQty 값을 추출할 수 없습니다.");

                NETX mNETX = new NETX();
                mNETX.Add("@MemberNo", DataType.Int, 0, MemberNo);
                mNETX.Add("@ProductNo", DataType.Int, 0, ProductNo);
                mNETX.Add("@OrderQty", DataType.Int, 0, OrderQty);
                mNETX.Add("@Region", DataType.VarChar, 100, _Region);
                mNETX.Add("@Host", DataType.VarChar, 100, req.Host);
                mNETX.Add("@APIPath", DataType.VarChar, 200, req.Path);

                mNETX.ExecuteNonQuery("DBO.USP_T_ORDERS");

                string responseMessage = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}" + "\r\n" +
                    $"Region = {_Region}" + "\r\n" +
                    $"Host = {req.Host.ToString()}" + "\r\n" +
                    $"Path = {req.Path.ToString()}" + "\r\n" +
                    $"Method = {req.Method}" + "\r\n" +
                    $"Body = {Body}" + "\r\n" +
                    $"Message = Orders 테이블에 데이터를 정상적으로 반영했습니다.";

                return new OkObjectResult(responseMessage);
            }
            catch (Exception Ex)
            {
                log.LogError(Ex, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} : {Ex.Message}", null);

                return new OkObjectResult(Ex.Message);
            }
            finally
            {
                log.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}" + "\r\n" +
                    "Host {req.Host.ToString()} 에서 Http trigger - Order 이(가) 종료되었습니다.");
            }
        }

        [FunctionName("OrderCancel")]
        public static async Task<IActionResult> OrderCancel(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log
            )
        {
            try
            {
                log.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}" + "\r\n" +
                    "Host {req.Host.ToString()} 에서 Http trigger - OrderCancel 이(가) 시작되었습니다.");

                string Body = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(Body);
                
                int OrgOrderNo = Convert.ToInt32(data?["OrgOrderNo"]);

                if (String.IsNullOrEmpty(OrgOrderNo.ToString()) || OrgOrderNo == 0)
                    throw new Exception("request body 에서 OrgOrderNo 값을 추출할 수 없습니다.");

                NETX mNETX = new NETX();
                mNETX.Add("@OrgOrderNo", DataType.Int, 0, OrgOrderNo);
                mNETX.Add("@IsCancel", DataType.Bit, 0, true);
                mNETX.Add("@Region", DataType.VarChar, 100, _Region);
                mNETX.Add("@Host", DataType.VarChar, 100, req.Host);
                mNETX.Add("@APIPath", DataType.VarChar, 200, req.Path);

                mNETX.ExecuteNonQuery("DBO.USP_T_ORDERS");

                string responseMessage = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}" + "\r\n" +
                    $"Region = {_Region}" + "\r\n" +
                    $"Host = {req.Host.ToString()}" + "\r\n" +
                    $"Path = {req.Path.ToString()}" + "\r\n" +
                    $"Method = {req.Method}" + "\r\n" +
                    $"Body = {Body}" + "\r\n" +
                    $"Message = Orders 테이블에 데이터를 정상적으로 반영했습니다.";

                return new OkObjectResult(responseMessage);
            }
            catch (Exception Ex)
            {
                log.LogError(Ex, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} : {Ex.Message}", null);

                return new OkObjectResult(Ex.Message);
            }
            finally
            {
                log.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}"+ "\r\n" +
                    "Host {req.Host.ToString()} 에서 Http trigger - OrderCancel 이(가) 종료되었습니다.");
            }
        }

        private static readonly string _Region = "Korea Central";
        //private static readonly string _Region = "West US";

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