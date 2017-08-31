using CmsShoppingCard.Models.Date;
using CmsShoppingCard.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCard.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // Declare list of pabe VM
            List<PageVM> pagesList;


            using (Db db = new Db())
            {
                //Init the list
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //Return view with yhe list
            return View(pagesList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Check model state

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Declate slug
                string slug;

                //Init pageDTO
                PageDTO dto = new PageDTO();

                //DTO title
                dto.Title = model.Title;

                // Check for and set slug id need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // Make sure title and slug are unique
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "The Title or slug allredy exists");
                    return View(model);
                }


                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;
                dto.Sorting = 100;


                //save DTO
                db.Pages.Add(dto);
                db.SaveChanges();

            }
            // set TempData message
            TempData["SM"] = "You have added a new page";

            //Redirect

            return RedirectToAction("AddPage");
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {

            //Declare Page VM
            PageVM model;

            using (Db db = new Db())
            {

                //Get teh page

                PageDTO dto = db.Pages.Find(id);

                //Confirm page exists
                if (dto == null)
                {
                    return Content("The page does not exist");
                }
                //Init page VM

                model = new PageVM(dto);

            }
            //Return view with model
            return View(model);
        }
        // Post: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //Get page id
                int id = model.Id;
                //Declare slug

                string slug = "home";


                //Get The page

                PageDTO dto = db.Pages.Find(id);

                //DTO the title

                dto.Title = model.Title;

                //Check for slug and set if need be

                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //Make sure title and slug are unique
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                   db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug allredy exists");
                    return View(model);
                }


                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.Sorting = 100;
                dto.HasSideBar = model.HasSideBar;

                ///Save the DTO
                db.SaveChanges();
            }

            // set TempData message

            TempData["SM"] = "You have edited the page ";

            //Redirect



            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/DetailsPage/id
        public ActionResult DetailsPage(int id)
        {
            //Declare PageVM

            PageVM model;

            using (Db db = new Db())
            {
                //Get the page
                PageDTO dto = db.Pages.Find(id);
                //Confirm page exists
                if (dto == null)
                {
                    return Content("The page does not exists");
                }
                //Init PageVM
                model = new PageVM(dto);
            }
            //Return view with model
            return View(model);
        }


        // POST: Admin/Pages/DetailsPage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                //Get the page

                PageDTO dto = db.Pages.Find(id);

                //Remove the page
                db.Pages.Remove(dto);
                //Save 
                db.SaveChanges();
            }

            //Redirect
            return RedirectToAction("Index");
        }
        [HttpPost]
        public void ReorderPages(int[] id)
        {

            using (Db db = new Db())
            {

                //Set initial count
                int count = 1;

                //Declare PageDTO
                PageDTO dto;

                //Set sourting for each page
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;

                }
            }

        }

        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {

            // Declare the model
            SideBarVM model;

            using (Db db = new Db())
            {
                //get the DTO

                SideBarDTO dto = db.SideBar.Find(1);

                //Init model

                model = new SideBarVM(dto);
            }
            // Return View with model
            return View(model);
        }

        // POST: Admin/Pages/EditSidebar/
        [HttpPost]
        public ActionResult EditSidebar(SideBarVM model)
        {

            using (Db db = new Db())
            {
                //Get the DTO
                SideBarDTO dto = db.SideBar.Find(1);

                //DTO the body
                dto.Body = model.Body;
                //Save
                db.SaveChanges();
            }
            //Set TempData message
            TempData["SM"] = "You have edit the sidebar!";

            //Redirect
            return RedirectToAction("EditSidebar");
        }
    }



}