using BBPSService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace BBPSService.Controllers
{
    [RoutePrefix("api")]
    public class GetServiceListController : ApiController
    {
        CommonResponse comm = new CommonResponse();
        ClsMasterDb User = new ClsMasterDb();
        ClsStatusResponse resp = new ClsStatusResponse();
        string ErrMsg = "ERR";
        string SucMsg = "TXN";
        [Route("GetServiceList")]
        [HttpGet]
        public CommonResponse GetServiceList()
        {
            try
            {
                //string Token= GetToken();
                bool Token = User.CheckTokenValid(GetToken());
                if (Token)
                {
                    DataTable dt = User.GetServiceList();
                    if (dt.Rows.Count > 0)
                    {
                        comm = resp.returnMessage(SucMsg, "Successfull Fetch", dt);
                        return comm;
                    }
                    else
                    {
                        comm = resp.returnMessage(ErrMsg, "Somthing Error", "Service List Not Fetch / Empty Service List");
                        return comm;
                    }
                }
                else
                {
                    comm = resp.returnMessage(ErrMsg, "Invalid Token", "Data Not Found");
                    return comm;
                }
            }
            catch (Exception ex)
            {
                comm = resp.returnMessage(ErrMsg, ex.Message, "Service Not Found");
                return comm;
            }
        }
        private string GetToken()
        {
            string HToken = "";
            HttpRequestMessage request = Request;
            if (request.Headers.TryGetValues("Token", out IEnumerable<string> Token))
            {
                HToken = Token.FirstOrDefault();
            }
            return HToken;
        }
    }
}
