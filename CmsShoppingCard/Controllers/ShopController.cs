using CmsShoppingCard.Models.Date;
using CmsShoppingCard.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCard.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            //Declare list of CategoryVM
            List<CategoryVm> categoryVMList;
            //Init the list
            using(Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVm(x)).ToList();
            }

            //Return partial view with List
            return PartialView(categoryVMList);
        }

        // GET: Shop/Category/name
        public ActionResult Category (string name)
        {
            //Declate a list of ProductVM
            List<ProductsVM> ProductVMLiest;
            
            using( Db db = new Db())
            {

                //Get category id
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;

                //Init the list
                ProductVMLiest = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductsVM(x)).ToList();

                //Get category name
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                ViewBag.CategoryName = productCat.CategoryName;
            }

            //Return view with list

            return View(ProductVMLiest);
        }

        // GET: Shop/ProductDetails/name
        [ActionName("product-details")]
        public ActionResult ProductDetails (string name)
        {
            //Declare the Vm and DTO
            ProductsVM model;
            ProdutcDTO dto;

            //Init product id
            int id = 0;
            using (Db db = new Db())
            {
                //Check if producy exists
                if (!db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                //init productDTO
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                //Get  id
                id = dto.Id;

                //Init model
                model = new ProductsVM(dto);
            }
            //Get gallery images
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                .Select(fn => Path.GetFileName(fn));

            //Return view with model
            return View("ProductDetails", model);
        }
    }
}