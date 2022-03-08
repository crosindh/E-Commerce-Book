using E_Book.DataAccess.Repository.IRepository;
using E_Book.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Book.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public CompanyController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        public IActionResult Index()
        {
            IEnumerable<Company> objCompany = _unitofwork.Company.GetAll();
            return View(objCompany);
        }
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company = new();

            if (id == null || id == 0)
            {
                // Create New Product

                return View(company);
            }
            else
            {
                //Update Product
                company = _unitofwork.Company.GetFirstorDeafult(x => x.Id == id);
                return View(company);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    _unitofwork.Company.Add(obj);
                    TempData["sucess"] = "Comapny Created Sucessfully";
                }
                else
                {
                    _unitofwork.Company.Update(obj);
                    TempData["sucess"] = "Company Updated Sucessfully";
                }
                _unitofwork.Save();
                return RedirectToAction("Index");
            }
            return View();
        }


        #region API CALLS 
        public IActionResult GetAll()
        {
            var companyList = _unitofwork.Company.GetAll();

            return Json(new { data = companyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitofwork.Company.GetFirstorDeafult(x => x.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            _unitofwork.Company.Remove(obj);
            _unitofwork.Save();
            return Json(new { success = true, message = "Delete Successfull" });

        }

        #endregion
    }
}
