using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace BBPSService.Models
{
    public class ClsMasterDb : ClsConnection
    {
        public DataTable GetUserLogin(string UserId, string Password)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_LoginMaster", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@Password", Password);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool CheckUserIdExist(string UserId)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SELECT * FROM TblUserMaster WHERE UserId='" + UserId + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public DataTable GetUserData(string TokenNo)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SELECT * FROM TblUserMaster WHERE TokenNo='" + TokenNo + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool CheckTokenValid(string TokenNo)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SELECT * FROM TblUserMaster WHERE TokenNo='" + TokenNo + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public int SaveLoginHistory(string UserId, string Password, string Status, string TokenNo)
        {

            int result = 0;

            try
            {
                double latitude = 0, longitude = 0;

                // Get current location
                using (GeoCoordinateWatcher watcher = new GeoCoordinateWatcher())
                {
                    if (watcher.TryStart(false, TimeSpan.FromMilliseconds(1000)))
                    {
                        GeoCoordinate currentLocation = watcher.Position.Location;

                        if (currentLocation != null)
                        {
                            latitude = currentLocation.Latitude;
                            longitude = currentLocation.Longitude;
                        }
                        else
                        {
                            Console.WriteLine("Unable to determine the current location.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Geolocation is not available on this device.");
                    }
                }

                // Get device information
                string macAddress = GetMacAddress();
                string ipAddress = GetLocalIPAddress();
                string deviceInfo = $"{macAddress}+{latitude}+{longitude}";

                // Save login history to the database
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_SaveLoginHistory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@Password", Password);
                cmd.Parameters.AddWithValue("@TokenNo", TokenNo);
                cmd.Parameters.AddWithValue("@DeviceInfo", deviceInfo);
                cmd.Parameters.AddWithValue("@IpAddress", ipAddress);
                cmd.Parameters.AddWithValue("@MacAddress", macAddress);
                cmd.Parameters.AddWithValue("@Status", Status);
                con.Open();
                result = cmd.ExecuteNonQuery();
                con.Close();
                return result;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"An error occurred: {ex.Message}");
                return 0;
            }

        }
        public DataTable GetServiceData(string Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_GetServiceById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataTable GetOperatorList(string ServiceId)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_GetOperatorList", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ServiceId", ServiceId);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataTable GetServiceList()
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SELECT * FROM TblServiceMaster", con);
                //cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataTable GetOperatorById(string Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_GetOperatorById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OpId", Id);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string GetOperatorCodeById(string Id)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_GetOperatorCodeById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OpId", Id);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["OperatorCode"].ToString();
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
        public int SaveElectricityBill(string UserId, string OpId, string OpName, string ServiceId, string ConsumerNo, string Param1)
        {
            int result = 0;
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_SaveElectricityBill", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@OpId", OpId);
                cmd.Parameters.AddWithValue("@OpName", OpName);
                cmd.Parameters.AddWithValue("@ServiceId", ServiceId);
                cmd.Parameters.AddWithValue("@ConsumerNo", ConsumerNo);
                cmd.Parameters.AddWithValue("@Param1", Param1);
                con.Open();
                result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int SaveBillValidator(string ConsumerNo, string CustomerName, string BillValidatorId, string BillAmount, string OperatorId, string ServiceId, string OperatorCode, string Billdate, string Duedate, string apiTokenNo, string UserTokenNo)
        {
            int result = 0;
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_SaveBillValidator", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ConsumerNo", ConsumerNo);
                cmd.Parameters.AddWithValue("@CustomerName", CustomerName);
                cmd.Parameters.AddWithValue("@BillAmount", Convert.ToDecimal(BillAmount));
                cmd.Parameters.AddWithValue("@BillValidatorId", BillValidatorId);
                cmd.Parameters.AddWithValue("@OperatorId", Convert.ToInt32(OperatorId));
                cmd.Parameters.AddWithValue("@ServiceId", Convert.ToInt32(ServiceId));
                cmd.Parameters.AddWithValue("@OperatorCode", OperatorCode);
                cmd.Parameters.AddWithValue("@Billdate", Convert.ToDateTime(Billdate));
                cmd.Parameters.AddWithValue("@Duedate", Convert.ToDateTime(Duedate));
                cmd.Parameters.AddWithValue("@apiTokenNo", apiTokenNo);
                cmd.Parameters.AddWithValue("@UserTokenNo", UserTokenNo);
                con.Open();
                result = cmd.ExecuteNonQuery();
                con.Close();
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public DataTable GetBillValidatorByConsumerId(string ConsumerNo)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_GetBillValidatorData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ConsumerNo", ConsumerNo);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataTable GetBillPaymentHistoryByTransactionId(string TransactionId)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_GetBillPaymentHistoryByTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TransactionId", TransactionId);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int UpdateUserBalance(string UserId, string TokenNo, decimal BillAmount)
        {
            int result = 0;
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_UpdateUserBalance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@TokenNo", TokenNo);
                cmd.Parameters.AddWithValue("@BillAmount", BillAmount);
                con.Open();
                result = cmd.ExecuteNonQuery();
                con.Close();
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int SaveBillPaymentHistory(string ConsumerNo, decimal BillAmount, string TransactionId, string PaidBy, decimal Commission, int CommissionType, decimal Surcharge, int SurchargeType, decimal GST, int GSTType, decimal TDS, int TDSType)
        {
            int result = 0;
            try
            {
                string MacAddress = GetMacAddress();
                string IPAddress = GetLocalIPAddress();
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_SaveBillPaymentHistory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ConsumerNo", ConsumerNo);
                cmd.Parameters.AddWithValue("@BillAmount", BillAmount);
                cmd.Parameters.AddWithValue("@TransactionId", TransactionId);
                cmd.Parameters.AddWithValue("@IpAddress", IPAddress);
                cmd.Parameters.AddWithValue("@MacAddress", MacAddress);
                cmd.Parameters.AddWithValue("@PaidBy", PaidBy);
                cmd.Parameters.AddWithValue("@Commission", Commission);
                cmd.Parameters.AddWithValue("@CommissionType", CommissionType);
                cmd.Parameters.AddWithValue("@Surcharge", Surcharge);
                cmd.Parameters.AddWithValue("@SurchargeType", SurchargeType);
                cmd.Parameters.AddWithValue("@GST", GST);
                cmd.Parameters.AddWithValue("@GSTType", GSTType);
                cmd.Parameters.AddWithValue("@TDS", TDS);
                cmd.Parameters.AddWithValue("@TDSType", TDSType);
                con.Open();
                result = cmd.ExecuteNonQuery();
                con.Close();
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public DataTable GetBillPaymentHistory(string ConsumerNo)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("SP_GetBillPaymentHistory", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ConsumerNo", ConsumerNo);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataTable GetCommission(int UserId, int SchemeId, int OperatorId, int ServiceId, string Action)
        {
            try
            {
                SqlConnection con = new SqlConnection(strcon);
                SqlCommand cmd = new SqlCommand("GetCommission", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@SchemeId", SchemeId);
                cmd.Parameters.AddWithValue("@OperatorId", OperatorId);
                cmd.Parameters.AddWithValue("@ServiceId", ServiceId);
                cmd.Parameters.AddWithValue("@Action", Action);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        static string GetMacAddress()
        {
            string macAddress = string.Empty;
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    macAddress = networkInterface.GetPhysicalAddress().ToString();
                    break; // Use the first operational network interface found
                }
            }

            return macAddress;
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

    }
}