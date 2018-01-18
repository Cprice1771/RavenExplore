using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Raven.Client.Indexes;

namespace RavenDBExample.Indexes {
    public class Product_ByName : AbstractIndexCreationTask<Product> {
        public Product_ByName() {
            Map = products => from product in products
                              select new { product.Name };
            
        }

    }
}
