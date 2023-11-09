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
    public class UserLoginController : ApiController
    {
        CommonResponse comm = new CommonResponse();
        ClsMasterDb clsDb = new ClsMasterDb();
        ClsStatusResponse ObjStatus = new ClsStatusResponse();
        string ErrMsg = "ERR";
        string SucMsg = "TXN";

        [Route("UserLogin")]
        [HttpPost]
        [BasicAuthentication]
        public CommonResponse Login(JObject JObj)
        {
            try
            {
                string UserId = JObj["UserId"].ToString();
                string Password = JObj["Password"].ToString();
                string TokenNo = GetToken();
                if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Password))
                {
                    comm = ObjStatus.returnMessage(ErrMsg, "Validation Failed", "Data Not Found");
                    return comm;
                }
                else if (string.IsNullOrEmpty(TokenNo))
                {
                    comm = ObjStatus.returnMessage(ErrMsg, "Token Not Found, Pass Token No", "Data Not Found");
                    return comm;
                }
                else
                {
                    
                    bool CheckTokenValid = clsDb.CheckTokenValid(TokenNo);
                    try
                    {
                        if (CheckTokenValid)
                        {
                            DataTable dt = clsDb.GetUserLogin(UserId, Password);
                            if (dt.Rows.Count > 0)
                            {
                                int result = clsDb.SaveLoginHistory(UserId, Password, "Login", TokenNo);
                                comm = ObjStatus.returnMessage(SucMsg, "Successfull Login", dt);
                                return comm;
                            }
                            else
                            {
                                comm = ObjStatus.returnMessage(ErrMsg, "User Not Exist", "Failed");
                                return comm;
                            }
                        }
                        else
                        {
                            int result = clsDb.SaveLoginHistory(UserId, Password, "Login Failed", TokenNo);
                            comm = ObjStatus.returnMessage(ErrMsg, "Invalid Token / User Not Exist", "Login Failed");
                            return comm;
                        }
                    }
                    catch (Exception ex)
                    {
                        comm = ObjStatus.returnMessage(ErrMsg, ex.Message, "Login Failed");
                        return comm;
                    }
                }
            }
            catch (Exception ex)
            {
                comm = ObjStatus.returnMessage(ErrMsg, ex.Message, "Login Failed");
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
