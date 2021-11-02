using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAutoPractice.Data;
using SparkAutoPractice.Models;
using SparkAutoPractice.Models.ViewModels;
using SparkAutoPractice.Utility;

namespace SparkAutoPractice.Pages.Services
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public CarServiceViewModel CarServiceVM { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGet(int carId)
        {
            CarServiceVM = new()
            {
                Car = await _db.Car.Include(c => c.ApplicationUser).FirstOrDefaultAsync(c => c.Id == carId),
                ServiceHeader = new Models.ServiceHeader()
            };

            // Load the services that are in the shopping cart
            List<String> ServiceTypesInShoppingCart = _db.ServiceShoppingCart.Include(c => c.ServiceType)
                                                                             .Where(c => c.CarId == carId)
                                                                             .Select(c => c.ServiceType.Name)
                                                                             .ToList();

            // Load the services that are not in the shopping cart
            IQueryable<ServiceType> ServicesNotInCart = from s in _db.ServiceType
                                                        where !(ServiceTypesInShoppingCart.Contains(s.Name))
                                                        select s;

            CarServiceVM.ServiceType = ServicesNotInCart.ToList();

            CarServiceVM.ServiceShoppingCart = _db.ServiceShoppingCart.Include(c => c.ServiceType)
                                                                       .Where(c => c.CarId == carId)
                                                                       .ToList();

            // Calculate shopping cart total price
            CarServiceVM.ServiceHeader.TotalPrice = 0;
            foreach (var item in CarServiceVM.ServiceShoppingCart)
            {
                CarServiceVM.ServiceHeader.TotalPrice += item.ServiceType.Price;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCompleteService()
        {
            if (ModelState.IsValid)
            {
                CarServiceVM.ServiceHeader.DateAdded = DateTime.Now;
                CarServiceVM.ServiceHeader.CarId = CarServiceVM.Car.Id;
                CarServiceVM.ServiceShoppingCart = _db.ServiceShoppingCart.Include(c => c.ServiceType)
                                                                           .Where(c => c.CarId == CarServiceVM.Car.Id)
                                                                           .ToList();

                // Get total price of shopping cart
                foreach (var item in CarServiceVM.ServiceShoppingCart)
                {
                    CarServiceVM.ServiceHeader.TotalPrice += item.ServiceType.Price;
                }

                _db.ServiceHeader.Add(CarServiceVM.ServiceHeader);
                await _db.SaveChangesAsync();

                foreach (var detail in CarServiceVM.ServiceShoppingCart)
                {
                    ServiceDetails serviceDetails = new()
                    {
                        ServiceHeaderId = CarServiceVM.ServiceHeader.Id,
                        ServiceName = detail.ServiceType.Name,
                        ServicePrice = detail.ServiceType.Price,
                        ServiceTypeId = detail.ServiceTypeId
                    };

                    _db.ServiceDetails.Add(serviceDetails);
                }

                // Remove previous records from shopping cart
                _db.ServiceShoppingCart.RemoveRange(CarServiceVM.ServiceShoppingCart);

                await _db.SaveChangesAsync();

                return RedirectToPage("../Cars/Index", new { userId = CarServiceVM.Car.UserId });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCart()
        {
            ServiceShoppingCart ServiceCart = new()
            {
                CarId = CarServiceVM.Car.Id,
                ServiceTypeId = CarServiceVM.ServiceDetails.ServiceTypeId
            };

            _db.ServiceShoppingCart.Add(ServiceCart);
            await _db.SaveChangesAsync();
            return RedirectToPage("Create", new { carId = CarServiceVM.Car.Id });
        }

        public async Task<IActionResult> OnPostRemoveFromCart(int serviceTypeId)
        {
            ServiceShoppingCart ServiceCart = _db.ServiceShoppingCart
                                                 .FirstOrDefault(x => x.CarId == CarServiceVM.Car.Id && x.ServiceTypeId == serviceTypeId);

            _db.ServiceShoppingCart.Remove(ServiceCart);
            await _db.SaveChangesAsync();
            return RedirectToPage("Create", new { carId = CarServiceVM.Car.Id });
        }
    }
}
