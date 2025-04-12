using InventoryClient.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InventoryClient.Controllers
{
    public class ProductsController : Controller
    {
        private readonly string apiBaseUrl = "http://localhost/inventory_api/";

        // GET: Products
        public async Task<ActionResult> Index(string searchTerm = "")
        {
            List<Product> products = new List<Product>();

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(apiBaseUrl + "read.php");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<Product>>(json);
                }
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = products.Where(p => p.name.ToLower().Contains(searchTerm.ToLower())).ToList();
            }

            return View(products);
        }

        // GET: Products/Search - New action for AJAX search
        public async Task<ActionResult> Search(string searchTerm = "")
        {
            List<Product> products = new List<Product>();

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(apiBaseUrl + "read.php");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<Product>>(json);
                }
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = products.Where(p => p.name.ToLower().Contains(searchTerm.ToLower())).ToList();
            }

            return PartialView("_ProductList", products);
        }

        // The rest of your controller methods remain unchanged
        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        public async Task<ActionResult> Create(Product product)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var jsonData = JsonConvert.SerializeObject(product);
                var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiBaseUrl + "create.php", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Product created successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create product.");
                }
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Product product = null;

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(apiBaseUrl + "read.php");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<Product>>(json);
                    product = products.Find(p => p.id == id);
                }
            }

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        private ActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

        // POST: Products/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(Product product)
        {
            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(product);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiBaseUrl + "update.php", content); // You can use PUT if your PHP server supports it
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Product updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update product.");
                }
            }
            return View(product);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var payload = new { id = id };
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiBaseUrl + "delete.php", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Product deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to delete product.";
                }
            }

            return RedirectToAction("Index");
        }
    }
}