using System.Collections.Generic;
using webapi_csharp.Models;

namespace webapi_csharp.Data{
    public class MockRouletteRepo : IRouletteRepo
    {
        public IEnumerable<Roulette> GetCasinoRoulettes()
        {
            var roulettes = new List<Roulette>{
                new Roulette{Id=1,State="Open"},
                new Roulette{Id=2,State="Closed"},
                new Roulette{Id=3,State="Open"},
            };
            return roulettes;
        }

        public Roulette GetRouletteById(int id)
        {
            return new Roulette{Id=1,State="Open"};
        }
    }
}