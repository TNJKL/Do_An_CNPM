using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebSiteBanMoHinh.Models;
using WebSiteBanMoHinh.Models.ViewModels;
using WebSiteBanMoHinh.Repository;

namespace WebSiteBanMoHinh.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext _context)
        {
            _dataContext = _context;
        }

        //public IActionResult Index()
        //{
        //    List<CartItemModel> cartItems = HttpContext.Session.GetJSon<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
        //    var shippingPriceCookie = Request.Cookies["ShippingPrice"];
        //    double shippingPrice = 0;
        //    var coupon_code = Request.Cookies["CouponTitle"];

        //    if (shippingPriceCookie != null)
        //    {
        //        shippingPrice = JsonConvert.DeserializeObject<double>(shippingPriceCookie);
        //    }

        //    CartItemViewModel cartVM = new()
        //    {
        //        CartItems = cartItems,
        //        GrandTotal = cartItems.Sum(x => x.Quantity * x.Price),
        //        ShippingCost = shippingPrice,
        //        CouponCode = coupon_code
        //    };

        //    var address = HttpContext.Session.GetJSon<AddressModel>("ShippingAddress");
        //    if (address != null)
        //    {
        //        TempData["City"] = address.City;
        //        TempData["District"] = address.District;
        //        TempData["Ward"] = address.Ward;
        //        TempData["HouseNumber"] = address.HouseNumber;
        //    }

        //    return View(cartVM);
        //}

        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJSon<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            double shippingPrice = 0;
            var coupon_code = Request.Cookies["CouponTitle"];

            if (shippingPriceCookie != null)
            {
                shippingPrice = JsonConvert.DeserializeObject<double>(shippingPriceCookie);
            }

            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price),
                ShippingCost = shippingPrice,
                CouponCode = coupon_code
            };

            var address = HttpContext.Session.GetJSon<AddressModel>("ShippingAddress");
            if (address != null)
            {
                TempData["City"] = address.City;
                TempData["District"] = address.District;
                TempData["Ward"] = address.Ward;
                TempData["HouseNumber"] = address.HouseNumber;
                TempData["FullName"] = address.FullName;
                TempData["PhoneNumber"] = address.PhoneNumber;
            }

            return View(cartVM);
        }

        public IActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Add(long Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            List<CartItemModel> cart = HttpContext.Session.GetJSon<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

            if (cartItem == null)
            {
                cart.Add(new CartItemModel(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);
            int totalCount = cart.Sum(x => x.Quantity);

            return Json(new { success = true, count = totalCount, message = "Thêm vào giỏ hàng thành công" });
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            List<CartItemModel> cart = HttpContext.Session.GetJSon<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            int count = cart.Sum(x => x.Quantity);
            return Json(new { count = count });
        }

        [HttpPost]
        //public async Task<IActionResult> Decrease(long Id)
        //{
        //    List<CartItemModel> cart = HttpContext.Session.GetJSon<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
        //    CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
        //    if (cartItem == null)
        //    {
        //        return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng" });
        //    }

        //    if (cartItem.Quantity > 1)
        //    {
        //        --cartItem.Quantity;
        //    }
        //    else
        //    {
        //        cart.RemoveAll(p => p.ProductId == Id);
        //    }

        //    if (cart.Count == 0)
        //    {
        //        HttpContext.Session.Remove("Cart");
        //    }
        //    else
        //    {
        //        HttpContext.Session.SetJson("Cart", cart);
        //    }

        //    var grandTotal = cart.Sum(x => x.Quantity * x.Price);
        //    int totalCount = cart.Sum(x => x.Quantity);

        //    return Json(new
        //    {
        //        success = true,
        //        message = "Giảm số lượng thành công",
        //        grandTotal = grandTotal.ToString("#,##0 VNĐ"),
        //        count = totalCount,
        //        quantity = cartItem?.Quantity ?? 0,
        //        itemTotal = (cartItem != null ? cartItem.Quantity * cartItem.Price : 0).ToString("#,##0 VNĐ")
        //    });
        //}

        public async Task<IActionResult> Decrease(long Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJSon<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng" });
            }

            bool isRemoved = false;
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
                isRemoved = true;
            }

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            var grandTotal = cart.Sum(x => x.Quantity * x.Price);
            int totalCount = cart.Sum(x => x.Quantity);

            return Json(new
            {
                success = true,
                message = isRemoved ? "Đã xóa sản phẩm khỏi giỏ hàng" : "Giảm số lượng thành công",
                grandTotal = grandTotal.ToString("#,##0 VNĐ"),
                count = totalCount,
                quantity = isRemoved ? 0 : cartItem.Quantity,
                itemTotal = (isRemoved ? 0 : cartItem.Quantity * cartItem.Price).ToString("#,##0 VNĐ")
            });
        }

        [HttpPost]
        public async Task<IActionResult> Increase(long Id)
        {
            ProductModel product = await _dataContext.Products.Where(p => p.Id == Id).FirstOrDefaultAsync();
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            List<CartItemModel> cart = HttpContext.Session.GetJSon<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng" });
            }

            if (cartItem.Quantity < product.Quantity)
            {
                ++cartItem.Quantity;
                HttpContext.Session.SetJson("Cart", cart);
                var grandTotal = cart.Sum(x => x.Quantity * x.Price);
                int totalCount = cart.Sum(x => x.Quantity);

                return Json(new
                {
                    success = true,
                    message = "Tăng số lượng thành công",
                    grandTotal = grandTotal.ToString("#,##0 VNĐ"),
                    count = totalCount,
                    quantity = cartItem.Quantity,
                    itemTotal = (cartItem.Quantity * cartItem.Price).ToString("#,##0 VNĐ")
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Số lượng tối đa trong kho",
                    grandTotal = cart.Sum(x => x.Quantity * x.Price).ToString("#,##0 VNĐ"),
                    count = cart.Sum(x => x.Quantity),
                    quantity = cartItem.Quantity,
                    itemTotal = (cartItem.Quantity * cartItem.Price).ToString("#,##0 VNĐ")
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(long Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJSon<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            if (!cart.Any(p => p.ProductId == Id))
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng" });
            }

            cart.RemoveAll(p => p.ProductId == Id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            var grandTotal = cart.Sum(x => x.Quantity * x.Price);
            int totalCount = cart.Sum(x => x.Quantity);

            return Json(new
            {
                success = true,
                message = "Xóa sản phẩm thành công",
                grandTotal = grandTotal.ToString("#,##0 VNĐ"),
                count = totalCount
            });
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            return Json(new
            {
                success = true,
                message = "Xóa toàn bộ giỏ hàng thành công",
                grandTotal = "0 VNĐ",
                count = 0
            });
        }

        [HttpPost]
        //public async Task<IActionResult> GetShipping(ShippingModel shippingModel, string tinh, string quan, string phuong, string houseNumber)
        //{
        //    var existingShipping = await _dataContext.Shippings
        //        .FirstOrDefaultAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

        //    decimal shippingPrice = existingShipping?.Price ?? 50000;

        //    var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
        //    var cookieOptions = new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Expires = DateTimeOffset.UtcNow.AddMinutes(30),
        //        Secure = true
        //    };
        //    Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);

        //    var address = new AddressModel
        //    {
        //        City = tinh,
        //        District = quan,
        //        Ward = phuong,
        //        HouseNumber = houseNumber
        //    };
        //    HttpContext.Session.SetJson("ShippingAddress", address);

        //    return Json(new
        //    {
        //        success = true,
        //        shippingPrice = shippingPrice.ToString("#,##0 VNĐ"),
        //        address = address
        //    });
        //}

        public async Task<IActionResult> GetShipping(ShippingModel shippingModel, string tinh, string quan, string phuong, string houseNumber, string fullName, string phoneNumber)
        {
            var existingShipping = await _dataContext.Shippings
                .FirstOrDefaultAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

            decimal shippingPrice = existingShipping?.Price ?? 50000;

            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                Secure = true
            };
            Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);

            var address = new AddressModel
            {
                City = tinh,
                District = quan,
                Ward = phuong,
                HouseNumber = houseNumber,
                FullName = fullName,
                PhoneNumber = phoneNumber
            };
            HttpContext.Session.SetJson("ShippingAddress", address);

            return Json(new
            {
                success = true,
                shippingPrice = shippingPrice.ToString("#,##0 VNĐ"),
                address = address
            });
        }

        [HttpPost]
        public IActionResult DeleteShipping()
        {
            Response.Cookies.Delete("ShippingPrice");
            HttpContext.Session.Remove("ShippingAddress");
            return Json(new
            {
                success = true,
                message = "Xóa phí vận chuyển thành công",
                shippingPrice = "0 VNĐ"
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetCoupon(CouponModel couponModel, string coupon_value)
        {
            var validCoupon = await _dataContext.Coupons
                .FirstOrDefaultAsync(x => x.Name == coupon_value && x.Quantity >= 1);

            if (validCoupon == null)
            {
                return Json(new { success = false, message = "Mã coupon không tồn tại hoặc đã hết" });
            }

            TimeSpan remainingTime = validCoupon.DateExpired - DateTime.Now;
            int daysRemaining = remainingTime.Days;

            if (daysRemaining < 0)
            {
                return Json(new { success = false, message = "Mã coupon đã hết hạn" });
            }

            string couponTitle = validCoupon.Name + " | " + validCoupon.Description;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("CouponTitle", couponTitle, cookieOptions);

            return Json(new
            {
                success = true,
                message = "Áp dụng coupon thành công",
                couponTitle = couponTitle
            });
        }
    }

    public class AddressModel
    {
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string HouseNumber { get; set; }

        //them vao 
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }
}