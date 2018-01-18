using System.Linq;
using Models;
using Raven.Client.Indexes;

namespace RavenDBExample.Indexes {
    public class Employees_EmployeeCountByTitle : AbstractIndexCreationTask<Employee, Employees_EmployeeCountByTitle.Reduceresult> {
        public Employees_EmployeeCountByTitle() {

            Map = employees => from employee in employees
                               select new {
                                   employee.Title,
                                   Count = 1
                               };
            Reduce = results => from result in results
                                group result by result.Title
                                into g
                                select new {
                                    Title = g.Key,
                                    Count = g.Sum(x => x.Count)
                                };
        }

        public class Reduceresult {
            public string Title { get; set; }
            public int Count { get; set; }
        }
    }


}
