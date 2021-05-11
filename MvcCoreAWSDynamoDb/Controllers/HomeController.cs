using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcCoreAWSDynamoDb.Models;
using MvcCoreAWSDynamoDb.Services;

namespace MvcCoreAWSBlank.Controllers {
    public class HomeController : Controller {

        public ServiceAWSDynamoDb service;

        public HomeController (ServiceAWSDynamoDb service) {
            this.service = service;
        }
        public async Task<IActionResult> Index () {
            return View(await this.service.GetCochesAsync());
        }

        public async Task<IActionResult> Details (int id) {
            return View(await this.service.GetCocheAsync(id));
        }

        public IActionResult Create () {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create (Coche car,
            String incluirmotor, String tipo, int caballos, int cilindrada) {
            if (incluirmotor != null) {
                car.Motor = new Motor();
                car.Motor.Tipo = tipo;
                car.Motor.Caballos = caballos;
                car.Motor.Cilindrada = cilindrada;
            }
            await this.service.CreateCocheAsync(car);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete (int id) {
            await this.service.DeleteCocheAsync(id);
            return RedirectToAction("Index");
        }

    }
}
