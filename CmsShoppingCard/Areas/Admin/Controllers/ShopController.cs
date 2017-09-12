using CmsShoppingCard.Models.Date;
using CmsShoppingCard.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;

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

        // POStT: Admin/Shop/AddProduct/
        [HttpPost]
        public ActionResult AddProduct(ProductsVM model, HttpPostedFileBase file)
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
                product.CategoryName = catDTO.Name;
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
                var path1 = string.Format("{0}\\{1}", pathString2, imageName);
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

        // GET: Admin/Shop/Products/
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            //Declare a list of ProductVM
            List<ProductsVM> listOfProductVM;

            // Set page number

            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {

                //Init the list

                listOfProductVM = db.Products.ToArray()
                                  .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                                  .Select(x => new ProductsVM(x))
                                  .ToList();

                //Populate categories select list
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");


                //Set selected category
                ViewBag.SelectedCat = catId.ToString();


            }

            //Set pagination
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
            ViewBag.OnePageOfProducts = onePageOfProducts;

            //Return view with list

            return View(listOfProductVM);
        }

        // GET: Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            //Declate the productVM
            ProductsVM model;

            using (Db db = new Db())
            {

                //Get the product

                ProdutcDTO dto = db.Products.Find(id);

                //Make sure product exists
                if (dto == null)
                {
                    return Content("That product does not exists");
                }

                // init model

                model = new ProductsVM(dto);

                // make a select lest
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                //Get all allety images
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Image/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));

            }
            //Return View with the model
            return View(model);
        }
        // POST: Admin/Shop/EditProduct/id
        [HttpPost]
        public ActionResult EditProduct(ProductsVM model, HttpPostedFileBase file)
        {
            //Get a product id
            int id = model.Id;

            //Populate categorie select list and gallery images
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Image/Uploads/Products/" + id + "/Gallery/Thumbs"))
            .Select(fn => Path.GetFileName(fn));

            //Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Make sure name is unique
            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(model);
                }
            }
            //Update product
            using (Db db = new Db())
            {
                ProdutcDTO dto = db.Products.Find(id);
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }

            //set TempData message
            TempData["SM"] = "You have edited the product";

            #region image Upload

            // Check for file upload
            if (file != null && file.ContentLength > 0)
            {

                // Get extension
                string ext = file.ContentType.ToLower();

                // Verify extension
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
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension.");
                        return View(model);
                    }
                }

                // Set uplpad directory paths
                var orginalDirectory = new DirectoryInfo(string.Format("{0}Image\\Uploads", Server.MapPath(@"\")));

                var pathString1 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                // Delete files from directories

                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in di1.GetFiles())
                    file2.Delete();

                foreach (FileInfo file3 in di2.GetFiles())
                    file3.Delete();

                // Save image name

                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    ProdutcDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Save original and thumb images

                var path1 = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                file.SaveAs(path1);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }


            #endregion

            //Redirect
            return RedirectToAction("EditProduct");
        }

        // GET: Admin/Shop/DeleteProduct/id
        public ActionResult DeleteProduct(int id)
        {
            //Delete product from DB
            using (Db db = new Db())
            {
                ProdutcDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);

                db.SaveChanges();
            }

            //Felete product folder

            var orginalDirectory = new DirectoryInfo(string.Format("{0}Image\\Uploads", Server.MapPath(@"\")));

            string pathString = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            //Redirect

            return RedirectToAction("Products");

        }

        // POST: Admin/Shop/SaveGalleryImages/
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            //loop throuhg files
            foreach (string fileName in Request.Files)
            {

                //Init the files

                HttpPostedFileBase file = Request.Files[fileName];

                //Check is's not null
                if(file != null && file.ContentLength > 0)
                {

                    //set directory paths

                    var orginalDirectory = new DirectoryInfo(string.Format("{0}Image\\Uploads", Server.MapPath(@"\")));

                    string pathString1 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(orginalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                    //Set image paths

                    var path = string.Format("{0}\\{1}", pathString1, fileName);
                    var path2 = string.Format("{0}\\{1}", pathString2, fileName);

                    //Save orginal and thumb

                    file.SaveAs(path);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200);
                    img.Save(path2);


                }
            }
        }
    }
}