using E_Book.DataAccess.Repository.IRepository;
using E_Book.Models;
using E_Book.Models.ViewModels;
using E_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace E_Book.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),

                OrderHeader = new()
            };
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),
                OrderHeader = new()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstorDeafult(x => x.Id == claims.Value);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.PaymentStatus = StaticData.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = StaticData.StatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claims.Value;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();


            foreach (var cart in ShoppingCartVM.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    ProductId = cart.ProductId,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
        
            // Stripe Settings 
            var domain = "https://localhost:44301/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes= new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>(), 
                Mode = "payment",
                SuccessUrl = domain+$"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",    
                CancelUrl = domain+$"customer/cart/Index",
            
            };
            foreach(var item in ShoppingCartVM.ListCart)
            {
               var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),//2000
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "T-shirt",
                        },

                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();
            Session session = service.Create(options);
           
            // Get SessionId and PaymentIntentId
            _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id,session.Id,session.PaymentIntentId);
            _unitOfWork.Save();
            
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            //_unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            //_unitOfWork.Save();
            //return RedirectToAction("Index", "Home");
        }

        public IActionResult OrderConfirmation(int cartId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstorDeafult(x => x.Id == cartId);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            //Check Stripe Status
            if(session.PaymentStatus.ToLower()=="paid")
            {
                _unitOfWork.OrderHeader.UpdateStatus(cartId,StaticData.StatusApproved,StaticData.PaymentStatusApproved);
            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(x=> x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.Save();
            return RedirectToAction("Index", "Home");

        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstorDeafult(x => x.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstorDeafult(x => x.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstorDeafult(x => x.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public double GetPriceBasedOnQuantity(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
            {
                return price;
            }
            else
            {
                if (quantity <= 100)
                {
                    return price50;
                }
                else
                {
                    return price100;
                }
            }
        }
    }
}
