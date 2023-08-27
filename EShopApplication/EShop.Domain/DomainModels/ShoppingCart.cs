﻿using EShop.Domain.Identity;
using System;
using System.Collections.Generic;

namespace EShop.Domain.DomainModels
{
    public class ShoppingCart : BaseEntity
    {
        public string OwnerId { get; set; }
        public EShopApplicationUser Owner { get; set; } 
        public virtual ICollection<ProductInShoppingCart> ProductInShoppingCarts { get; set; }
    }
}
