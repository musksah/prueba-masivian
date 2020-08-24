using System.Collections.Generic;
using webapi_csharp.Models;

namespace webapi_csharp.Data{
    public interface IRouletteRepo
    {
        IEnumerable<Roulette> GetCasinoRoulettes();
        Roulette GetRouletteById(int id);
    }
}