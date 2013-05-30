using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Abstract
{
    /*
    public class OrderProcessorStatus
    {
        public string statusText { get; set; }
        public bool completed { get; set; }
    }
    */

    public interface IOrderProcessor
    {
        string StatusText { get; set; }
        bool IsDone { get; set; }
        bool HaveToken { get; set; }

        void ProcessOrder(Cart cart, ShippingDetails shippingDetails);        
    }
}