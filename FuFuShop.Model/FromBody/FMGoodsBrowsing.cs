using FuFuShop.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuFuShop.Model.FromBody
{
    public class FMGoodsBrowsing
    {           
        /// <summary>
        /// 
        /// </summary>
        public string Time { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        public List<GoodsBrowsing> Goods { get; set; } 
    }
}
