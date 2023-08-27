﻿using EShop.Domain.Identity;
using System;
using System.Collections.Generic;

namespace EShop.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public EShopApplicationUser User { get; set; }
        public IEnumerable<ProductInOrder> ProductInOrders { get; set; }
    }
}
