using E_Book.DataAccess;
using E_Book.DataAccess.Repository.IRepository;
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
        private readonly IUnitOfWork _unitofwork;
        public CategoryController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> ObjCategoryList = _unitofwork.Category.GetAll();
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
                _unitofwork.Category.Add(obj);
                _unitofwork.Save();
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

            var CategoryidFromDB = _unitofwork.Category.GetFirstorDeafult(x=> x.Id==id);
            
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
                _unitofwork.Category.Update(obj);
                _unitofwork.Save();
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

            var CategoryidFromDB = _unitofwork.Category.GetFirstorDeafult(x => x.Id == id);

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
            var obj = _unitofwork.Category.GetFirstorDeafult(x => x.Id == id);
            if (obj==null)
            {
                return NotFound();
            }
            _unitofwork.Category.Remove(obj);
            _unitofwork.Save();
            TempData["sucess"] = "Category Deleted Sucessfully";
            return RedirectToAction("Index");


        }
    }
}
