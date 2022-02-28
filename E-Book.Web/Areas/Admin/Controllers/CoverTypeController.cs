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
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public CoverTypeController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverType = _unitofwork.CoverType.GetAll();
            return View(objCoverType);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType obj)
        {
            var checkname = _unitofwork.CoverType.GetFirstorDeafult(x => x.Name == obj.Name);
            if(checkname !=null)
            {
                ModelState.AddModelError("Name", "This Type Of Book Cover Already Exists !");
            }
            if(ModelState.IsValid)
            {
                _unitofwork.CoverType.Add(obj);
                _unitofwork.Save();
                TempData["sucess"] = "Cover Type Added Sucessfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if(id==null || id==0)
            {
                return NotFound();
            }
           
            var getidforCoverType = _unitofwork.CoverType.GetFirstorDeafult(x=> x.Id==id);
            
            if(getidforCoverType == null)
            {
                return NotFound();
            }
            return View(getidforCoverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType obj)
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
