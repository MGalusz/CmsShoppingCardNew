﻿using CmsShoppingCard.Models.Date;
using CmsShoppingCard.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCard.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page  = "")
        {
            //Get/set page slug
            if (page == "")
                page = "home";

            //Declare model and DTO
            PageVM model;
            PageDTO dto;

            //Check if page exist
            using (Db db = new Db())
            {
                if ( ! db.Pages.Any(x=>x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }

            //Get page DTO
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            //Set page title

            ViewBag.PageTitle = dto.Title;

                //Check for sidebar
                if(dto.HasSideBar == true)
            {
                ViewBag.SideBar = "Yes";
            }
            else
            {
                ViewBag.SideBar = "No";
            }

            //Init model
            model = new PageVM(dto);

                //Return view with model
                return View(model);
        }


        public ActionResult PagesMenuPartial()
        {
            // Declare a list of PageVM
            List<PageVM> pageVMList;

            // Get all pages except home
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();
            }
            // Return partial view with list
            return PartialView(pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            //Declare model
            SideBarVM model;

            //Init model
            using( Db db = new Db())
            {
                SideBarDTO dto = db.SideBar.Find(1);
                model = new SideBarVM(dto);
            }

            //Return partial view with model

            return PartialView(model);
        }

    }
}