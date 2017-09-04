using CmsShoppingCard.Models.Date;
using CmsShoppingCard.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}