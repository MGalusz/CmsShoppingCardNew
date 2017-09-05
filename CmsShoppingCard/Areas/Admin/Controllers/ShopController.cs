using CmsShoppingCard.Models.Date;
using CmsShoppingCard.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CmsShoppingCard.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories/
        public ActionResult Categories()
        {
            //Declare a list of Models
            List<CategoryVm> calegotyVmList;

            using (Db db = new Db())
            {
                //Init the List
                calegotyVmList = db.Categories
                                    .ToArray()
                                    .OrderBy(x => x.Sorting)
                                    .Select(x => new CategoryVm(x))
                                    .ToList();
            }

            return View(calegotyVmList);
        }

        // POST: Admin/Shop/AddNewCategory/Id
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Declere Id
            string id;

            using (Db db = new Db())
            {
                //Check that catogory nasme in unnique
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                //Init DTO
                CategoryDTO dto = new CategoryDTO();

                //Add to DTO
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;


                //Save DTO
                db.Categories.Add(dto);
                db.SaveChanges();
                //Get the id
                id = dto.Id.ToString();
            }
            //Rerutn id
            return id;
        }


        // POST: Admin/Shop/ReorderCategories/id
        [HttpPost]
        public void ReorderCategories(int[] id)
        {

            using (Db db = new Db())
            {

                //Set initial count
                int count = 1;

                //Declare PageDTO
                CategoryDTO dto;

                //Set sourting for each page
                foreach (var categoriId in id)
                {
                    dto = db.Categories.Find(categoriId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;

                }
            }

        }
        // Get: Admin/Pages/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                //Get the category

                CategoryDTO dto = db.Categories.Find(id);

                //Remove the category
                db.Categories.Remove(dto);
                //Save 
                db.SaveChanges();
            }

            //Redirect
            return RedirectToAction("Categories");
        }


        // POST: Admin/Shop/RenameCategory/newCatName,id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                //Check category name is unique
                if (db.Categories.Any(x => x.Name == newCatName))
                {
                    return "titletaken";
                }

                //Get DTO

                CategoryDTO dto = db.Categories.Find(id);

                //Edit DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                //Save
                db.SaveChanges();

            }
            //Return
            return "ok";

        }

        // GET: Admin/Shop/AddProduct/
        [HttpGet]
        public ActionResult AddProduct()
        {
            // init model

            ProductsVM model = new ProductsVM();

            //Add select list od categories to model
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }

            //Return view with model

            return View(model);
        }

        // POEST: Admin/Shop/AddProduct/
        [HttpPost]
        public ActionResult AddProducts(ProductsVM model, HttpPostedFileBase file)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
                    return View(model);
                }


            }

            // Make sure product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(model);
                }
            }

            //Declate Produt id
            int id;

            // Init and save productDTO
            using (Db db = new Db())
            {
                ProdutcDTO product = new ProdutcDTO();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoruName = catDTO.Name;
                db.Products.Add(product);
                db.SaveChanges();
                // Get insert id
                id = product.Id;
            }

            //Set TempData message
            TempData["SM"] = "You have added a product!";

            #region Upload Image
            // Create necessary directories 
            var orginalDirectory = new DirectoryInfo(string.Format("{0}Image\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(orginalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");


            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);
            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);
            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);
            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);
            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            // Check if a file was uploades

            if (file != null && file.ContentLength > 0)
            {


                //Get file extension
                string ext = file.ContentType.ToLower();

                //Verify extnsion
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {

                        model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
                        ModelState.AddModelError("", "That image was not uploaded - wrong image extension!");
                        return View(model);
                    }

                }


                // initial image name
                string imageName = file.FileName;

                //save image name to DTO
                using (Db db = new Db())
                {
                    ProdutcDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();

                }

                    //Set orginal and thumb image path
                    var path1 =string.Format("{0}\\{1}", pathString2, imageName);
                  var path2 = string.Format("{0}\\{1}", pathString3, imageName);
                //Save orginal image
                file.SaveAs(path1);

                //Create and save thumb

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }
            #endregion

            //Redirect
            return RedirectToAction("AddProduct");
        }
    }
}