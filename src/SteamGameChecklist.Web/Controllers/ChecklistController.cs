using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SteamGameChecklist.Web.Db.Contexts;
using SteamGameChecklist.Web.Db.Models;
using System.Linq;
using SteamGameChecklist.Web.Models;

namespace SteamGameChecklist.Web.Controllers
{
    [Route("api/[controller]")]
    public class ChecklistController : Controller
    {
        [HttpGet("[action]")]
        public IEnumerable<Game> AllGames()
        {
            using (var db = new SteamGameChecklistContext())
            {
                return db.Games.Where(g => g.Hidden == false)
                    .OrderByDescending(g => g.Playtime2Weeks)
                    .ThenByDescending(g => g.PlaytimeForever).ToList();
            }
        }

        [HttpPost("[action]")]
        public void HideGame([FromBody] HideGameRequest req)
        {
            using (var db = new SteamGameChecklistContext())
            {
                var game = db.Find<Game>(req.GameId);
                game.Hidden = true;
                db.Update(game);
                db.SaveChanges();
            }
        }

        [HttpGet("[action]")]
        public Stats Stats()
        {
            using (var db = new SteamGameChecklistContext())
            {
                return new Stats
                {
                    TotalGames = db.Games.Count(),
                    FinishedGames = db.Games.Where(g => g.Hidden).Count(),
                };
            }
        }
    }
}
