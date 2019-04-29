using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SteamGameChecklist.Web.Db.Contexts;
using SteamGameChecklist.Web.Db.Models;
using System.Linq;

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
        public void HideGame(long id)
        {
            using (var db = new SteamGameChecklistContext())
            {
                var game = db.Find<Game>(id);
                game.Hidden = true;
                db.Update(game);
            }
        }
    }
}
