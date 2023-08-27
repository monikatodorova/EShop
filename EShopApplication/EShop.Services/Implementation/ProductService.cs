using EShop.Domain.DomainModels;
using EShop.Domain.DTO;
using EShop.Repository.Interface;
using EShop.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EShop.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductInShoppingCart> _productInShoppingCartRepository;
        private readonly IUserRepository _userRepository;
        public ProductService(IRepository<Product> productRepository, IRepository<ProductInShoppingCart> productInShoppingCartRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _productInShoppingCartRepository = productInShoppingCartRepository;
            _userRepository = userRepository;
        }

        public void CreateNewProduct(Product product)
        {
            this._productRepository.Insert(product);
        }

        public void DeleteProduct(Guid? id)
        {
            var product = this.GetDetailsForProduct(id);
            this._productRepository.Delete(product);
        }

        public List<Product> GetAllProducts()
        {
            return this._productRepository.GetAll().ToList();
        }

        public Product GetDetailsForProduct(Guid? id)
        {
            return this._productRepository.Get(id);
        }

        public void UpdateExistingProduct(Product product)
        {
            this._productRepository.Update(product);
        }

        public bool AddToShoppingCart(AddToShoppingCartDto item, string userId)
        {
            var user = this._userRepository.Get(userId);
            var userShoppingCart = user.UserCart;

            if (item.SelectedProductId != null && userShoppingCart != null)
            {
                var product = this.GetDetailsForProduct(item.SelectedProductId);

                if (product != null)
                {
                    ProductInShoppingCart itemToAdd = new ProductInShoppingCart
                    {
                        Product = product,
                        ProductId = product.Id,
                        ShoppingCart = userShoppingCart,
                        ShoppingCartId = userShoppingCart.Id,
                        Quantity = item.Quantity
                    };

                    this._productInShoppingCartRepository.Insert(itemToAdd);
                    return true;
                }
                return false;
            }
            return false;
        }

        public AddToShoppingCartDto GetShoppingCartInfo(Guid? id)
        {
            var product = this.GetDetailsForProduct(id);
            AddToShoppingCartDto model = new AddToShoppingCartDto
            {
                SelectedProduct = product,
                SelectedProductId = product.Id,
                Quantity = 1
            };

            return model;
        }
    }
}
