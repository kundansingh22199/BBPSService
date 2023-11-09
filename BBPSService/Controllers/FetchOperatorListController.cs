using BBPSService.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BBPSService.Controllers
{
    [RoutePrefix("api")]
    public class FetchOperatorListController : ApiController
    {
        CommonResponse comm = new CommonResponse();
        ClsMasterDb clsDb = new ClsMasterDb();
        ClsStatusResponse ObjStatus = new ClsStatusResponse();
        string ErrMsg = "ERR";
        string SucMsg = "TXN";
        [Route("FetchOperatorList")]
        [HttpPost]
        [BasicAuthentication]
        public CommonResponse Login(JObject JObj)
        {
            try
            {
                string ServiceId = JObj["ServiceID"].ToString();
                if (string.IsNullOrEmpty(ServiceId))
                {
                    comm = ObjStatus.returnMessage(ErrMsg, "Validation Error", "Data Not Found");
                    return comm;
                }
                else
                {
                    bool Token = clsDb.CheckTokenValid(GetToken());
                    if (Token)
                    {
                        DataTable dt = clsDb.GetOperatorList(ServiceId);
                        try
                        {
                            if (dt.Rows.Count > 0)
                            {
                                comm = ObjStatus.returnMessage(SucMsg, "Successfull Fetch Data", dt);
                                return comm;
                            }
                            else
                            {
                                comm = ObjStatus.returnMessage(ErrMsg, "Somthing Wrong", "Data Not Found");
                                return comm;
                            }
                        }
                        catch (Exception ex)
                        {
                            comm = ObjStatus.returnMessage(ErrMsg, ex.Message, "Data Not Found");
                            return comm;
                        }
                    }
                    else
                    {
                        comm = ObjStatus.returnMessage(ErrMsg, "Somthing Wrong", "Data Not Found");
                        return comm;
                    }    
                }
            }
            catch (Exception ex)
            {
                comm = ObjStatus.returnMessage(ErrMsg, ex.Message, "Data Not Found");
                return comm;
            }
            //return comm;
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
