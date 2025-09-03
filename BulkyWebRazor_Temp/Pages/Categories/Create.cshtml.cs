using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
        [BindProperties]
    public class createModel : PageModel
    {
        //to get all the data from the form
        private readonly ApplicationDbContext _db;
       
        //to get all this value from the form
        //[BindProperty]
        public Category Category { get; set; }
        public createModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            _db.categories.Add(Category);
            _db.SaveChanges();
            TempData["success"] = "category created successfully";
            return RedirectToPage("Index");
        }
    }
}
