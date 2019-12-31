using Microsoft.AspNetCore.Mvc;
using SteamGameChecklist.DB.Contexts;
using SteamGameChecklist.DB.Models;
using SteamGameChecklist.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace SteamGameChecklist.Web.Controllers
{
    [Route("api/[controller]")]
    public class ChecklistController : Controller
    {
        [HttpGet("[action]")]
        public IEnumerable<Game> AllGames()
        {
            using (var db = new ChecklistContext())
            {
                return db.Games.Where(g => !g.Hidden)
                    .OrderByDescending(g => g.Playtime2Weeks)
                    .ThenByDescending(g => g.PlaytimeForever).ToList();
            }
        }

        [HttpPost("[action]")]
        public void HideGame([FromBody] HideGameRequest req)
        {
            using (var db = new ChecklistContext())
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
            using (var db = new ChecklistContext())
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
