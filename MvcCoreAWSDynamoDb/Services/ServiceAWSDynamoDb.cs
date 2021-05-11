﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using MvcCoreAWSDynamoDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreAWSDynamoDb.Services {
    public class ServiceAWSDynamoDb {

        private DynamoDBContext context;

        public ServiceAWSDynamoDb () {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();
            this.context = new DynamoDBContext(client);
        }

        public async Task CreateCocheAsync (Coche car) {
            await this.context.SaveAsync<Coche>(car);
        }

        public async Task<List<Coche>> GetCochesAsync () {
            var tabla = this.context.GetTargetTable<Coche>();
            var scanOptions = new ScanOperationConfig();
            var results = tabla.Scan(scanOptions);

            List<Document> data = await results.GetNextSetAsync();
            IEnumerable<Coche> coches = this.context.FromDocuments<Coche>(data);
            return coches.ToList();
        }

        public async Task<Coche> GetCocheAsync (int idCoche) {
            return await this.context.LoadAsync<Coche>(idCoche);
        }

        public async Task DeleteCocheAsync (int idCoche) {
            await this.context.DeleteAsync<Coche>(idCoche);
        }
    }
}
