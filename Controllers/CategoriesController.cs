using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Data;
using Store.Models;
using System.Linq;

namespace Store.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        public CategoriesController(ApplicationDbContext context)
        {
            db = context;
        }





        [HttpGet]
        [Authorize(Roles = "Colaborator,Administrator")]
        public IActionResult Index()
        {
            var categories = db.Categories.ToList();
            ViewBag.AllCategories = categories;
            return View();
        }





        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult New(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category); 
            }

            try
            {
                db.Categories.Add(category); 
                db.SaveChanges();
                TempData["Success"] = "Category created successfully!";
                return RedirectToAction("Index", "Categories");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the category: " + ex.Message);
                return View(category); // Revino la formular dacă apare o eroare
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(int id)
        {
            var category = db.Categories.Find(id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(Category category)
        {
            if(ModelState.IsValid)
            {
                db.Categories.Update(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = db.Categories.Find(id);
            if (category != null)
            {
                db.Categories.Remove(category);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


    }
}


