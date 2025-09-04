using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //to access to root folder
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();


            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList =
               _unitOfWork.Category.GetAll().Select(u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString()
               });
            //ViewBag.CategoryList = CategoryList;
            ProductVM productVM = new()
            {
                CategoryList = CategoryList,
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            //custom validation apply


            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string finalPath = Path.Combine(wwwRootPath, "Images", "product");

                    string filePath = Path.Combine(finalPath, fileName);

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldpath =
                            Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('/'));

                        if (System.IO.File.Exists(oldpath))
                        {
                            System.IO.File.Delete(oldpath);
                        }
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\Images\product\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);

                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }


                _unitOfWork.Save();
                TempData["success"] = "Product Created Successfully!";
                return RedirectToAction("Index", "Product");
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product productFromdb = _unitOfWork.Product.Get(u => u.Id == id);

            if (productFromdb == null)
            {
                return NotFound();
            }
            return View(productFromdb);
        }
        [HttpPost]

        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product Updated Successfully!";
                return RedirectToAction("Index");
            }

            return View();
        }

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}
        //to recognize from the frontend , get put, post etc
        //[HttpPost, ActionName("Delete")]

        //public IActionResult DeletePOST(int? id)
        //{
        //    Product? obj = _unitOfWork.Product.Get(u => u.Id == id);

        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }

        //    _unitOfWork.Product.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product Deleted Successfully!";
        //    return RedirectToAction("Index");
        //}
     
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = objProductList});
        }
        #endregion

        #region API CALLS
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }


            string wwwRootPath = _webHostEnvironment.WebRootPath;

           
                string finalPath = Path.Combine(wwwRootPath, "Images", "product");

                string filePath = Path.Combine(finalPath, productToBeDeleted.ImageUrl);

            if (!string.IsNullOrEmpty(productToBeDeleted.ImageUrl))
            {
                var oldpath = Path.Combine(wwwRootPath, productToBeDeleted.ImageUrl.TrimStart('\\', '/'));

                if (System.IO.File.Exists(oldpath))
                {
                    System.IO.File.Delete(oldpath);
                }
            }



            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }

}
