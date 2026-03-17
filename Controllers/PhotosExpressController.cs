using Microsoft.AspNetCore.Mvc;
using Mobile.Models;
using Mobile.Models.EntityModels;
using Mobile.Models.ViewModels;
using Mobile.Utilities;

namespace Mobile.Controllers
{
    public class PhotosExpressController : Controller
    {

        private readonly Db _db;

        public IActionResult PhotosExpressEntry()
        {
            return View(new PhotosExpressViewModel());
        }

        [HttpPost]
        public IActionResult PhotosExpress(PhotosExpressViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save the data to the database
                var photosExpress = new PhotosExpress
                {
                    // Map properties from the view model to the entity model
                    // For example:
                    // Property1 = model.Property1,
                    // Property2 = model.Property2,
                };

                _db.PhotosExpress.Add(photosExpress);
                _db.SaveChanges();

                return RedirectToAction("Success"); // Redirect to a success page or another action
            }

            return View(model); // If the model state is invalid, return the view with the current model to show validation errors
        }
    }
}
