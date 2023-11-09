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
    public class GetServiceByIdController : ApiController
    {
        CommonResponse comm = new CommonResponse();
        ClsMasterDb clsDb = new ClsMasterDb();
        ClsStatusResponse ObjStatus = new ClsStatusResponse();
        string ErrMsg = "ERR";
        string SucMsg = "TXN";

        [Route("GetServiceById")]
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
                        DataTable dt = clsDb.GetServiceData(ServiceId);
                        try
                        {
                            if (dt.Rows.Count > 0)
                            {
                                ClsServiceModel sm = new ClsServiceModel();
                                sm.ID = dt.Rows[0]["ID"].ToString();
                                sm.ServiceName = dt.Rows[0]["ServiceName"].ToString();
                                sm.Status = dt.Rows[0]["Status"].ToString();
                                sm.CreateDate = dt.Rows[0]["CreateDate"].ToString();
                                comm = ObjStatus.returnMessage(SucMsg, "Successfull Fetch Data", sm);
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
        public string GetToken()
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
