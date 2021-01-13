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
    public class MenuItemsController : Controller
    {
        // GET: MenuItems
        public ActionResult Index()
        {
            var commonResponse = new MenuItemsRepository().GetAllMenuItems("1");

            if (commonResponse.ResponseCode == (int)ResponseCode.Success && commonResponse.ResponseData != null)
            {
                return View(commonResponse.ResponseData);
            }
            return View();
        }

        // GET: MenuItems/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MenuItems/Create
        public ActionResult Create()
        {
            MenuItemViewModel collection = new MenuItemViewModel();
            var commonResponse = new MenuCategoryRepository().GetAllMenuCategoryForDropdown("1");

            if (commonResponse.ResponseCode == (int)ResponseCode.Success && commonResponse.ResponseData != null)
            {
                collection.CategoryList = commonResponse.ResponseData as List<MenuCategory>;
                return View(collection);
            }

            return View(collection);
        }

        // POST: MenuItems/Create
        [HttpPost]
        public ActionResult Create(MenuItemViewModel collection)
        {
            var commonResponse = new CommonResponse();
            collection.IID = "1";
            try
            {
              

                string name = DateTime.Now.Ticks + SecurityUtility.RandomString(6) + ".jpg";

                if (collection.File != null)
                {
                    string path = System.Web.HttpContext.Current.Server.MapPath("~/ImageStorage/MenuItem");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    path = System.IO.Path.Combine(path, name);
                    collection.File.SaveAs(path);
                    collection.Image = name;



                    commonResponse = new MenuItemsRepository().AddMenuItems(collection);
                    if (commonResponse.ResponseCode == (int)ResponseCode.Success)
                    {
                        TempData["SuccessMessage"] = commonResponse.ResponseUserMsg;
                        ModelState.Clear();
                        return RedirectToAction("Index");

                    }
                    else
                    {

                        string fullPath = Request.MapPath("~/ImageStorage/MenuItem/" + name);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }


                        commonResponse = new MenuCategoryRepository().GetAllMenuCategoryForDropdown("1");
                        if (commonResponse.ResponseCode == (int)ResponseCode.Success && commonResponse.ResponseData != null)
                        {
                            collection.CategoryList = commonResponse.ResponseData as List<MenuCategory>;
                        }

                        TempData["ErrorMessage"] = commonResponse.ResponseUserMsg;
                        return View(collection);
                    }
                }
                else
                {
                    commonResponse = new MenuCategoryRepository().GetAllMenuCategoryForDropdown("1");
                    if (commonResponse.ResponseCode == (int)ResponseCode.Success && commonResponse.ResponseData != null)
                    {
                        collection.CategoryList = commonResponse.ResponseData as List<MenuCategory>;
                    }
                    TempData["ErrorMessage"] = "Please provide right input.";
                    return View(collection);
                }
            }
            catch (Exception ex)
            {
                commonResponse = new MenuCategoryRepository().GetAllMenuCategoryForDropdown("1");
                if (commonResponse.ResponseCode == (int)ResponseCode.Success && commonResponse.ResponseData != null)
                {
                    collection.CategoryList = commonResponse.ResponseData as List<MenuCategory>;
                }
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return View(collection);
            }
        }

        // GET: MenuItems/Edit/5
        public ActionResult Edit(string id)
        {
            var commonResponse = new MenuItemsRepository().GetMenuItemsById(id);
            var c = new MenuCategoryRepository().GetAllMenuCategoryForDropdown("1");
            if (commonResponse.ResponseCode == (int)ResponseCode.Success && commonResponse.ResponseData != null)
            {
                MenuItemViewModel menuItemView= commonResponse.ResponseData as MenuItemViewModel;
                menuItemView.CategoryList = c.ResponseData as List<MenuCategory>;
                return View(menuItemView);
            }
            return View();
        }

        // POST: MenuItems/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, MenuItemViewModel collection)
        {
            collection.Id = id;

            if (collection.File != null)
            {
                string name = DateTime.Now.Ticks + SecurityUtility.RandomString(6) + ".jpg";
                string path = System.Web.HttpContext.Current.Server.MapPath("~/ImageStorage/MenuItem");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                path = System.IO.Path.Combine(path, name);
                collection.File.SaveAs(path);
                collection.Image = name;
            }


            CommonResponse commonResponse = new MenuItemsRepository().UpdateMenuItems(collection);

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

        // GET: MenuItems/Delete/5
        public ActionResult Delete(string id)
        {
            return PartialView("_DeleteMenuItems", new MenuItemViewModel() { Id = id });
        }

        // POST: MenuItems/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, MenuItemViewModel collection)
        {
            CommonResponse commonResponse = new MenuItemsRepository().DeleteMenuItems(id);

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
