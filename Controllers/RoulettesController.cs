using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using webapi_csharp.Data;
using webapi_csharp.Models;

namespace webapi_csharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoulettesController : ControllerBase
    {
        private readonly MockRouletteRepo _repository = new MockRouletteRepo();
        private readonly IDatabase _dataBase;

        public RoulettesController(IDatabase dataBase)
        {
            this._dataBase = dataBase;
        }

        [HttpGet]
        public string Get([FromQuery] string key)
        {
            return "ok";
        }

        [HttpPost]
        public ActionResult CreateRoulette()
        {
            if (_dataBase.KeyExists("LastId"))
            {
                string lastId = _dataBase.StringGet("LastId");
                int newId = int.Parse(lastId) + 1;
                HashEntry[] roulette = {
                    new HashEntry("id", newId),
                 };
                _dataBase.StringSet("LastId", newId);
                _dataBase.HashSet("roulette:" + newId, roulette);
                return Ok("id: " + newId);
            }
            else
            {
                HashEntry[] roulette = {
                    new HashEntry("id", 1),
                 };
                _dataBase.StringSet("LastId", 1);
                _dataBase.HashSet("roulette:1", roulette);
                return Ok("id: 1");
            }
        }

        [HttpPut("open/{id}")]
        public ActionResult openRouletteById(int id)
        {
            HashEntry[] roulette = {
                    new HashEntry("state","open"),
                 };
            if (_dataBase.HashExists("roulette:" + id, "id"))
            {
                _dataBase.HashSet("roulette:" + id, roulette);
                return Ok("Roulette Opened!!");
            }
            else
            {
                return Ok("The roulette doesn't exists");
            }
        }

        [HttpPut]
        public ActionResult MakeBet([FromForm] Bet bet)
        {
            if (bet.BetByNumber)
            {
                if (bet.Number < 0 || bet.Number > 36)
                {

                    return BadRequest("The bet number has to be between 0 and 36");
                }
                else
                {
                    HashEntry[] betData = {
                        new HashEntry("Number", bet.Number),
                        new HashEntry("Amount", bet.Amount),
                        new HashEntry("BetByNumber", bet.BetByNumber),
                        new HashEntry("Roulette", bet.Roulette),
                    };
                    this.chargeBetInRoulette(bet.Roulette, betData);
                    return Ok("NBet done in roulette" + bet.Roulette);
                }
            }
            else
            {
                HashEntry[] betData = {
                        new HashEntry("Color", bet.Color),
                        new HashEntry("Amount", bet.Amount),
                        new HashEntry("BetByNumber", bet.BetByNumber),
                        new HashEntry("Roulette", bet.Roulette),
                    };
                this.chargeBetInRoulette(bet.Roulette, betData);
                return Ok("CBet done in roulette" + bet.Roulette);
            }
        }

        private void chargeBetInRoulette(int idRoulete, HashEntry[] bet)
        {
            if (_dataBase.HashExists("bet:1:roulete:" + idRoulete, "Roulette"))
            {
                int lastBetId = int.Parse(_dataBase.StringGet("LastBet:R:" + idRoulete)) + 1;
                _dataBase.StringSet("LastBet:R:" + idRoulete, 1);
                _dataBase.HashSet("bet:" + lastBetId + ":roulete:" + idRoulete, bet);
            }
            else
            {
                _dataBase.StringSet("LastBet:R:" + idRoulete, 1);
                _dataBase.HashSet("bet:1:roulete:" + idRoulete, bet);
                _dataBase.HashSet("roulette:" + idRoulete, bet);
            }
        }

        [HttpPut("close/{id}")]
        public ActionResult CloseRoulette(int idRoulete)
        {
            HashEntry[] roulette = {
                    new HashEntry("state","closed"),
                 };
            if (_dataBase.HashExists("roulette:" + idRoulete, "id"))
            {
                _dataBase.HashSet("roulette:" + idRoulete, roulette);
                return Ok("Roulette Closed!!");
            }
            else
            {
                return Ok("The roulette doesn't exists");
            }
        }


    }
}