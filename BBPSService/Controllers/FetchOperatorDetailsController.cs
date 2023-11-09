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
    public class FetchOperatorDetailsController : ApiController
    {
        CommonResponse comm = new CommonResponse();
        ClsMasterDb clsDb = new ClsMasterDb();
        ClsStatusResponse ObjStatus = new ClsStatusResponse();
        string ErrMsg = "ERR";
        string SucMsg = "TXN";

        [Route("FetchOperatorDetails")]
        [HttpPost]
        [BasicAuthentication]
        public CommonResponse FetchOperatorDetails(JObject JObj)
        {
            try
            {
                string OpId = JObj["OpId"].ToString();
                if (string.IsNullOrEmpty(OpId))
                {
                    comm = ObjStatus.returnMessage(ErrMsg, "Validation Error", "Data Not Found");
                    return comm;
                }
                else
                {
                    bool Token = clsDb.CheckTokenValid(GetToken());
                    if (Token)
                    {
                        DataTable dt = clsDb.GetOperatorById(OpId);
                        try
                        {
                            if (dt.Rows.Count > 0)
                            {
                                ClsOperatorModel opm = new ClsOperatorModel();
                                opm.SlNo = dt.Rows[0]["SlNo"].ToString();
                                opm.OperatorCode = dt.Rows[0]["OperatorCode"].ToString();
                                opm.OperatorName = dt.Rows[0]["OperatorName"].ToString();
                                opm.Status = dt.Rows[0]["Status"].ToString();
                                comm = ObjStatus.returnMessage(SucMsg, "Successfull Fetch Data", opm);
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
                        comm = ObjStatus.returnMessage(ErrMsg, "Invalid Token / Not Exist", "Data Not Found");
                        return comm;
                    }    
                }
            }
            catch (Exception ex)
            {
                comm = ObjStatus.returnMessage(ErrMsg, ex.Message, "Data Not Found");
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
