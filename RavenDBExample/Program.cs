using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Json.Linq;
using RavenDBExample.Indexes;

namespace RavenDBExample {
    class Program {

        static void AggregateIndex() {
            IndexCreation.CreateIndexes(typeof(Employees_EmployeeCountByTitle).Assembly, DocumentStoreHolder.Store);
            using (var session = DocumentStoreHolder.Store.OpenSession()) {
                var result = session.Query<Employees_EmployeeCountByTitle.Reduceresult, Employees_EmployeeCountByTitle>();

                foreach (var item in result) {
                    Console.WriteLine($"{item.Title}: {item.Count}");
                }
            }
        }

        static void GetUpdateItem() {
            using (var session = DocumentStoreHolder.Store.OpenSession()) {
                var p1 = session.Load<Product>("products/1");
                p1.Name = "LALALA";
                session.SaveChanges();
                var products = session.Query<Product, Product_ByName>().Where(x => x.Name == "LALALA").FirstOrDefault();
                Console.WriteLine(products.Name);
            }
        }

        static void Attatchments() {
            var file = File.OpenRead(@"C:\rm.png");
            DocumentStoreHolder.Store.DatabaseCommands.PutAttachment("images/1", null, data: file, metadata: new RavenJObject { { "Description", "A Picture" } });

            using (var session = DocumentStoreHolder.Store.OpenSession()) {
                var p1 = session.Load<Product>("products/1");


                p1.ImageId = "images/1";

                session.SaveChanges();

                var attatch = DocumentStoreHolder.Store.DatabaseCommands.GetAttachment(p1.ImageId);

                Console.WriteLine($"Size: {attatch.Size}");

            }
        }

        static void Paging() {
            using (var session = DocumentStoreHolder.Store.OpenSession()) {
                int pageNumber = 0;
                int resultsPerPage = 2;
                RavenQueryStatistics stats;
                var products = session.Query<Product>()
                    .Statistics(out stats)
                    .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(10)))
                    .Skip(pageNumber * resultsPerPage)
                    .Take(resultsPerPage)
                    .ToArray();

                foreach (var product in products) {
                    Console.WriteLine(product.Name);
                }

                Console.WriteLine($"Index Stale?: {stats.IsStale}");
                Console.WriteLine($"Total Documents: {stats.TotalResults}");
            }
        }

        static void Includes() {
            using (var session = DocumentStoreHolder.Store.OpenSession()) {

                var supplier = session.Query<Supplier>().FirstOrDefault();

                var newProduct = new Product() {
                    Id = "product/125",
                    Name = "Apples for Apples",
                    Supplier = supplier.Id,
                    Category = "Board Game",
                    QuantityPerUnit = "1 x 10",
                    PricePerUnit = 20.00m,
                    UnitsInStock = 10,
                    UnitsOnOrder = 5,
                    Discontinued = false,
                    ReorderLevel = 0,
                    ImageId = null
                };

                session.Store(newProduct);
                session.SaveChanges();
            }

            using (var session = DocumentStoreHolder.Store.OpenSession()) {

                var product = session.Include<Product>(x => x.Supplier).Load<Product>("product/125");

                Console.WriteLine(product.Name);

                var supplier = session.Load<Supplier>(product.Supplier);

                Console.WriteLine(supplier.Name);

            }

        }

        static void Patch() {
            var newTerritory = new Territory { Code = "123", Name = "404", Area = "South" };
            DocumentStoreHolder.Store.DatabaseCommands.Patch("regions/3",
                new[] {
                    new PatchRequest{
                        Type = PatchCommandType.Add,
                        Name = "Territories",
                        Value = RavenJObject.FromObject(newTerritory)
                    }
                });
        }

        static void Main(string[] args) {

            Patch();
        }
    }
}
