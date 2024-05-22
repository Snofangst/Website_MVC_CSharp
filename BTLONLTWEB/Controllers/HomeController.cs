using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BTLONLTWEB.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Dynamic;
using PagedList;
using PagedList.Mvc;

namespace BTLONLTWEB.Controllers
{
    
    public class NoCache : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }
    public static class ExtensionMethods
    {
        public static string Replace2(this string s, char[] separators, string newVal)
        {
            string[] temp;

            temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return String.Join(newVal, temp);
        }
    }
    public class HomeController : Controller
    {
        [HttpPost]
        public ActionResult Login(Login user)
        {
            if (ModelState.IsValid)
            {
                Session["file"] = null;
                Session["ID"] = null;
                Session["User"] = null;
                QuanLyBanHangEntities db = new QuanLyBanHangEntities();
                foreach (var item in db.Users)
                {

                    if ((item.Name == user.Name || (item.Email != null && item.Email.Trim() == user.Name)) && item.Password == user.Password && item.TypeUser.Trim() == "Admin")
                    {
                        Session["ID"] = "1";
                        Session["User"] = item.Name;
                        return RedirectToAction("Index", "Home");
                    }
                    else if ((item.Name == user.Name || (item.Email != null && item.Email.Trim() == user.Name)) && item.Password == user.Password && item.TypeUser.Trim() != "Admin")
                    {
                        Session["ID"] = "2";
                        Session["User"] = item.Name;
                        return RedirectToAction("Index", "Home");
                    }
                }
                ViewBag.Validation = "Sai mật khẩu";
                return View(user);
            }
            else
                return View(user);


        }

        public ActionResult Register()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Register(Information P)
        {

            if (ModelState.IsValid)
            {
                Session["ID"] = null;
                Session["User"] = null;
                Session["file"] = null;
                string IDAvatar = "";
                QuanLyBanHangEntities db = new QuanLyBanHangEntities();
                User s = new User();
                s = db.Users.ToList().LastOrDefault();
                string id = s.IdUser.Remove(0, 4);
                string ID = "USER" + String.Format("{0:D5}", Int32.Parse(id) + 1);
                string Member = "Đồng";
                string TypeUser = "Customer";
                Register a = new Register();
                if (P.Gender == "M")
                    IDAvatar = "~/Image/Avatars/Male.jpg";
                else
                    IDAvatar = "~/Image/Avatars/Female.jpg";
                s = a.Regi(ID, P.Name, P.Email, null, P.Password, P.Gender, null, Member, 0, TypeUser, IDAvatar);
                if (s != null)
                {
                    db.Users.Add(s);
                    db.SaveChanges();
                }
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return View(P);
            }
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {


            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Logout()
        {
            Session["ID"] = null;
            Session["User"] = null;
            Session["file"] = null;
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
        [NoCache]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Account()
        {
            Response.AddHeader("Refresh", "100");
            var i = Session["ID"];
            var j = Session["User"];
            if (i == null)
                return RedirectToAction("Login", "Home");
            else
            {
                QuanLyBanHangEntities db = new QuanLyBanHangEntities();
                User s = db.Users.Where(p => p.Name == j).ToList().FirstOrDefault();

                ViewBag.Image = s.IdAvatar.ToString().Trim();
                Session["Account"] = s;
                return View();
            }
        }
        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [NoCache]
        public ActionResult Account(ChangeInform S)
        {
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();

            User s = db.Users.Where(p => p.IdUser == S.ID).FirstOrDefault();
            s.IdUser = S.ID;
            if (S.Name != null)
                s.Name = S.Name;
            if (S.PhoneNumber != null)
                s.PhoneNumber = S.PhoneNumber;
            if (S.Email != null)
                s.Email = S.Email;
            if (S.NewPassword != null)
                s.Password = S.NewPassword;
            if (S.Address != null)
                s.Address = S.Address;
            if (S.Path != null)
            {
                string name1 = System.IO.Path.GetFileName(S.Path).ToString();
                name1 = Path.Combine(Server.MapPath("~/Image/Temp"), name1);
                if (System.IO.File.Exists(name1) == true)
                {
                    string name2 = System.IO.Path.GetFileName(S.Path).ToString();
                    name2 = Path.Combine(Server.MapPath("~/Image/Avatars"), name2);
                    if (System.IO.File.Exists(name2) == true)
                    {
                        System.IO.File.Delete(name2);

                    }
                    System.IO.File.Move(name1, name2);
                    System.IO.File.Delete(name1);
                    name2 = name2.Replace("\\", "/");
                    name2 = name2.Substring(name2.LastIndexOf("/Image"));
                    name2 = name2.Insert(0, "~");
                    s.IdAvatar = name2;
                    Session["file"] = s.IdAvatar;
                }
                else
                    Session["file"] = S.Path;
            }
            ViewBag.User = s;

            if (ModelState.IsValid)
            {
                Session["User"] = s.Name;
                db.SaveChanges();
                return RedirectToAction("Account", "Home");
            }
            else
                return View(S);
        }
        [HttpPost]
        public ActionResult ImageUpload(ChangeInform P)
        {
            User a = ViewBag.Account;
            if (P.Path != null)
            {
                try
                {
                    QuanLyBanHangEntities db = new QuanLyBanHangEntities();
                    HttpPostedFileBase FILE = Request.Files[0];
                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
                    var extension = Path.GetExtension(FILE.FileName);

                    string filepath = "";
                    if (FILE.ContentLength > 0 && allowedExtensions.Contains(extension))
                    {
                        string filename = Path.GetFileNameWithoutExtension(FILE.FileName);
                        filename = "AVA" + P.ID.Trim().Remove(0, 4);
                        filename = filename + extension;
                        filepath = Path.Combine(Server.MapPath("~/Image/Temp"), filename);
                        FILE.SaveAs(filepath);
                        filepath = filepath.Replace("\\", "/");
                        filepath = filepath.Substring(filepath.LastIndexOf("/Image"));
                        Session["file"] = filepath;
                        ViewBag.Announce = "0";
                        return RedirectToAction("Account", "Home");
                    }
                    else
                    {
                        if (!allowedExtensions.Contains(extension))
                        {
                            ViewBag.Announce = "3";
                            return RedirectToAction("Account", "Home");
                        }
                    }
                    return RedirectToAction("Account", "Home");

                }
                catch
                {
                    ViewBag.Announce = "2";
                    return RedirectToAction("Account", "Home");
                }
            }
            else
            {
                ViewBag.Announce = "1";
                return RedirectToAction("Account", "Home");
            }
        }
        //==============================================Product_Page==========================================//
        public ActionResult Product_PC()
        {
            List<DisplayProd> a = new List<DisplayProd>();
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            var x = (from product in db.Products
                     from image in db.Images
                     from details in db.ImageDetails
                     from catalog in db.Catalogs
                     where details.IdImage.Equals(image.IdImage) && details.IdProduct.Equals(product.IdProduct) && details.ImageType.Equals("ẢNH BÌA") && product.IdCatalog.Equals(catalog.IdCatalog) && catalog.NameCatalog.Trim().Equals("PC")
                     select new { ID = product.IdProduct, Name = product.NameProduct, Path = image.FileImage, Price = product.Price, Decription = product.Description, Discount = product.Discount }).ToList();
            foreach (var item in x)
            {
                DisplayProd b = new DisplayProd();
                b.ID = item.ID;
                b.Name = item.Name;
                b.Path = item.Path;
                b.Price = item.Price;
                if (item.Discount != null)
                    b.Discount = (double)item.Discount;
                b.Description = item.Decription;
                a.Add(b);
            }
            return View(a);
        }
        public ActionResult Product_Mouse()
        {
            List<DisplayProd> a = new List<DisplayProd>();
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            var x = (from product in db.Products
                     from image in db.Images
                     from details in db.ImageDetails
                     from catalog in db.Catalogs
                     where details.IdImage.Equals(image.IdImage) && details.IdProduct.Equals(product.IdProduct) && details.ImageType.Equals("ẢNH BÌA") && product.IdCatalog.Equals(catalog.IdCatalog) && catalog.NameCatalog.Trim().Equals("Mouse")
                     select new { ID = product.IdProduct, Name = product.NameProduct, Path = image.FileImage, Price = product.Price, Decription = product.Description, Discount = product.Discount }).ToList();
            foreach (var item in x)
            {
                DisplayProd b = new DisplayProd();
                b.ID = item.ID;
                b.Name = item.Name;
                b.Path = item.Path;
                b.Price = item.Price;
                if (item.Discount != null)
                    b.Discount = (double)item.Discount;
                b.Description = item.Decription;
                a.Add(b);
            }
            return View(a);
        }
        public ActionResult Product_Keyboard()
        {
            List<DisplayProd> a = new List<DisplayProd>();
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            var x = (from product in db.Products
                     from image in db.Images
                     from details in db.ImageDetails
                     from catalog in db.Catalogs
                     where details.IdImage.Equals(image.IdImage) && details.IdProduct.Equals(product.IdProduct) && details.ImageType.Equals("ẢNH BÌA") && product.IdCatalog.Equals(catalog.IdCatalog) && catalog.NameCatalog.Trim().Equals("Keyboard")
                     select new { ID = product.IdProduct, Name = product.NameProduct, Path = image.FileImage, Price = product.Price, Decription = product.Description, Discount = product.Discount }).ToList();
            foreach (var item in x)
            {
                DisplayProd b = new DisplayProd();
                b.ID = item.ID;
                b.Name = item.Name;
                b.Path = item.Path;
                b.Price = item.Price;
                if (item.Discount != null)
                    b.Discount = (double)item.Discount;
                b.Description = item.Decription;
                a.Add(b);
            }
            return View(a);
        }
        public ActionResult Product_Monitor()
        {
            List<DisplayProd> a = new List<DisplayProd>();
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            var x = (from product in db.Products
                     from image in db.Images
                     from details in db.ImageDetails
                     from catalog in db.Catalogs
                     where details.IdImage.Equals(image.IdImage) && details.IdProduct.Equals(product.IdProduct) && details.ImageType.Equals("ẢNH BÌA") && product.IdCatalog.Equals(catalog.IdCatalog) && catalog.NameCatalog.Trim().Equals("Monitor")
                     select new { ID = product.IdProduct, Name = product.NameProduct, Path = image.FileImage, Price = product.Price, Decription = product.Description, Discount = product.Discount }).ToList();
            foreach (var item in x)
            {
                DisplayProd b = new DisplayProd();
                b.ID = item.ID;
                b.Name = item.Name;
                b.Path = item.Path;
                b.Price = item.Price;
                if (item.Discount != null)
                    b.Discount = (double)item.Discount;
                b.Description = item.Decription;
                a.Add(b);
            }
            return View(a);
        }
        [HttpPost]
        public ActionResult Product_Search(string search)
        {
            List<DisplayProd> a = new List<DisplayProd>();
            if(search=="")
                return View(a);
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            var x = (from product in db.Products
                     from image in db.Images
                     from details in db.ImageDetails
                     from catalog in db.Catalogs
                     where details.IdImage.Equals(image.IdImage) && details.IdProduct.Equals(product.IdProduct) && details.ImageType.Equals("ẢNH BÌA") && product.IdCatalog.Equals(catalog.IdCatalog) && product.NameProduct.ToLower().Contains(search.ToLower().Trim())
                     select new { ID = product.IdProduct, Name = product.NameProduct, Path = image.FileImage, Price = product.Price, Decription = product.Description, Discount = product.Discount }).ToList();
            foreach (var item in x)
            {
                DisplayProd b = new DisplayProd();
                b.ID = item.ID;
                b.Name = item.Name;
                b.Path = item.Path;
                b.Price = item.Price;
                if (item.Discount != null)
                    b.Discount = (double)item.Discount;
                b.Description = item.Decription;
                a.Add(b);
            }
            return View(a);
        }
        //======================================================================================================//
        public ActionResult Details(string id)
        {
            List<string> listDecrip = new List<string>();
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            if (id == null)
                return RedirectToAction("Error", "Home");
            Product x = db.Products.Where(p => p.IdProduct == id).FirstOrDefault();
            List<string> slidelist = new List<string>();
            if (x.Description != null)
                listDecrip = x.Description.ToString().Split(',').ToList();
            var y = (
                     from image in db.Images
                     from details in db.ImageDetails
                     where details.IdImage.Equals(image.IdImage) && details.ImageType.Equals("ẢNH SLIDE") && details.IdProduct.Equals(id)
                     select new { Path = image.FileImage }).ToList();
            foreach (var item in y)
            {
                slidelist.Add(item.Path);
            }
            ViewBag.SlideList = null;
            ViewBag.SlideList = slidelist;
            ViewBag.DecripList = listDecrip;
            ViewBag.Product = x;
            return View();
        }
        [HttpPost]
        public ActionResult Details(string id, Cart ca)
        {
            List<string> listDecrip = new List<string>();
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            if (id == null)
                return RedirectToAction("Error", "Home");
            Product x = db.Products.Where(p => p.IdProduct == id).FirstOrDefault();
            List<string> slidelist = new List<string>();
            if (x.Description != null)
                listDecrip = x.Description.ToString().Split(',').ToList();
            var y = (
                     from image in db.Images
                     from details in db.ImageDetails
                     where details.IdImage.Equals(image.IdImage) && details.ImageType.Equals("ẢNH SLIDE") && details.IdProduct.Equals(id)
                     select new { Path = image.FileImage }).ToList();
            foreach (var item in y)
            {
                slidelist.Add(item.Path);
            }
            ViewBag.SlideList = null;
            ViewBag.SlideList = slidelist;
            ViewBag.DecripList = listDecrip;
            ViewBag.Product = x;
            if (ModelState.IsValid)
            {
                User u = new User();
                if (Session["User"] == null)
                {
                    User annoy = new User();
                    string iduser = "";
                    if (db.Users.Count() == 0)
                        iduser = "ORD" + String.Format("{0:D6}", 1);
                    else
                    {
                        int max = db.Users.ToList().Max(i => Int32.Parse(i.IdUser.Remove(0, 4)));
                        iduser = "USER" + String.Format("{0:D5}", max + 1);
                    }
                    annoy.IdUser = iduser;
                    annoy.Name = iduser;
                    annoy.Member = "NONE";
                    annoy.Gender = "UD";
                    annoy.TypeUser = "Customer";
                    annoy.TotalSpent = 0;
                    annoy.IdAvatar = "~/Image/Avatars/Undefined.png";
                    annoy.Password = " ";

                    db.Users.Add(annoy);
                    Session["User"] = annoy.IdUser;
                    db.SaveChanges();
                    u.IdUser = annoy.IdUser;
                }
                else
                {
                    string name = Session["User"].ToString();
                    u = db.Users.ToList().Where(p => p.Name == name).FirstOrDefault();
                }

                if (db.Orders.ToList().Exists(p => p.IdUser == u.IdUser && (p.Payment == "CHƯA XÁC ĐỊNH" || p.Delivery == "ĐANG LẬP")))
                {
                    Order o = db.Orders.ToList().Where(p => p.IdUser == u.IdUser && (p.Payment == "CHƯA XÁC ĐỊNH" || p.Delivery == "ĐANG LẬP")).FirstOrDefault();
                    if (db.OrderDetails.ToList().Exists(p => p.IdProduct == ca.IDProduct && p.IdOrder == o.IdOrder) == false)
                    {
                        OrderDetail i = new OrderDetail();
                        i.IdOrder = o.IdOrder;
                        i.IdProduct = ca.IDProduct;
                        i.Quantity = ca.Quantity;
                        db.OrderDetails.Add(i);
                        db.SaveChanges();
                    }
                    else
                    {
                        Product prod = db.Products.ToList().Where(p => p.IdProduct == ca.IDProduct).FirstOrDefault();
                        OrderDetail i = db.OrderDetails.ToList().Where(p => p.IdProduct == ca.IDProduct && p.IdOrder == o.IdOrder).FirstOrDefault();
                        i.Quantity += ca.Quantity;
                        if (i.Quantity > prod.Quantity)
                            i.Quantity = (int)prod.Quantity;
                        db.SaveChanges();
                    }
                }
                else
                {
                    string idorder = "";
                    if (db.Orders.Count() == 0)
                        idorder = "ORD" + String.Format("{0:D6}", 1);
                    else
                    {
                        int max = db.Orders.ToList().Max(i => Int32.Parse(i.IdOrder.Remove(0, 3)));
                        idorder = "ORD" + String.Format("{0:D6}", max + 1);
                    }
                    Order o = new Order();
                    o.IdOrder = idorder;
                    o.IdUser = u.IdUser;
                    o.Total = 0;
                    o.Date = DateTime.Now;
                    o.Payment = "CHƯA XÁC ĐỊNH";
                    o.Delivery = "ĐANG LẬP";
                    db.Orders.Add(o);
                    db.SaveChanges();
                    OrderDetail details = new OrderDetail();
                    details.IdOrder = o.IdOrder;
                    details.IdProduct = ca.IDProduct;
                    details.Quantity = ca.Quantity;
                    db.OrderDetails.Add(details);
                    db.SaveChanges();
                }

                return RedirectToAction("Cart", "Home");
            }
            return View(ca);
        }
        public ActionResult Cart()
        {
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            if (Session["User"] != null)
            {
                string name = Session["User"].ToString();
                User x = db.Users.ToList().Where(p => p.Name == name).FirstOrDefault();
                List<Cart> cart = new List<Cart>();

                var i = (from product in db.Products
                         from image in db.Images
                         from imgdetails in db.ImageDetails
                         from order in db.Orders
                         from orddetails in db.OrderDetails
                         where (order.Payment.Equals("CHƯA XÁC ĐỊNH") || order.Delivery.Equals("ĐANG LẬP")) && x.IdUser == order.IdUser && order.IdOrder == orddetails.IdOrder && orddetails.IdProduct == product.IdProduct && product.IdProduct == imgdetails.IdProduct && imgdetails.IdImage == image.IdImage && imgdetails.ImageType.Equals("ẢNH BÌA")
                         select new { IDOrder = order.IdOrder, IDUser = x.IdUser, Name = product.NameProduct, Path = image.FileImage, Price = product.Price, IdProduct = product.IdProduct, Discount = product.Discount, Quantity = orddetails.Quantity, MaxQuantity = product.Quantity }).ToList();
                foreach (var item in i)
                {
                    if (item.MaxQuantity == 0)
                    {
                        OrderDetail a = db.OrderDetails.Where(p => p.IdProduct == item.IdProduct).FirstOrDefault();
                        db.OrderDetails.Remove(a);
                        db.SaveChanges();
                    }
                    else
                    {
                        Cart a = new Cart();
                        a.IDUser = item.IDUser;
                        a.IDProduct = item.IdProduct;
                        a.Image = item.Path;
                        a.NameProduct = item.Name;
                        a.Quantity = item.Quantity;
                        a.Price = item.Price - (item.Price * (double)item.Discount);
                        a.SubTotal = a.Quantity * a.Price;
                        a.IDOrder = item.IDOrder;
                        a.maxQuantity = (int)item.MaxQuantity;
                        cart.Add(a);
                    }
                }
                ViewBag.Cart = cart;
                ViewBag.ORDER = db.Orders.ToList().Where(p => p.IdUser == x.IdUser && (p.Payment.Equals("CHƯA XÁC ĐỊNH") || p.Delivery.Equals("ĐANG LẬP"))).FirstOrDefault();
                return View();

            }
            else
                return View();
        }
        [HttpPost]
        public ActionResult Cart(Receipt v)
        {
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            if (ModelState.IsValid)
            {
                Order ord = db.Orders.ToList().Where(p => p.IdOrder == v.IdOrder).FirstOrDefault();
                ord.Date = v.Date;
                ord.Payment = v.Payment;
                ord.Total = v.Total;
                TempData["Order"] = ord;
                db.SaveChanges();
                return RedirectToAction("Receiver_Form", "Home");
            }
            else
            {

                if (Session["User"] != null)
                {
                    string name = Session["User"].ToString();
                    User x = db.Users.ToList().Where(p => p.Name == name).FirstOrDefault();
                    List<Cart> cart = new List<Cart>();

                    var i = (from product in db.Products
                             from image in db.Images
                             from imgdetails in db.ImageDetails
                             from order in db.Orders
                             from orddetails in db.OrderDetails
                             where (order.Payment.Equals("CHƯA XÁC ĐỊNH") || order.Delivery.Equals("ĐANG LẬP")) && x.IdUser == order.IdUser && order.IdOrder == orddetails.IdOrder && orddetails.IdProduct == product.IdProduct && product.IdProduct == imgdetails.IdProduct && imgdetails.IdImage == image.IdImage && imgdetails.ImageType.Equals("ẢNH BÌA")
                             select new { IDOrder = order.IdOrder, IDUser = x.IdUser, Name = product.NameProduct, Path = image.FileImage, Price = product.Price, IdProduct = product.IdProduct, Discount = product.Discount, Quantity = orddetails.Quantity, MaxQuantity = product.Quantity }).ToList();
                    foreach (var item in i)
                    {
                        if (item.MaxQuantity == 0)
                        {
                            OrderDetail a = db.OrderDetails.Where(p => p.IdProduct == item.IdProduct).FirstOrDefault();
                            db.OrderDetails.Remove(a);
                            
                            db.SaveChanges();
                        }
                        else
                        {
                            Cart a = new Cart();
                            a.IDUser = item.IDUser;
                            a.IDProduct = item.IdProduct;
                            a.Image = item.Path;
                            a.NameProduct = item.Name;
                            a.Quantity = item.Quantity;
                            a.Price = item.Price - (item.Price * (double)item.Discount);
                            a.SubTotal = a.Quantity * a.Price;
                            a.IDOrder = item.IDOrder;
                            a.maxQuantity = (int)item.MaxQuantity;
                            cart.Add(a);
                        }
                    }
                   
                    ViewBag.Cart = cart;
                    ViewBag.ORDER = db.Orders.ToList().Where(p => p.IdUser == x.IdUser && (p.Payment.Equals("CHƯA XÁC ĐỊNH") || p.Delivery.Equals("ĐANG LẬP"))).FirstOrDefault();
                    return View(v);
                }
                else
                    return View(v);

            }
        }
        [HttpPost]
        public ActionResult Delete(string IDOrder, string IDProduct)
        {

            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            OrderDetail x = db.OrderDetails.ToList().Where(p => p.IdOrder == IDOrder && p.IdProduct == IDProduct).FirstOrDefault();
            db.OrderDetails.Remove(x);
            db.SaveChanges();
            if (db.OrderDetails.Count(p => p.IdOrder == IDOrder) == 0)
            {
                Order deletedord = db.Orders.Where(p => p.IdOrder == IDOrder).FirstOrDefault();
                db.Orders.Remove(deletedord);
                db.SaveChanges();
            }
            return RedirectToAction("Cart", "Home");
        }
        [HttpPost]
        public ActionResult Update(string IDOrder, string IDProduct, int Quantity)
        {

            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            OrderDetail x = db.OrderDetails.ToList().Where(p => p.IdOrder == IDOrder && p.IdProduct == IDProduct).FirstOrDefault();
            Product y = db.Products.ToList().Where(p => p.IdProduct == IDProduct).FirstOrDefault();
            if (Quantity == 0)
            {
                db.OrderDetails.Remove(x);
                db.SaveChanges();
            }
            else
            {
                if (Quantity > y.Quantity)
                {
                    ViewBag.Warning = "Số lượng lớn hơn tồn kho!";
                    return RedirectToAction("Cart", "Home");
                }
                else
                {
                    x.Quantity = Quantity;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Cart", "Home");
        }
        public ActionResult Receiver_Form()
        {
            var ord = new Order();
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            if (TempData["Order"] != null)
            {
                ord = TempData["Order"] as Order;
                Session["Receiver"] = ord;
            }
            else if (Session["Receiver"] != null)
            {
                ord = Session["Receiver"] as Order;
            }
            if (ord.IdUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Receiver_Form info = new Receiver_Form();
                info.UserID = ord.IdUser;
                info.OrderID = ord.IdOrder;
                var x = (from user in db.Users
                         where user.IdUser == ord.IdUser
                         select new { user.Receiver, user.PhoneNumber, user.Address }).FirstOrDefault();

                if (x.Receiver != null)
                    info.Name = x.Receiver;
                if (x.PhoneNumber != null)
                    info.PhoneNumber = x.PhoneNumber.Trim();
                if (x.PhoneNumber != null)
                    info.Address = x.Address;
                if (x.Address != null)
                    info.Delivery = ord.Delivery;
                return View(info);
            }
        }
        [HttpPost]
        public ActionResult Receiver_Form(Receiver_Form info)
        {
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            string id = "";
            if (ModelState.IsValid)
            {
                Order order = db.Orders.Where(p => p.IdOrder == info.OrderID).FirstOrDefault();
                order.Delivery = info.Delivery;
                User user = new User();
                if (info.PhoneNumber != null && db.Users.ToList().Exists(P => P.IdUser != info.UserID && (P.PhoneNumber != null && P.PhoneNumber.Trim() == info.PhoneNumber)))
                {
                    user = db.Users.Where(P => (P.PhoneNumber != null && P.PhoneNumber.Trim() == info.PhoneNumber)).FirstOrDefault();
                    order.IdUser = user.IdUser;
                    db.SaveChanges();
                    user.Address = info.Address;
                    db.SaveChanges();

                    var y = Session["Receiver"] as Order;
                    y.IdUser = user.IdUser;
                    Session["Receiver"] = y;
                    id = Session["User"].ToString() ;
                    Session["User"] = user.Name;
                }
                else
                {
                    user = db.Users.Where(p => p.IdUser == info.UserID).FirstOrDefault();
                    user.Address = info.Address;
                    id = Session["User"].ToString();
                    user.PhoneNumber = info.PhoneNumber;
                }
                user.Receiver = info.Name;
                user.TotalSpent += order.Total;
                List<OrderDetail> details = db.OrderDetails.Where(p => p.IdOrder == order.IdOrder).ToList();
                foreach (var item in details)
                {
                    Product product = db.Products.Where(p => p.IdProduct == item.IdProduct).FirstOrDefault();
                    product.Quantity -= item.Quantity;
                }
                db.SaveChanges();
                TempData["Success"] = 1;
                if (Session["ID"] == null)
                    Session["User"] = id;
                Session["Receiver"] = null;
                return RedirectToAction("Success", "Home");
            }
            else
            {

                return View(info);
            }

        }
        public ActionResult Success()
        {
            var x = TempData["Success"];
            if (x == null)
                return RedirectToAction("Index", "Home");
            else
                return View();
        }
        public ActionResult RestorePassword()
        {
            return View();

        }
        [HttpPost]
        public ActionResult RestorePassword(RestorePassword rp)
        {
            if (ModelState.IsValid == true)
            {
                QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
                User user = DB.Users.ToList().Where(p => p.Name.Trim() == rp.Username && p.Email != null && p.Email.Trim() == rp.Email.ToString().Trim()).FirstOrDefault();
                ViewBag.Password = user.Password;
                return View();
            }
            else
                return View(rp);

        }

        //======================================================ADMIN==========================================================//
        public ActionResult Add_Product()
        {

            var i = Session["ID"];
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            if (i == null || i.ToString() == "2")
                return RedirectToAction("Error", "Home");
            else
            {
                ViewBag.ListBrand = db.Brands.ToList().OrderBy(u => u.IdBrand);
                ViewBag.ListCatalog = db.Catalogs.ToList().OrderBy(u => u.IdCatalog);
                return View();
            }
        }
        [HttpPost]
        public ActionResult Add_Product(Add_Product p)
        {
            HttpPostedFileBase FILE = Request.Files[0];
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            ViewBag.ListBrand = db.Brands.ToList().OrderBy(u=>u.IdBrand);
            ViewBag.ListCatalog = db.Catalogs.ToList().OrderBy(u => u.IdCatalog);

            if (ModelState.IsValid)
            {
                string id = "";
                if (db.Products.Count() == 0)
                    id = "PRO" + String.Format("{0:D6}", 1);
                else
                {
                    int max = db.Products.ToList().Max(i => Int32.Parse(i.IdProduct.Remove(0, 3)));
                    id = "PRO" + String.Format("{0:D6}", max + 1);
                }
                Product s = new Product();
                s.IdProduct = id;
                s.NameProduct = p.NameProduct;
                s.IdBrand = p.IdBrand;
                s.IdCatalog = p.IdCatalog;
                s.Price = double.Parse(p.Price.Replace(".", string.Empty));
                s.Quantity = p.Quantity;
                s.Discount = p.Discount;
                if (p.Description != null)
                {
                    s.Description = p.Description.Replace(System.Environment.NewLine, ",").ToString();
                    s.Description = s.Description.Remove(s.Description.ToString().LastIndexOf(","));
                }
                s.State = p.State;
                db.Products.Add(s);
                string idiamge = "";

                var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
                var extension = Path.GetExtension(FILE.FileName);
                if (db.Images.Count() == 0)
                    idiamge = "IMG" + String.Format("{0:D6}", 1);
                else
                {
                    int max = db.Images.ToList().Max(i => Int32.Parse(i.IdImage.Remove(0, 3)));
                    idiamge = "IMG" + String.Format("{0:D6}", max + 1);
                }
                string filepath = "";
                if (FILE.ContentLength > 0 && allowedExtensions.Contains(extension))
                {
                    string filename = Path.GetFileNameWithoutExtension(FILE.FileName);
                    string filestore = "";
                    do
                    {
                        filename = id + "_" + "Post";
                        filepath = Path.Combine(Server.MapPath("~/Image/Products"), filename + extension);
                        filestore = filepath.Replace("\\", "/");
                        filestore = filestore.Substring(filestore.LastIndexOf("/Image"));
                        filestore = filestore.Insert(0, "~");
                    } while (db.Images.ToList().Exists(o => o.FileImage == filestore) == true);
                    FILE.SaveAs(filepath);

                    Image i = new Image();
                    i.IdImage = idiamge;
                    i.FileImage = filestore;
                    db.Images.Add(i);
                    db.SaveChanges();

                    ImageDetail details = new ImageDetail();
                    details.IdImage = idiamge;
                    details.IdProduct = id;
                    details.ImageType = "ẢNH BÌA";
                    db.ImageDetails.Add(details);
                    db.SaveChanges();
                    int j = 0;
                    int n = Request.Files.Count;
                    for (int k = 0; k < n; k++)
                    {
                        if (k != 0)
                        {
                            filepath = filename = extension = null;
                            var file = Request.Files[k];
                            extension = Path.GetExtension(file.FileName);
                            if (allowedExtensions.Contains(extension))
                            {
                                filename = Path.GetFileNameWithoutExtension(file.FileName);
                                filename = id + "_" + "Slide" + "(" + j + ")";
                                filepath = Path.Combine(Server.MapPath("~/Image/Products"), filename + extension);
                                file.SaveAs(filepath);
                                filepath = filepath.Replace("\\", "/");
                                filepath = filepath.Substring(filepath.LastIndexOf("/Image"));
                                filepath = filepath.Insert(0, "~");
                                if (db.Images.Count() == 0)
                                    idiamge = "IMG" + String.Format("{0:D6}", 1);
                                else
                                {
                                    int max = db.Images.ToList().Max(y => Int32.Parse(y.IdImage.Remove(0, 3)));
                                    idiamge = "IMG" + String.Format("{0:D6}", max + 1);
                                }
                                i = new Image();
                                i.IdImage = idiamge;
                                i.FileImage = filepath;
                                db.Images.Add(i);
                                db.SaveChanges();

                                details = new ImageDetail();
                                details.IdImage = idiamge;
                                details.IdProduct = id;
                                details.ImageType = "ẢNH SLIDE";
                                db.ImageDetails.Add(details);
                                db.SaveChanges();
                                j++;
                            }
                        }

                    }
                }
                else
                {
                    return RedirectToAction("Add_Product", "Home");
                }

                db.SaveChanges();
                ViewBag.Anounce = "0";
            }
            else
            {
                ViewBag.Anounce = "1";
            }
            return View(p);
        }
        //public ActionResult DS_HinhAnhSanPham()
        //{
        //    var i = Session["ID"];
        //    if (i == null || i.ToString() == "2")
        //        return RedirectToAction("Error", "Home");
        //    else
        //    {
        //        QuanLyBanHangEntities db = new QuanLyBanHangEntities();
        //        List<Image> listImage = db.Images.ToList();

        //        return View(listImage);
        //    }
        //}
        private List<Image> layHinhanh(int count)
        {
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            return db.Images.OrderBy(p=>p.IdImage).Take(count).ToList();
        }
        public ActionResult DS_HinhAnhSanPham(int ? page)
        {
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
             var i = Session["ID"];
             if (i == null || i.ToString() == "2")
                 return RedirectToAction("Error", "Home");
             else
             {
                 int pageSize = 15;
                 int pageNum = (page ?? 1);
                 var HinhAnh = layHinhanh(db.Images.Count());
                 return View(HinhAnh.ToPagedList(pageNum, pageSize));
             }
        }
        public ActionResult Add_Image()
        {
            var i = Session["ID"];
            if (i == null || i.ToString() == "2")
                return RedirectToAction("Error", "Home");
            else
            {
                return View();
            }
        }
      
        [HttpPost]
        public ActionResult Add_Image(ImagePath P)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    QuanLyBanHangEntities db = new QuanLyBanHangEntities();
                    HttpPostedFileBase FILE = Request.Files[0];
                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
                    var extension = Path.GetExtension(FILE.FileName);
                    string id = "";
                    if (db.Images.Count() == 0)
                        id = "IMG" + String.Format("{0:D6}", 1);
                    else
                    {
                        int max = db.Images.ToList().Max(p => Int32.Parse(p.IdImage.Remove(0, 3)));
                        id = "IMG" + String.Format("{0:D6}", max + 1);
                    }
                    string filepath = "";
                    if (FILE.ContentLength > 0 && allowedExtensions.Contains(extension))
                    {
                        string filename = Path.GetFileName(FILE.FileName);
                        filepath = Path.Combine(Server.MapPath("~/Image/Products"), filename);
                        FILE.SaveAs(filepath);
                        filepath = filepath.Replace("\\", "/");
                        filepath = filepath.Substring(filepath.LastIndexOf("/Image"));
                        filepath = filepath.Insert(0, "~");
                        //FileInfo delete = new FileInfo(filepath);
                        //if (delete.Exists)
                        //    delete.Delete();
                        Image i = new Image();
                        i.IdImage = id;
                        i.FileImage = filepath;
                        db.Images.Add(i);
                        db.SaveChanges();
                        ViewBag.Announce = "0";
                        return View();
                    }
                    else
                    {
                        if (!allowedExtensions.Contains(extension))
                        {
                            ViewBag.Announce = "3";
                            return View();
                        }

                    }
                    return View();

                }
                catch
                {
                    ViewBag.Announce = "2";
                    return View();
                }

            }
            else
            {
                ViewBag.Announce = "1";
                return View();
            }
        }
        public ActionResult DS_Brand()
        {
            var i = Session["ID"];
            if (i == null || i.ToString() == "2")
                return RedirectToAction("Error", "Home");
            else
            {
                QuanLyBanHangEntities db = new QuanLyBanHangEntities();
                List<Brand> Brand = db.Brands.OrderBy(P => P.IdBrand).ToList();
                ViewBag.BrandList = Brand;
                return View();
            }
        }
        [HttpPost]
        public ActionResult DS_Brand(BrandName br, string feature)
        {
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            if(br.Name==null)
            {
                List<Brand> Brand = db.Brands.OrderBy(P => P.IdBrand).ToList();
                ViewBag.BrandList = Brand;
                ViewBag.Announce2 = "3";
                return View(br);
            }
           
            if (feature == "Replace")
            {
                if (ModelState.IsValid)
                {
                    Brand brand = db.Brands.ToList().Where(P => P.IdBrand == br.ID).FirstOrDefault();
                    brand.NameBrand = br.Name;
                    db.SaveChanges();
                    List<Brand> Brand = db.Brands.OrderBy(P => P.IdBrand).ToList();
                    ViewBag.BrandList = Brand;
                    ViewBag.Announce2 = "1";
                    return RedirectToAction("DS_Brand", "Home");
                }
                else
                {
                    ViewBag.Announce2 = "0";
                    List<Brand> Brand = db.Brands.OrderBy(P => P.IdBrand).ToList();
                    ViewBag.BrandList = Brand;
                    return View(br);
                }
            }
            else if(feature=="Add")
            {
                if (ModelState.IsValid)
                {
                    Brand brand = new Brand();
                    brand.IdBrand = br.ID;
                    brand.NameBrand = br.Name;
                    db.Brands.Add(brand);
                    db.SaveChanges();
                    List<Brand> Brand = db.Brands.OrderBy(P => P.IdBrand).ToList();
                    ViewBag.BrandList = Brand;
                    ViewBag.Announce2 = "1";
                    return RedirectToAction("DS_Brand", "Home");
                }
                else
                {
                    ViewBag.Announce2 = "0";
                    List<Brand> Brand = db.Brands.OrderBy(P => P.IdBrand).ToList();
                    ViewBag.BrandList = Brand;
                    return View(br);
                }
            }
            else if(feature=="Delete")
            {
                
                if(db.Products.ToList().Exists(p=>p.IdBrand==br.ID)==true)
                {
                    ViewBag.Announce2 = "2";
                    List<Brand> Brand = db.Brands.OrderBy(P => P.IdBrand).ToList();
                    ViewBag.BrandList = Brand;
                    return View(br);
                }
                else
                {
                    ViewBag.Announce2 = "1";
                    Brand brand = db.Brands.ToList().Where(p => p.IdBrand == br.ID).FirstOrDefault();
                    db.Brands.Remove(brand);
                    db.SaveChanges();
                    List<Brand> Brand = db.Brands.OrderBy(P => P.IdBrand).ToList();
                    ViewBag.BrandList = Brand;
                    return RedirectToAction("DS_Brand", "Home");
                }
            }
            else
            {
                ViewBag.Announce2 = "0";
                List<Brand> Brand = db.Brands.OrderBy(P => P.IdBrand).ToList();
                ViewBag.BrandList = Brand;
                return View(br);
            }
        }
        public ActionResult DS_Catalog()
        {
            var i = Session["ID"];
            if (i == null || i.ToString() == "2")
                return RedirectToAction("Error", "Home");
            else
            {
                QuanLyBanHangEntities db = new QuanLyBanHangEntities();
                List<Catalog> Catalog = db.Catalogs.OrderBy(P => P.IdCatalog).ToList();
                ViewBag.CatalogList = Catalog;
                return View();
            }
        }
        [HttpPost]
        public ActionResult DS_Catalog(CatalogName ca, string feature)
        {
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            if (ca.Name == null)
            {
                List<Catalog> Catalog = db.Catalogs.OrderBy(P => P.IdCatalog).ToList();
                ViewBag.CatalogList = Catalog;
                ViewBag.Announce2 = "3";
                return View(ca);
            }

            if (feature == "Replace")
            {
                if (ModelState.IsValid)
                {
                    Catalog catalog = db.Catalogs.ToList().Where(P => P.IdCatalog == ca.ID).FirstOrDefault();
                    catalog.NameCatalog = ca.Name;
                    db.SaveChanges();
                    List<Catalog> Catalog = db.Catalogs.OrderBy(P => P.IdCatalog).ToList();
                    ViewBag.CatalogList = Catalog;
                    ViewBag.Announce2 = "1";
                    return RedirectToAction("DS_Catalog", "Home");
                }
                else
                {
                    ViewBag.Announce2 = "0";
                    List<Catalog> Catalog = db.Catalogs.OrderBy(P => P.IdCatalog).ToList();
                    ViewBag.CatalogList = Catalog;
                    return View(ca);
                }
            }
            else if (feature == "Add")
            {
                if (ModelState.IsValid)
                {
                    Catalog catalog = new Catalog();
                    catalog.IdCatalog = ca.ID;
                    catalog.NameCatalog = ca.Name;
                    db.Catalogs.Add(catalog);
                    db.SaveChanges();
                    List<Catalog> Catalog = db.Catalogs.OrderBy(P => P.IdCatalog).ToList();
                    ViewBag.CatalogList = Catalog;
                    ViewBag.Announce2 = "1";
                    return RedirectToAction("DS_Catalog", "Home");
                }
                else
                {
                    ViewBag.Announce2 = "0";
                    List<Catalog> Catalog = db.Catalogs.OrderBy(P => P.IdCatalog).ToList();
                    ViewBag.CatalogList = Catalog;
                    return View(ca);
                }
            }
            else if (feature == "Delete")
            {

                if (db.Products.ToList().Exists(p => p.IdCatalog == ca.ID) == true)
                {
                    ViewBag.Announce2 = "2";
                    List<Catalog> Catalog = db.Catalogs.OrderBy(P => P.IdCatalog).ToList();
                    ViewBag.CatalogList = Catalog;
                    return View(ca);
                }
                else
                {
                    ViewBag.Announce2 = "1";
                    Catalog catalog = db.Catalogs.ToList().Where(p => p.IdCatalog == ca.ID).FirstOrDefault();
                    db.Catalogs.Remove(catalog);
                    db.SaveChanges();
                    List<Catalog> Catalog = db.Catalogs.OrderBy(P => P.IdCatalog).ToList();
                    ViewBag.CatalogList = Catalog;
                    return RedirectToAction("DS_Catalog", "Home");
                }
            }
            else
            {
                ViewBag.Announce2 = "0";
                List<Catalog> Catalog = db.Catalogs.OrderBy(P => P.IdCatalog).ToList();
                ViewBag.CatalogList = Catalog;
                return View(ca);
            }
        }
        public ActionResult QL_HoaDon()
        {
             var i = Session["ID"];
             if (i == null || i.ToString() == "2")
                 return RedirectToAction("Error", "Home");
             else
             {
                 var x = TempData["Anounce"];
                 QuanLyBanHangEntities db = new QuanLyBanHangEntities();
                 if (TempData["Sort"] == null)
                 {
                     ViewBag.Anounce = x;
                     return View(db.Orders.ToList());
                 }
                 else
                 {
                     ViewBag.Anounce = x;
                     ViewBag.Selected = TempData["Select"].ToString();
                     List<Order> list = TempData["Sort"] as List<BTLONLTWEB.Models.Order>;
                     return View(list);
                 }
             }
        }
        
        [HttpPost]
        public ActionResult Sort (string sort)
        {
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            switch(sort)
            {
                case "TỔNG TIỀN TĂNG DẦN":
                    TempData["Select"] = sort;
                    TempData["Sort"] = db.Orders.ToList().OrderBy(p => p.Total).ToList();
                    return RedirectToAction("QL_HoaDon", "Home");
                case "TỔNG TIỀN GIẢM DẦN":
                    TempData["Select"] = sort;
                    TempData["Sort"] = db.Orders.ToList().OrderByDescending(p => p.Total).ToList();
                    return RedirectToAction("QL_HoaDon", "Home");
                case "NGÀY TĂNG DẦN":
                    TempData["Select"] = sort;
                    TempData["Sort"] = db.Orders.ToList().OrderBy(p => p.Date).ToList();
                    return RedirectToAction("QL_HoaDon", "Home");
                case "NGÀY GIẢM DẦN":
                    TempData["Select"] = sort;
                    TempData["Sort"] = db.Orders.ToList().OrderByDescending(p => p.Date).ToList();
                    return RedirectToAction("QL_HoaDon", "Home");
                case "CHƯA XÁC MINH":
                    TempData["Select"] = sort;
                    TempData["Sort"] = db.Orders.ToList().Where(p => p.Payment == "CHƯA XÁC ĐỊNH" || p.Delivery == "ĐANG LẬP").ToList();
                    return RedirectToAction("QL_HoaDon", "Home");
                default:
                    return RedirectToAction("QL_HoaDon", "Home");
            }
        }
        public ActionResult DeleteOrder(string id)
        {
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            if (db.Orders.ToList().Exists(p => p.IdOrder.Trim() == id) == true)
            {
                Order order=db.Orders.Where(p=>p.IdOrder.Trim()==id.Trim()).FirstOrDefault();
                List<OrderDetail> listProducts = db.OrderDetails.Where(p => p.IdOrder.Trim() == id.Trim()).ToList();
                foreach (var item in listProducts)
                {
                    db.OrderDetails.Remove(item);
                    db.SaveChanges();
                    
                    
                }
                TempData["Anounce"] = "0";
                db.Orders.Remove(order);
                db.SaveChanges();
                return RedirectToAction("QL_HoaDon", "Home");
            }
            else
            {
                TempData["Anounce"] = "1";
                return RedirectToAction("QL_HoaDon", "Home");
            }
            
        }
        public ActionResult ChiTietHoaDon(string id)
        {
             var i = Session["ID"];
             if (i == null || i.ToString() == "2")
                 return RedirectToAction("Error", "Home");
             else
             {
                 if (id == null)
                     return RedirectToAction("Error", "Home");
                 else
                 {
                     QuanLyBanHangEntities db = new QuanLyBanHangEntities();
                     var ord = (from user in db.Users
                                from order in db.Orders
                                where order.IdOrder.Trim() == id.Trim() &&order.IdUser==user.IdUser
                                select new
                                {
                                    IDUser = user.IdUser,
                                    IDOrder = order.IdOrder,
                                    Name = user.Receiver,
                                    PhoneNumber = user.PhoneNumber,
                                    Address = user.Address,
                                    Date = order.Date,
                                    Total = order.Total,
                                    Payment = order.Payment,
                                    Delivery = order.Delivery
                                }).FirstOrDefault();
                     Details det = new Details(ord.IDUser, ord.Name, ord.PhoneNumber, ord.Address, ord.IDOrder, ord.Payment, ord.Delivery, (double)ord.Total, ord.Date);
                     var prod=(from product in db.Products
                                from order in db.Orders
                                from details in db.OrderDetails
                                where order.IdOrder.Trim() == id.Trim() &&
                                      order.IdOrder.Trim() ==details.IdOrder.Trim() &&
                                      details.IdProduct==product.IdProduct
                                select new
                                {
                                    IDProduct = product.IdProduct,
                                    NameProduct = product.NameProduct,
                                    Price = product.Price-(product.Price * product.Discount),
                                    Quantity=details.Quantity,
                                    SubTotal = (product.Price - (product.Price * product.Discount)) * details.Quantity
                                }).ToList();
                     int TotalItem = 0;
                     List<ProductDetails> listprod=new List<ProductDetails>();
                     foreach(var item in prod)
                     {
                         ProductDetails product = new ProductDetails(item.IDProduct, item.NameProduct, (double)item.Price, item.Quantity, (double)item.SubTotal);
                         listprod.Add(product);
                         TotalItem += item.Quantity;
                     }
                     ViewBag.TotalItem = TotalItem;
                     ViewBag.ListProduct = listprod;
                     return View(det);
                 }
             }
        }
        
        public ActionResult UserDetails(string id)
        {
             var i = Session["ID"];
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            if (i == null || i.ToString() == "2")
                return RedirectToAction("Error", "Home");
            else
            {
                User us = db.Users.Where(p => p.IdUser.ToLower().Trim() == id.Trim().ToLower()).FirstOrDefault();
                if(us==null|| us.TypeUser.Trim()=="Admin")
                    return RedirectToAction("Error", "Home");
                List<Order> orders = db.Orders.Where(P => P.IdUser == us.IdUser && P.Delivery!="ĐANG LẬP" &&P.Payment!="CHƯA XÁC ĐỊNH" ).ToList();
                ViewBag.Orders = orders;
                int slsp = 0;
                if(orders.Count!=0)
                {
                    
                    foreach(var item in orders)
                    {
                        int sl = db.OrderDetails.Where(P => P.IdOrder == item.IdOrder).Sum(p => p.Quantity);
                        slsp += sl;
                    }
                }
                ViewBag.TotalItems = slsp;
                return View(us);
            }
        }
        public ActionResult UserManagement()
        {
            var i = Session["ID"];
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            if (i == null || i.ToString() == "2")
                return RedirectToAction("Error", "Home");
            else
            {
                ViewBag.ListUsers = db.Users.ToList();
                return View();
            }
        }
        [HttpGet]
        public ActionResult UserManagement(string search)
        {
            var i = Session["ID"];
            QuanLyBanHangEntities db = new QuanLyBanHangEntities();
            if (i == null || i.ToString() == "2")
                return RedirectToAction("Error", "Home");
            else
            {
                List<User> listusers = db.Users.Where(p => p.IdUser.ToLower().Trim().Contains(search.ToLower().Trim()) ||
                                                      p.Name.ToLower().Trim().Contains(search.ToLower().Trim()) ||
                                                      (p.Email != null && p.Email.ToLower().Trim().Contains(search.ToLower().Trim())) ||
                                                      (p.PhoneNumber != null && p.PhoneNumber.ToLower().Trim().Contains(search.ToLower().Trim()))).ToList();
                if (search ==null)
                    ViewBag.ListUsers = db.Users.ToList();
                else
                    ViewBag.ListUsers = listusers;
                return View();
            }
        }
        public ActionResult DeleteUser(string id)
        {
            var i = Session["ID"];
            if (i == null || i.ToString() == "2")
                return RedirectToAction("Error", "Home");
            if (id == null)
                return RedirectToAction("Error", "Home");
            else
            {
                QuanLyBanHangEntities db = new QuanLyBanHangEntities();
                User user = db.Users.Where(p => p.IdUser.Trim() == id).FirstOrDefault();
                if(user==null || user.TypeUser.Trim()=="Admin")
                    return RedirectToAction("Error", "Home");
                else
                {
                    string fullpath = Request.MapPath(user.IdAvatar);
                    string filename=Path.GetFileName(fullpath);
                    if (System.IO.File.Exists(fullpath) && filename != "Male.jpg" && filename != "Female.jpg" && filename!="Undefined.png")
                        System.IO.File.Delete(fullpath);
                    List<Order> orders = db.Orders.Where(p => p.IdUser == user.IdUser).ToList();
                    foreach(var item in orders)
                    {
                        List<OrderDetail> details = db.OrderDetails.Where(P => P.IdOrder == item.IdOrder).ToList();
                        foreach(var detail in details)
                        {
                            db.OrderDetails.Remove(detail);
                            db.SaveChanges();
                        }
                        if(db.OrderDetails.Count(p=>p.IdOrder==item.IdOrder)==0)
                        {
                            db.Orders.Remove(item);
                            db.SaveChanges();
                        }
                    }
                    db.Users.Remove(user);
                    db.SaveChanges();
                    return RedirectToAction("UserManagement", "Home");
                }
            }
        }
        public ActionResult BanUser(string id)
        {
            var i = Session["ID"];
            if (i == null || i.ToString() == "2")
                return RedirectToAction("Error", "Home");
            if (id == null)
                return RedirectToAction("Error", "Home");
            else
            {
                QuanLyBanHangEntities db = new QuanLyBanHangEntities();
                User user = db.Users.Where(p => p.IdUser.Trim() == id).FirstOrDefault();
                if (user == null || user.TypeUser.Trim() == "Admin")
                    return RedirectToAction("Error", "Home");
                else
                {
                    if (user.Member == "BANNED")
                    {
                        user.Member = "ĐỒNG";
                        db.SaveChanges();
                        return RedirectToAction("UserManagement", "Home");
                    }
                    else
                    {
                        user.Member = "BANNED";
                        db.SaveChanges();
                        return RedirectToAction("UserManagement", "Home");
                    }
                }
            }
        }
    }
}