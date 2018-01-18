using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavenDBExample {
    public class Character {

        public string Name { get; set; }
        public CharacterClass Class { get; set; }
        public Race Race { get; set; }
        public List<Item> Inventory { get; set; }
    }
}
