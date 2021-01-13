using GlobalEntities.Entities;
using GlobalEntities.Methods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Repository;
using static GlobalEntities.Enums.GlobalEnums;

namespace WebApplication.Controllers
{
    public class MenuCategoryController : Controller
    {
        // GET: MenuCategory
        public ActionResult Index()
        {
            var commonResponse = new MenuCategoryRepository().GetAllMenuCategory("1");

            if (commonResponse.ResponseCode == (int)ResponseCode.Success && commonResponse.ResponseData != null)
            {
                 return View(commonResponse.ResponseData);
            }
            return View();
        }

        // GET: MenuCategory/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MenuCategoryViewModel collection)
        {
            collection.IID = "1";

            try
            {
                if (ModelState.IsValid)
                {

                    string name = DateTime.Now.Ticks+ SecurityUtility.RandomString(6) + ".jpg";

                    if (collection.File != null)
                    {
                        string path = System.Web.HttpContext.Current.Server.MapPath("~/ImageStorage/MenuCategory");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        path = System.IO.Path.Combine(path, name);
                        collection.File.SaveAs(path);
                        collection.Image = name;


                        CommonResponse commonResponse = new MenuCategoryRepository().AddMenuCategory(collection);
                        if (commonResponse.ResponseCode == (int)ResponseCode.Success)
                        {
                            TempData["SuccessMessage"] = commonResponse.ResponseUserMsg;
                            ModelState.Clear();
                            return View();

                        }
                        else
                        {

                            string fullPath = Request.MapPath("~/ImageStorage/MenuCategory/" + name);
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }

                            TempData["ErrorMessage"] = commonResponse.ResponseUserMsg;
                            return View(collection);
                        }
                    }
                }
                TempData["ErrorMessage"] = "Please provide right input.";
                return View(collection);
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = "Error: "+ex.Message;
                return View(collection);
            }
        }

        // GET: MenuCategory/Edit/5
        public ActionResult Edit(string id)
        {
            var commonResponse = new MenuCategoryRepository().GetAllMenuCategoryById(id);

            if (commonResponse.ResponseCode == (int)ResponseCode.Success && commonResponse.ResponseData != null)
            {
                return PartialView("_EditMenuCategory", commonResponse.ResponseData);
            }
            return PartialView("_EditEmergencyPartial");
        }

        // POST: MenuCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, MenuCategoryViewModel collection)
        {
            collection.Id = id;

            if (collection.File != null)
            {
                string name = DateTime.Now.Ticks + SecurityUtility.RandomString(6) + ".jpg";
                string path = System.Web.HttpContext.Current.Server.MapPath("~/ImageStorage/MenuCategory");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                path = System.IO.Path.Combine(path, name);
                collection.File.SaveAs(path);
                collection.Image = name;
            }


            CommonResponse commonResponse = new MenuCategoryRepository().UpdateMenuCategory(collection);

            if (commonResponse.ResponseCode == (int)ResponseCode.Success)
            {
                TempData["SuccessMessage"] = commonResponse.ResponseMsg;
            }
            else
            {
                TempData["ErrorMessage"] = commonResponse.ResponseMsg;
            }
            return RedirectToAction("Index");
        }

        // GET: MenuCategory/Delete/5
        public ActionResult Delete(string id)
        {
            return PartialView("_DeleteMenuCategory", new MenuCategoryViewModel() { Id = id });
        }

        // POST: MenuCategory/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, MenuCategoryViewModel collection)
        {
            CommonResponse commonResponse = new MenuCategoryRepository().DeleteMenuCategory(id);

            if (commonResponse.ResponseCode == (int)ResponseCode.Success)
            {
                TempData["SuccessMessage"] = commonResponse.ResponseMsg;
            }
            else
            {
                TempData["ErrorMessage"] = commonResponse.ResponseMsg;
            }
            return RedirectToAction("Index");
        }
    }
}
