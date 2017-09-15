using CmsShoppingCard.Models.Date;
using CmsShoppingCard.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCard.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            //Init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>(); 

            //Check if cart is empty
            if (cart.Count ==0 || Session["cart"] == null)
            {
                ViewBag.Message = " Your cart is empty.";
                return View();
            }

            //Calculate total and save Viewbag
            decimal total = 0;
            
            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.Grandtoral = total;


            //Return viw with model

            return View(cart);
        }

        public ActionResult CartPartial()
        {
            //Init CartVM
            CartVM model = new CartVM();

            //Init quantity
            int qty = 0;

            //Init price
            decimal price = 0m;

            //Check for cart session
            if(Session["cart"] !=null)
            {
                //Get total qty and price
                var list =(List<CartVM>)Session["cart"];

                foreach ( var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }
            }
            else
            {
                //or set qty and price to 0
                model.Quantity = 0;
                model.Price = 0m;

            }



            //Return partial view with model

            return PartialView(model);


        }


        public ActionResult AddToCartPartial(int id)
        {
            //init CarCM list

            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>() ;

            //Init CartVM
            CartVM model = new CartVM();
            
            using( Db db = new Db())
            {
                //Get the product

                ProdutcDTO produtc = db.Products.Find(id);

                //Check if the product is already in cart
                var productInCar = cart.FirstOrDefault(x => x.ProductId == id);

                //if not, add new
                if (productInCar == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = produtc.Id,
                        ProductName = produtc.Name,
                        Quantity = 1,
                        Price = produtc.Price,
                        Image = produtc.ImageName

                    });
                }
                else
                {
                    // if is is, increment
                    productInCar.Quantity++;
                }
            }

            //Get total qty and price and add to model
            int qty = 0;
            decimal price = 0m;

            foreach ( var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            //Sace cart back to session
            Session["cart"] = cart;

            //Return the parcian view with model

            return PartialView(model);
        }

    }
}