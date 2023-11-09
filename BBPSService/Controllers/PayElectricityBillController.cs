using BBPSService.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace BBPSService.Controllers
{
    [RoutePrefix("api")]
    public class PayElectricityBillController : ApiController
    {
        CommonResponse comm = new CommonResponse();
        ClsMasterDb User = new ClsMasterDb();
        ClsStatusResponse ObjStatus = new ClsStatusResponse();
        string ErrMsg = "ERR";
        string SucMsg = "TXN";
        [Route("PayElectricityBill")]
        [HttpPost]
        [BasicAuthentication]
        public CommonResponse PayElectricityBill(JObject JObj)
        {
            try
            {
                string ConsumerNo = JObj["ConsumerNo"].ToString();
                string BillAmount = JObj["BillAmount"].ToString();
                string TokenNo = GetToken();
                if (string.IsNullOrEmpty(ConsumerNo) || string.IsNullOrEmpty(BillAmount))
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
                    DataTable dtPay = User.GetBillPaymentHistory(ConsumerNo);
                    try
                    {
                        if (dtPay.Rows.Count > 0)
                        {
                            comm = ObjStatus.returnMessage(ErrMsg, "Already Paid", "Already Paid");
                            return comm;
                            //string data = $"{{\"ConsumerNo\":\"{ConsumerNo}\",\"BillAmount\":\"{dtPay.Rows[0]["BillAmount"]}\",\"TransactionId\":\"{dtPay.Rows[0]["TransactionId"]}\",\"PaymentDate\":\"{dtPay.Rows[0]["PaymentDate"]}\",\"PaidBy\":\"{dtPay.Rows[0]["PaidBy"]}\"}}";
                        }
                        else
                        {
                            DataTable dtUser = User.GetUserData(TokenNo);
                            try
                            {
                                if (dtUser.Rows.Count > 0)
                                {
                                    decimal commission = 0, tds = 0, gst = 0, surcharge = 0, trxamt = 0 ;
                                    int GstType = 0, CommType = 0, tdsType = 0, SurChargeType = 0;
                                    string commissionType = dtUser.Rows[0]["CommissionType"].ToString();
                                    int schemeId = Convert.ToInt32(dtUser.Rows[0]["SchemeID"]);

                                    decimal Balance = Convert.ToDecimal(dtUser.Rows[0]["WalletBalance"]);
                                    string UserId = dtUser.Rows[0]["UserID"].ToString();
                                    int UId = Convert.ToInt32(dtUser.Rows[0]["Id"]);

                                    DataTable dt = User.GetBillValidatorByConsumerId(ConsumerNo);
                                    try
                                    {
                                        if (dt.Rows.Count > 0)
                                        {
                                            string BillValidatorId = dt.Rows[0]["BillValidatorId"].ToString();
                                            decimal ValidatorAmount = Convert.ToDecimal(dt.Rows[0]["BillAmount"]);
                                            int OperatorId = Convert.ToInt32(dt.Rows[0]["OperatorId"]);
                                            int ServiceId = Convert.ToInt32(dt.Rows[0]["ServiceId"]);
                                            string Billdate = dt.Rows[0]["Billdate"].ToString();
                                            string Duedate = dt.Rows[0]["Duedate"].ToString();
                                            string apiTokenNo = dt.Rows[0]["apiTokenNo"].ToString();
                                            string UserTokenNo = dt.Rows[0]["UserTokenNo"].ToString();
                                            string CustomerName = dt.Rows[0]["CustomerName"].ToString();

                                            DateTime CreateDate = Convert.ToDateTime(dt.Rows[0]["CreateDate"]);
                                            DateTime NowDate = DateTime.Now;
                                            TimeSpan timeDifference = NowDate - CreateDate;
                                            int minutesDifference = (int)timeDifference.TotalMinutes;
                                            if (minutesDifference <= 20)
                                            {
                                                if (Convert.ToDecimal(BillAmount) == ValidatorAmount)
                                                {
                                                    if (commissionType.Trim().ToLower() == "scheme")
                                                    {
                                                        DataTable dtComm = User.GetCommission(UId, schemeId, OperatorId, ServiceId, "schemewise");
                                                        if(dtComm!=null && dtComm.Rows.Count > 0)
                                                        {
                                                            commission = Convert.ToDecimal(dtComm.Rows[0]["Commission"]);
                                                            tds = Convert.ToDecimal(dtComm.Rows[0]["TDS"]);
                                                            surcharge = Convert.ToDecimal(dtComm.Rows[0]["Surcharge"]);
                                                            gst = Convert.ToDecimal(dtComm.Rows[0]["GST"]);

                                                            CommType = Convert.ToInt32(dtComm.Rows[0]["CommissionType"]);
                                                            GstType = Convert.ToInt32(dtComm.Rows[0]["GSTType"]);
                                                            SurChargeType = Convert.ToInt32(dtComm.Rows[0]["SurchargeType"]);
                                                            tdsType = Convert.ToInt32(dtComm.Rows[0]["TDSType"]);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        DataTable dtComm = User.GetCommission(UId, schemeId, OperatorId, ServiceId, "userwise");
                                                        if (dtComm != null && dtComm.Rows.Count > 0)
                                                        {
                                                            commission = Convert.ToDecimal(dtComm.Rows[0]["Commission"]);
                                                            tds = Convert.ToDecimal(dtComm.Rows[0]["TDS"]);
                                                            surcharge = Convert.ToDecimal(dtComm.Rows[0]["Surcharge"]);
                                                            gst = Convert.ToDecimal(dtComm.Rows[0]["GST"]);

                                                            CommType = Convert.ToInt32(dtComm.Rows[0]["CommissionType"]);
                                                            GstType = Convert.ToInt32(dtComm.Rows[0]["GSTType"]);
                                                            SurChargeType = Convert.ToInt32(dtComm.Rows[0]["SurchargeType"]);
                                                            tdsType = Convert.ToInt32(dtComm.Rows[0]["TDSType"]);
                                                        }
                                                    }
                                                    if (CommType == 1)
                                                    {
                                                        commission = commission * ValidatorAmount / 100;
                                                    }
                                                    if (tdsType == 1)
                                                    {
                                                        tds = tds * commission / 100;
                                                    }
                                                    if (SurChargeType == 1)
                                                    {
                                                        surcharge = surcharge * ValidatorAmount / 100;
                                                    }
                                                    if (GstType == 1)
                                                    {
                                                        gst = gst * surcharge / 100;
                                                    }
                                                    trxamt = ValidatorAmount + (surcharge + gst) - (commission - tds);
                                                    if (Balance > trxamt)
                                                    {
                                                        int result = User.UpdateUserBalance(UserId, TokenNo, Balance - trxamt);
                                                        if (result > 0)
                                                        {
                                                            Random rd = new Random();
                                                            string TransactionId = "TRX" + rd.Next(11111, 99999) + DateTime.Now.ToString("yyyyMMddHHmmss");
                                                            int result2 = User.SaveBillPaymentHistory(ConsumerNo, trxamt, TransactionId, UId.ToString(), commission,CommType,surcharge,SurChargeType,gst,GstType,tds,tdsType);
                                                            //string data= $"{{\"CustomerName\":\"{CustomerName}\",\"Billamount\":\"{BillAmount}\",\"Payamount\":\"{trxamt.ToString("f2")}\",\"TransactionId\":\"{TransactionId}\",\"PayDate\":\"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\"}}";
                                                            DataTable dtTrans = User.GetBillPaymentHistoryByTransactionId(TransactionId);
                                                            string PaymentDate = dtTrans.Rows[0]["PaymentDate"].ToString();

                                                            ClsTransactionHistory trsns = new ClsTransactionHistory();
                                                            trsns.ConsumerNo = ConsumerNo;
                                                            trsns.CustomerName = CustomerName;
                                                            trsns.ValidatorId = BillValidatorId;
                                                            trsns.Amount = trxamt.ToString("f2");
                                                            trsns.TransactionId = TransactionId;
                                                            trsns.PaymentDate = PaymentDate;
                                                            trsns.OperatorId = OperatorId;
                                                            trsns.ServiceId = ServiceId;
                                                            comm = ObjStatus.returnMessage(SucMsg, "Successfull Pay", trsns);
                                                            return comm;
                                                        }
                                                        else
                                                        {
                                                            comm = ObjStatus.returnMessage(ErrMsg, "Somthing Wrong", "UnSuccessfull");
                                                            return comm;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        comm = ObjStatus.returnMessage(ErrMsg, "Wallet Balance Is Low", "UnSuccessfull");
                                                        return comm;
                                                    }
                                                }
                                                else
                                                {
                                                    comm = ObjStatus.returnMessage(ErrMsg, "Balance MissMatch", "UnSuccessfull");
                                                    return comm;
                                                }
                                            }
                                            else
                                            {
                                                comm = ObjStatus.returnMessage(ErrMsg, "TimeOut", "Try Again");
                                                return comm;
                                            }
                                        }
                                        else
                                        {
                                            comm = ObjStatus.returnMessage(ErrMsg, "ValidatorId Not Found", "Try Again");
                                            return comm;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        comm = ObjStatus.returnMessage(ErrMsg, "Invalid Bill Validation Id / Consumer No", "Data Not Found");
                                        return comm;
                                    }
                                }
                                else
                                {
                                    comm = ObjStatus.returnMessage(ErrMsg, "Invalid TokenNo / User Not Exist", "Data Not Found");
                                    return comm;
                                }
                            }
                            catch (Exception)
                            {
                                comm = ObjStatus.returnMessage(ErrMsg, "Invalid User/ TokenNo", "Data Not Found");
                                return comm;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        comm = ObjStatus.returnMessage(ErrMsg, ex.Message, "Invalid Consumer No");
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
