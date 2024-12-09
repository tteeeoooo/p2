using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Models;

namespace Store.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        public ProductsController(ApplicationDbContext context)
        {
            db = context;
        }
        // metodele Index si Show sunt get, New si Edit sunt get si post, iar Delete e post
        [HttpGet]
        public IActionResult Index()
        {
            var products = db.Products.Include(p => p.Category).ToList();
            return View(products);
        }



        [HttpGet]
        public ActionResult Details(int id)// afisarea detaliilor unui singur produs in functie de id
        {
            var product = db.Products
                          .Include(p => p.Reviews) 
                          .FirstOrDefault(p => p.Id == id);



            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [HttpGet]
        [Authorize(Roles = "Colaborator,Administrator")]
        public IActionResult Create()
        {
            var categories = db.Categories.ToList();
            if (categories == null || !categories.Any())
            {
                Console.WriteLine("No categories found in the database.");
            }
            else
            {
                Console.WriteLine($"Loaded {categories.Count} categories.");
            }

            ViewBag.AllCategories = categories;
            return View();
        }



        [HttpPost]
        [Authorize(Roles = "Colaborator,Administrator")]
        public IActionResult Create(Product product)
        {
            Console.WriteLine($"CategoryId received: {product.CategoryId}");

            
            var newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Brand = product.Brand,
                Ingredients = product.Ingredients,
                Stock = product.Stock,
                CategoryId = product.CategoryId, 
                DateListed = DateTime.Now
            };

            db.Products.Add(newProduct);
            db.SaveChanges();

            return RedirectToAction("Index", "Products");

        }



        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var product = db.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null || product.Stock <= 0)
            {
                TempData["Error"] = "The product is out of stock and cannot be added to the cart.";
                return RedirectToAction("Details", new { id = productId });
            }
            TempData["Success"] = $"{product.Name} has been added to your cart!";
            return RedirectToAction("Details", new { id = productId });
        }



        [HttpGet]
        [Authorize(Roles = "Colaborator,Administrator")]
        public IActionResult Edit(int id)
        {
            Product? product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.EditedProduct = product;
            return View();
        }
        [HttpPost]
        public ActionResult Edit(int id, Product requested_obj)
        {
            Product? product = db.Products.Find(id);
            try
            {   // instantierea noului obiect de tip product pt adaugarea lui in bd
                product.Name = requested_obj.Name;
                product.Description = requested_obj.Description;
                product.DateListed = requested_obj.DateListed;
                product.Price = requested_obj.Price;
                product.Brand = requested_obj.Brand;
                product.Ingredients = requested_obj.Ingredients;
                product.Stock = requested_obj.Stock;
                db.SaveChanges();
                return RedirectToAction("Index");// dupa editare, user-ul e redirectionat catre view-ul index
            }
            catch (Exception)
            {
                return RedirectToAction("Edit", new { id = product.Id });
            }
        }


        [HttpPost]
        [Authorize(Roles = "Colaborator,Administrator")]
        public ActionResult Delete(int id)// delete nu are un view coresp
        {
            Product? product = db.Products.Find(id);
            if (product != null)
            {
                db.Products.Remove(product);// metoda remove
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }


        

        
    }
}