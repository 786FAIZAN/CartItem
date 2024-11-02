using CartItem.Models;
using CartItem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;

namespace CartItem.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly CartItemRepository _cartItemRepository;
        private readonly ItemRepository _itemRepository;

        public CartItemsController(CartItemRepository cartItemRepository, ItemRepository itemRepository)
        {
            _cartItemRepository = cartItemRepository;
            _itemRepository = itemRepository;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new CartItemViewModel
            {
                CartItems = _cartItemRepository.GetCartItems(userId.Value),
                Items = _itemRepository.GetAllItems(),
            };
            decimal discount = 0;
            decimal totalPrice = viewModel.CartItems.Sum(ci => ci.Quantity * ci.Price);

            var coupon = _cartItemRepository.GetUserCoupon(userId.Value);
            foreach(var item in coupon)
            {
                if (item.IsUsed && item.Code == "CDP10") // Apply 10% discount if the coupon has been used
                {
                    discount = totalPrice * 0.10m;
                }
            }
            ViewBag.TotalPrice = totalPrice - discount;
            // Calculate total price for display
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddToCart(int itemId, int quantity)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var item = _itemRepository.GetItemById(itemId);
            if (item == null || item.AvlQuantity < quantity)
            {
                ViewBag.Error = "Item not available or insufficient quantity.";
                return RedirectToAction("Index");
            }

            var cartItem = new CartItems
            {
                UserId = userId.Value,
                ItemId = itemId,
                ItemName = item.Name,
                Quantity = quantity,
                Price = item.Price,
                AddedDate = DateTime.Now
            };

            _cartItemRepository.AddToCart(cartItem);
            item.AvlQuantity -= quantity;
            _itemRepository.UpdateItem(item);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _cartItemRepository.RemoveFromCart(cartItemId);
            return RedirectToAction("Index");
        }

        public IActionResult ClearCart()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            _cartItemRepository.ClearCart(userId.Value);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ApplyCoupon(string couponCode)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new CartItemViewModel
            {
                CartItems = _cartItemRepository.GetCartItems(userId.Value),
                Items = _itemRepository.GetAllItems()
            };

            decimal discount = 0;
            decimal totalPrice = viewModel.CartItems.Sum(ci => ci.Quantity * ci.Price);
            var coupon = _cartItemRepository.GetCoupon(couponCode);

            // Create a new log entry
            var log = new CouponActionLog
            {
                UserId = userId.Value,
                CouponId = coupon?.Id ?? 0,
                ActionMessage = string.Empty,
                DiscountAmount = null,
                FreeItemCartId = null,
                FreeItemName = null,
                Timestamp = DateTime.Now
            };

            if (coupon != null && coupon.Validity >= DateTime.Now)
            {
                var coupons = _cartItemRepository.GetUserCoupon(userId.Value);
                foreach (var item in coupons)
                {
                    if (item.IsUsed && item.Code == "CDP10" && item.UserId == userId.Value) // Apply 10% discount if the coupon has been used
                    {
                        discount = totalPrice * 0.10m;
                    }
                }
                if (!coupon.IsUsed) // Check if the coupon has not been used
                {
                    // Apply discount logic
                    if (couponCode == "CDP10")
                    {
                        discount = totalPrice * 0.10m;
                        ViewBag.SuccessMessage = "Coupon applied successfully! 10% discount applied.";
                        log.ActionMessage = "Coupon applied successfully! 10% discount applied.";
                    }
                    else if (couponCode == "CDPCAP")
                    {
                        int jeansCount = viewModel.CartItems
                            .Where(ci => ci.ItemName.Contains("Jeans"))
                            .Sum(ci => ci.Quantity);
                        int freeCaps = jeansCount / 2;
                        decimal capPrice = viewModel.Items.FirstOrDefault(i => i.Name == "Caps")?.Price ?? 0;

                        // Add caps to the cart
                        for (int i = 0; i < freeCaps; i++)
                        {
                            var capItem = new CartItems
                            {
                                UserId = userId.Value,
                                ItemId = viewModel.Items.First(i => i.Name == "Caps").ItemId,
                                ItemName = "Caps-Free",
                                Quantity = 1,
                                Price = 0,
                                AddedDate = DateTime.Now
                            };
                            _cartItemRepository.AddToCart(capItem);
                        }

                        ViewBag.SuccessMessage = $"Coupon applied successfully! {freeCaps} cap(s) added for free.";
                        log.ActionMessage = $"Coupon applied successfully! {freeCaps} cap(s) added for free.";
                        log.FreeItemCartId = viewModel.Items.First(i => i.Name == "Caps").ItemId; // Assuming you want to log the ItemId
                        log.FreeItemName = "Caps-Free";
                    }

                    // Update the coupon as used
                    _cartItemRepository.UpdateCoupon(coupon.Id, userId.Value);
                    log.DiscountAmount = discount; // Set the discount amount to log
                }
                else
                {
                    ViewBag.TotalPrice = totalPrice - discount;
                    ViewBag.Error = "This coupon has already been used.";
                    log.ActionMessage = "This coupon has already been used.";
                }
            }
            else
            {
                ViewBag.Error = "Invalid coupon code or coupon expired.";
                log.ActionMessage = "Invalid coupon code or coupon expired.";
            }

            // Log the action
            _cartItemRepository.LogCouponAction(log);

            // Set ViewBag for total price calculation
            ViewBag.AppliedCoupon = couponCode;
            ViewBag.DiscountAmount = discount;
            ViewBag.TotalPrice = totalPrice - discount;

            return View("Index", viewModel);
        }

        public IActionResult CouponActionLog()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve the log entries for the current user
            var logs = _cartItemRepository.GetCouponActionLogs(userId.Value);

            return View(logs); // Return the logs to a view (create a view called CouponActionLogs.cshtml)
        }
    }

}
