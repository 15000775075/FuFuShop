using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuFuShop.Model.FromBody
{

    //Api接口====================================================================================================
    /// <summary>
    /// 前端接口提交售后发货快递信息
    /// </summary>
    public class FMBillReshipForSendReshipPost
    {

        public string logiCode { set; get; }

        public string logiNo { get; set; }

        public string reshipId { get; set; }

    }
}