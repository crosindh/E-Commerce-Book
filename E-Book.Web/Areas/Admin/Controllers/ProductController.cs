using E_Book.DataAccess;
using E_Book.DataAccess.Repository.IRepository;
using E_Book.Models;
using E_Book.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Book.Web.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public ProductController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverType = _unitofwork.CoverType.GetAll();
            return View(objCoverType);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitofwork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                CoverTypeList = _unitofwork.CoverType.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
           

            if (id==null || id==0)
            {
                // Create New Product
              
                return View(productVM);
            }
            else
            {
                //Update Productc

            }
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType obj)
        {
            if (ModelState.IsValid)
            {
                _unitofwork.CoverType.Update(obj);
                _unitofwork.Save();
                TempData["sucess"] = "Cover Type Updated Sucessfully";
               return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var getidforCoverType = _unitofwork.CoverType.GetFirstorDeafult(x => x.Id == id);

            if (getidforCoverType == null)
            {
                return NotFound();
            }
            return View(getidforCoverType);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitofwork.CoverType.GetFirstorDeafult(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitofwork.CoverType.Remove(obj);
            _unitofwork.Save();
            TempData["sucess"] = "Cover Type Deleted Sucessfully";
            return RedirectToAction("Index");
            
            
        }


    }
}
