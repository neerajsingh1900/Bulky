using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
        
namespace BulkyWeb.Controllers
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
            List<Category> objCategoryList = _db.categories.ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();  
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //custom validation apply
            if(obj?.Name == obj?.DisplayOrder.ToString())
            {
                //mapping in the name parameter
                 ModelState.AddModelError("name","The display order cannot exactly match the Name.");
            }

            if (obj?.Name.ToLower() == "test")
            {
         ModelState.AddModelError("","The display order cannot exactly match the name");
            }

            if (ModelState.IsValid)
            {
                _db.categories.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Category Created Successfully!";
                return RedirectToAction("Index", "Category");
            }
                
            return View();
        }
     
        public IActionResult Edit(int? id)
        {
            if(id==null || id == 0)
            {
                return NotFound();
            }
            Category categoryFromDb = _db.categories.Find(id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
      
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.categories.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Category Updated Successfully!";
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
            Category categoryFromDb = _db.categories.Find(id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        //to recognize from the frontend , get put, post etc
        [HttpPost,ActionName("Delete")]

        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _db.categories.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

                _db.categories.Remove(obj);
                _db.SaveChanges();
            TempData["success"] = "Category Deleted Successfully!";
            return RedirectToAction("Index");
        }
    }
}
