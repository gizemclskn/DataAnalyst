using DataAnalyst.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Python.Runtime;
using System;
using System.IO;

namespace DataAnalyst.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public HomeController(IWebHostEnvironment environment)
        {
            _environment = environment;

            try
            {
                // PythonDLL özelliğini ayarlayın
                Runtime.PythonDLL = @"C:\Users\gizem\AppData\Local\Programs\Python\Python312\python312.dll";

                // Python motorunu başlatın
                PythonEngine.Initialize();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Python başlatma hatası: {ex.Message}");
                throw; // veya başlatma hatasını nazikçe ele alabilirsiniz
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(UploadModel model)
        {
            try
            {
                if (model.File != null && model.File.Length > 0)
                {
                    // Dosya yolunu belirleyin
                    string fileDirectory = Path.Combine(_environment.ContentRootPath, "uploads", model.File.FileName);

                    using (var fileStream = new FileStream(fileDirectory, FileMode.Create))
                    {
                        model.File.CopyTo(fileStream);
                    }

                    // Python scriptini çalıştır
                    using (Py.GIL())
                    {
                        dynamic sys = Py.Import("sys");
                        sys.path.append(Path.Combine(_environment.ContentRootPath, "scripts"));

                        dynamic analyzeData = Py.Import("analyze_data");
                        string imagePath = analyzeData.analyze_data(fileDirectory);

                        // Grafik dosyasının yolunu ViewBag aracılığıyla view'e iletebilirsiniz
                        ViewBag.ImagePath = "/uploads/" + Path.GetFileName(imagePath);
                    }
                }
            }
            catch (PythonException pyEx)
            {
                ViewBag.ErrorMessage = $"Python hatası: {pyEx.Message}";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Dosya yükleme veya analizi sırasında bir hata oluştu: {ex.Message}";
            }

            return View("Index");
        }
    }
}
