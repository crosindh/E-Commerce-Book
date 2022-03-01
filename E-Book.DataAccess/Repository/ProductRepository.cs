using E_Book.DataAccess.Repository.IRepository;
using E_Book.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Book.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Product obj)
        {
            var objFromDB = _db.Products.FirstOrDefault(x => x.Id == obj.Id);
            if(objFromDB !=null)
            {
                objFromDB.Title = obj.Title;
                objFromDB.ISBN = obj.ISBN;
                objFromDB.Description = obj.Description;
                objFromDB.Author = obj.Author;
                objFromDB.ListPrice = obj.ListPrice;
                objFromDB.Price = obj.Price;
                objFromDB.Price50 = obj.Price50;
                objFromDB.Price100 = obj.Price100;
                objFromDB.CategoryId = obj.CategoryId;
                objFromDB.CoverTypeId = obj.CoverTypeId;

                if (objFromDB.ImageURL !=null)
                {
                    objFromDB.ImageURL = obj.ImageURL;
                }
            }
        }
    }
}
