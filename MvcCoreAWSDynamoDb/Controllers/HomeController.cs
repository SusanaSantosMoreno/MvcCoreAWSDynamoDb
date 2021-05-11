using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcCoreAWSDynamoDb.Helpers;
using MvcCoreAWSDynamoDb.Models;
using MvcCoreAWSDynamoDb.Services;

namespace MvcCoreAWSBlank.Controllers {
    public class HomeController : Controller {

        public ServiceAWSDynamoDb serviceDynamo;
        private UploadHelper uploadhelper;
        public ServiceAWSS3 ServiceS3;

        public HomeController (ServiceAWSDynamoDb service, ServiceAWSS3 serviceS3, UploadHelper helper) {
            this.serviceDynamo = service;
            this.ServiceS3 = serviceS3;
            this.uploadhelper = helper;
        }
        public async Task<IActionResult> Index () {
            return View(await this.serviceDynamo.GetCochesAsync());
        }

        public async Task<IActionResult> Details (int id) {
            Coche coche = await this.serviceDynamo.GetCocheAsync(id);
            Stream stream = await this.ServiceS3.GetFileAsync(coche.Imagen);
            ViewData["Imagen"] = File(stream, "image/png");
            return View(coche);
        }

        public IActionResult Create () {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create (int idCoche, String marca, 
            String modelo, int velocidadMaxima, IFormFile imagen, 
            String incluirmotor, String tipo, int caballos, int cilindrada) {

            Coche coche = new Coche(idCoche, marca, modelo, velocidadMaxima, imagen.FileName);
            if (incluirmotor != null) {
                coche.Motor = new Motor();
                coche.Motor.Tipo = tipo;
                coche.Motor.Caballos = caballos;
                coche.Motor.Cilindrada = cilindrada;
            }

            String path = await this.uploadhelper.UploadFileAsync(imagen, Folders.Images);
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                bool respuesta = await this.ServiceS3.UploadFileAsync(stream, imagen.FileName);
            };
            await this.serviceDynamo.CreateCocheAsync(coche);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete (int id) {
            Coche coche = await this.serviceDynamo.GetCocheAsync(id);
            await this.ServiceS3.DeleteFileAsync(coche.Imagen);
            await this.serviceDynamo.DeleteCocheAsync(id);
            return RedirectToAction("Index");
        }

    }
}
