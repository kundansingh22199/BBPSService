using BBPSService.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace BBPSService.Controllers
{
    [RoutePrefix("api")]
    public class FetchElectricityBillController : ApiController
    {
        CommonResponse comm = new CommonResponse();
        ClsMasterDb User = new ClsMasterDb();
        ClsStatusResponse ObjStatus = new ClsStatusResponse();
        string ErrMsg = "ERR";
        string SucMsg = "TXN";

        [Route("FetchElectricityBill")]
        [HttpPost]
        [BasicAuthentication]
        public CommonResponse FetchElectricityBill(JObject JObj)
        {
            try
            {
                string OpId = JObj["OpId"].ToString();
                string TokenNo = GetToken();
                string apiTokenNo = JObj["apiTokenNo"].ToString();
                string ConsumerNo = JObj["ConsumerNo"].ToString();
                string ServiceId = JObj["ServiceId"].ToString();
                if (string.IsNullOrEmpty(OpId) || string.IsNullOrEmpty(ConsumerNo) || string.IsNullOrEmpty(ServiceId) || string.IsNullOrEmpty(apiTokenNo))
                {
                    comm = ObjStatus.returnMessage(ErrMsg, "Validation Error", "Data Not Found");
                    return comm;
                }
                else if (string.IsNullOrEmpty(TokenNo))
                {
                    comm = ObjStatus.returnMessage(ErrMsg, "Token Not Found, Pass Token No", "Data Not Found");
                    return comm;
                }
                else
                {
                    DataTable dtUser = User.GetUserData(TokenNo); 
                    if (dtUser.Rows.Count>0)
                    {
                        DataTable dt = User.GetServiceData(ServiceId);
                        if (dt.Rows.Count > 0)
                        {
                            try
                            {
                                string OpCode = User.GetOperatorCodeById(OpId);
                                if (!string.IsNullOrWhiteSpace(OpCode))
                                {
                                    DataTable dt1 = User.GetOperatorById(OpId);
                                    try
                                    {
                                        if (dt1.Rows.Count > 0)
                                        {
                                            string OpName = dt1.Rows[0]["OperatorName"].ToString();
                                            //string apiTokenNo = "cdd6e75cfdd323825f0efbdc2dbd61e8";
                                            string url = "https://www.mplan.in/api/electricinfo.php?apikey=" + apiTokenNo + "&offer=roffer&tel=" + ConsumerNo + "&operator=" + OpCode;
                                            var client = new RestClient(url);
                                            client.Timeout = -1;
                                            var request = new RestRequest(Method.GET);
                                            IRestResponse response = client.Execute(request);
                                            string content = response.Content;
                                            //content = "{\"tel\":\"128203899377\",\"operator\":\"NBPDCL\",\"records\":[{\"CustomerName\":\"AJAY KUMAR SINGH\",\"Billamount\":\"0\",\"Billamountupto10\":\"0\",\"Billamountafter10\":\"0\",\"Billdate\":\"OCT-23\",\"Duedate\":\"2023-11-02\",\"status\":1}],\"status\":1}";
                                            //content = "{\"tel\":\"151545825\",\"operator\":\"BSESY\",\"records\":[{\"CustomerName\":\"ARCHANA BHARTI -\",\"BillNumber\":\"8826933861\",\"Billdate\":\"07-Nov-2023\",\"Billamount\":\"250\",\"Duedate\":\"07-Nov-2023\",\"status\":1}],\"status\":1}";
                                            try
                                            {
                                                JObject obj = JObject.Parse(content);
                                                string RespStatus = obj["status"].ToString();
                                                if (RespStatus == "1")
                                                {
                                                    string record = obj["records"].ToString();
                                                    JArray JArr = JArray.Parse(record);
                                                    string BillStatus = JArr[0]["status"].ToString();
                                                    if (BillStatus == "1")
                                                    {
                                                        
                                                        Random rd = new Random();
                                                        string BillPayValidator = "BP" + rd.Next(11111, 99999) + DateTime.Now.ToString("yyyyMMddHHmmss");
                                                        string CustomerName = JArr[0]["CustomerName"].ToString();
                                                        string BillNumber = ConsumerNo;//JArr[0]["BillNumber"].ToString();
                                                        string Billdate = JArr[0]["Billdate"].ToString();
                                                        string Billamount = JArr[0]["Billamount"].ToString();
                                                        string Duedate = JArr[0]["Duedate"].ToString();
                                                        FatchBillModel FBM = new FatchBillModel();
                                                        FBM.CustomerName = CustomerName;
                                                        FBM.ConsumerNo = BillNumber;
                                                        FBM.Billdate = Billdate;
                                                        FBM.Billamount = Convert.ToDouble(Billamount);
                                                        FBM.Duedate = Duedate;
                                                        //string data = $"{{\"CustomerName\":\"{CustomerName}\",\"BillNumber\":\"{BillNumber}\",\"Billdate\":\"{Billdate}\",\"Billamount\":\"{Billamount}\",\"Duedate\":\"{Duedate}\"}}";
                                                        comm = ObjStatus.returnMessage(SucMsg, "Successfull Fatch", FBM);
                                                        int result = User.SaveBillValidator(ConsumerNo, CustomerName, BillPayValidator, Billamount, OpId, ServiceId, OpCode, Billdate, Duedate, apiTokenNo, TokenNo);
                                                        return comm;
                                                    }
                                                    else
                                                    {
                                                        comm = ObjStatus.returnMessage(ErrMsg, "Bill Status False", "Data Not Found");
                                                        return comm;
                                                    }
                                                }
                                                else
                                                {
                                                    //string resp = "{\"records\":{\"msg\":\"You are not authorize.\",\"yourip\":\"124.123.100.115\"},\"status\":0}";
                                                    string msg = obj["records"]["msg"].ToString();
                                                    comm = ObjStatus.returnMessage(ErrMsg, msg, "Data Not Found");
                                                    return comm;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                comm = ObjStatus.returnMessage(ErrMsg, ex.Message, content);
                                                return comm;
                                            }
                                        }
                                        else
                                        {
                                            comm = ObjStatus.returnMessage(ErrMsg, "Operator Not Exist / Invalid Operator", "Operator Not Exist");
                                            return comm;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        comm = ObjStatus.returnMessage(ErrMsg, ex.Message, "Operator Not Exist / Invalid Operator");
                                        return comm;
                                    }

                                }
                                else
                                {
                                    comm = ObjStatus.returnMessage(ErrMsg, "Please check operator code / invalid op code.!", "Please check operator code / invalid op code.!");
                                    return comm;
                                }
                            }
                            catch (Exception ex)
                            {
                                comm = ObjStatus.returnMessage(ErrMsg, ex.Message, "Invalid Service Id/ Service Status False");
                                return comm;
                            }
                        }
                        else
                        {
                            comm = ObjStatus.returnMessage(ErrMsg, "Service Not Exists/ Service Status False", "Service Not Found");
                            return comm;
                        }
                    }
                    else
                    {
                        comm = ObjStatus.returnMessage(ErrMsg, "Invalid Token / User Not Exist", "User Not Found");
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
