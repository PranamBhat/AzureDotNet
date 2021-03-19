using System;
using System.Collections.Generic;

namespace Pranam.Restme.BigBuyUtils
{
    public class BigBuyOrder
    {
        public long Id { get; set; }
        public string InternalReference { get; set; }
        public decimal CashOnDelivery { get; set; }
        public string Language { get; set; }
        public string PaymentMethod { get; set; }
        public List<BigBuyShippingCarrier> Carriers { get; set; }
        public BigBuyShippingAddress ShippingAddress { get; set; }
        public List<BigBuyOrderItem> Products { get; set; }
        public DateTime DateAdd { get; set; }
    }
}