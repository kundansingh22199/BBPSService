using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBPSService.Models
{
    public class ClsStatusResponse
    {
        CommonResponse comm = new CommonResponse();
        public CommonResponse returnMessage(string StatusCode, Object StatusMsg, Object Data)
        {
            comm.StatusCode = StatusCode;
            comm.StatusMsg = StatusMsg;
            comm.Data = Data;
            return comm;
        }

    }
}