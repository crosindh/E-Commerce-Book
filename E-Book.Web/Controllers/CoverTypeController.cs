using E_Book.DataAccess;
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
        private readonly ApplicationDbContext _db;
        public CoverTypeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverType = _db.CoverTypes.ToList();
            return View(objCoverType);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(CoverType obj)
        {
            if(ModelState.IsValid)
            {
                _db.Add(obj);
                _db.SaveChanges();
                RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if(id==null || id==0)
            {
                return NotFound();
            }
           
            var getidforCoverType = _db.CoverTypes.FirstOrDefault(x=> x.Id==id);
            
            if(getidforCoverType == null)
            {
                return NotFound();
            }
            return View(getidforCoverType);
        }
        [HttpPost]
        public IActionResult Edit(CoverType obj)
        {
            if (ModelState.IsValid)
            {
               _db.Update(obj);
               _db.SaveChanges();
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

            var getidforCoverType = _db.CoverTypes.FirstOrDefault(x => x.Id == id);

            if (getidforCoverType == null)
            {
                return NotFound();
            }
            return View(getidforCoverType);
        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _db.CoverTypes.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
            
            
        }


    }
}
