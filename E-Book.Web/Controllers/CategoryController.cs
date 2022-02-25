using E_Book.DataAccess;
using E_Book.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Book.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> ObjCategoryList = _db.Categories;
            return View(ObjCategoryList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display Order Can't Be Same As Name");
            }
            if (ModelState.IsValid)
            {
                _db.Add(obj);
                _db.SaveChanges();
                TempData["sucess"] = "Category Added Sucessfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            
            var CategoryidFromDB = _db.Categories.Find(id);
            
            if (CategoryidFromDB == null)
            {
                return NotFound();
            }
            return View(CategoryidFromDB);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display Order Can't Be Same As Name");
            }
            if (ModelState.IsValid)
            {
                _db.Update(obj);
                _db.SaveChanges();
                TempData["sucess"] = "Category Updated Sucessfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var CategoryidFromDB = _db.Categories.Find(id);

            if (CategoryidFromDB == null)
            {
                return NotFound();
            }
            return View(CategoryidFromDB);
        }

        //POST
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int?id)
        {
            var obj = _db.Categories.Find(id);
            if(obj==null)
            {
                return NotFound();
            }
            _db.Remove(obj);
            _db.SaveChanges();
            TempData["sucess"] = "Category Deleted Sucessfully";
            return RedirectToAction("Index");


        }
    }
}
