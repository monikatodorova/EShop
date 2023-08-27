using EShop.Domain.DomainModels;
using EShop.Domain.DTO;
using EShop.Domain.Identity;
using EShop.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EShop.Web.Controllers
{
    public class ShoppingCartController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<EShopApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<EShopApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedUser = await _context.Users
                .Where(z => z.Id == userId)
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.ProductInShoppingCarts)
                .Include("UserCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();

            var userShoppingCart = loggedUser.UserCart;
            var productPrice = userShoppingCart.ProductInShoppingCarts.Select(z => new
            {
                ProductPrice = z.Product.ProductPrice,
                Quantity = z.Quantity
            }).ToList();

            double totalPrice = 0;
            foreach (var product in productPrice)
            {
                totalPrice += product.ProductPrice * product.Quantity;
            }

            //var allProducts = userShoppingCart.ProductInShoppingCarts.Select(z => z.Product).ToList();

            ShoppingCartDto ShoppingCartDtoItem = new ShoppingCartDto
            {
                Products = userShoppingCart.ProductInShoppingCarts.ToList(),
                TotalPrice = totalPrice
            };

            return View(ShoppingCartDtoItem);
        }

        public async Task<IActionResult> DeleteFromShoppingCart(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId) && id != null)
            {
                var loggedUser = await _context.Users
                .Where(z => z.Id == userId)
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.ProductInShoppingCarts)
                .Include("UserCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();

                var userShoppingCart = loggedUser.UserCart;

                var itemToDelete = userShoppingCart.ProductInShoppingCarts.Where(z => z.ProductId.Equals(id)).FirstOrDefault();
                userShoppingCart.ProductInShoppingCarts.Remove(itemToDelete);

                _context.Update(userShoppingCart);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "ShoppingCart");
        }


        public async Task<IActionResult> Order()
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                var loggedUser = await _context.Users
                .Where(z => z.Id == userId)
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.ProductInShoppingCarts)
                .Include("UserCart.ProductInShoppingCarts.Product")
                .FirstOrDefaultAsync();

                var userShoppingCart = loggedUser.UserCart;

                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    User = loggedUser,
                    UserId = userId
                };
                _context.Add(order);
                await _context.SaveChangesAsync();

                List<ProductInOrder> productInOrders = new List<ProductInOrder>();

                var result = userShoppingCart.ProductInShoppingCarts.Select(z => new ProductInOrder
                {
                    ProductId = z.ProductId,
                    OrderedProduct = z.Product,
                    OrderId = order.Id,
                    UserOrder = order
                }).ToList();

                productInOrders.AddRange(result);

                foreach (var product in productInOrders)
                {
                    _context.Add(product);
                }
                await _context.SaveChangesAsync();
                loggedUser.UserCart.ProductInShoppingCarts.Clear();

                _context.Update(loggedUser);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Index", "ShoppingCart");
        }
    }
}
