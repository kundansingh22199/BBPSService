using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBPSService.Models
{
    public class CommonResponse
    {
        public string StatusCode { get; set; }
        public Object StatusMsg { get; set; }
        public Object Data { get; set; }
    }
    public class ClsTransactionHistory
    {
        public string ConsumerNo { get; set; }
        public string CustomerName { get; set; }
        public string ValidatorId { get; set; }
        public string Amount { get; set; }
        public string TransactionId { get; set; }
        public string PaymentDate { get; set; }
        public int OperatorId { get; set; }
        public int ServiceId { get; set; }
    }
    public class ClsOperatorModel
    {
        public string SlNo { get; set; }
        public string OperatorCode { get; set; }
        public string OperatorName { get; set; }
        public string Status { get; set; }
    }
    public class ClsServiceModel
    {
        public string ID { get; set; }
        public string ServiceName { get; set; }
        public string Status { get; set; }
        public string CreateDate { get; set; }
    }
    public class FatchBillModel
    {
        public string CustomerName { get; set; }
        public string ConsumerNo { get; set; }
        public string Billdate { get; set; }
        public double Billamount { get; set; }
        public string Duedate { get; set; }
    }
}