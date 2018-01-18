using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Document;

namespace RavenDBExample {
    public static class DocumentStoreHolder {
        private static readonly Lazy<IDocumentStore> LazyStore =
            new Lazy<IDocumentStore>(() => {
                var store = new DocumentStore {
                    Url = "http://localhost:8080",
                    DefaultDatabase = "Northwind"
                };
                return store.Initialize();
            });

        public static IDocumentStore Store => LazyStore.Value;
    }
}
