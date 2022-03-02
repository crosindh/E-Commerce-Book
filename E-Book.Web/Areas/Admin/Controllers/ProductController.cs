using E_Book.DataAccess;
using E_Book.DataAccess.Repository.IRepository;
using E_Book.Models;
using E_Book.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace E_Book.Web.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitofwork,IWebHostEnvironment hostEnvironment)
        {
            _unitofwork = unitofwork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
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
                //Update Product
                productVM.Product = _unitofwork.Product.GetFirstorDeafult(x=>x.Id==id);
                return View(productVM);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if(file !=null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploads =Path.Combine(wwwRootPath,@"images\products");
                    var extention = Path.GetExtension(file.FileName);
                    using (var filestreams = new FileStream(Path.Combine(uploads, filename + extention), FileMode.Create))
                    {
                        file.CopyTo(filestreams);
                    }
                    obj.Product.ImageURL = @"\images\products\" + filename + extention;
                }
                _unitofwork.Product.Add(obj.Product);
                _unitofwork.Save();
                TempData["sucess"] = "Product Created Sucessfully";
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

        #region API CALLS 
        public IActionResult GetAll()
        {
            var productList = _unitofwork.Product.GetAll(includeProperties:"Category,CoverType");

            return Json(new {data = productList});
        }
        #endregion
    }
}
