using System.Threading.Tasks;
using SteamGameChecklist.Web.Db.Contexts;
using SteamGameChecklist.Web.Helpers;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using System;
using SteamGameChecklist.Web.Db.Models;
using Microsoft.Extensions.Configuration;

namespace SteamGameChecklist.Web.Services
{
    public class GetSteamGamesService : IGetSteamGamesService
    {
        private readonly string apiKey;
        private readonly ulong userId;

        public GetSteamGamesService(IConfiguration configuration)
        {
            apiKey = configuration.GetValue<string>("SteamApiKey");
            userId = configuration.GetValue<ulong>("UserId");
        }

        public void LoadGames()
        {
            var ownedGames = AsyncHelper.RunSync(() => GetGames(apiKey, userId));

            using (var db = new SteamGameChecklistContext())
            {
                foreach (var game in ownedGames.OwnedGames)
                {
                    var g = db.Find<Game>(Convert.ToInt64(game.AppId));

                    var played = 0;
                    if (game.PlaytimeLastTwoWeeks.HasValue)
                    {
                        played = game.PlaytimeLastTwoWeeks.Value.Minutes;
                    }

                    if (g == null)
                    {
                        db.Add(new Game
                        {
                            Id = game.AppId,
                            Name = game.Name,
                            Playtime2Weeks = played,
                            PlaytimeForever = game.PlaytimeForever.Minutes,
                            Image = game.ImgLogoUrl,
                            Hidden = false,
                        });
                    }
                    else
                    {
                        g.Playtime2Weeks = played;
                        g.PlaytimeForever = game.PlaytimeForever.Minutes;
                        db.Update(g);
                    }
                }

                db.SaveChanges();
            }
        }

        private async Task<OwnedGamesResultModel> GetGames(string apiKey, ulong userId)
        {
            var playerService = new PlayerService(apiKey);
            var resp = await playerService.GetOwnedGamesAsync(userId);
            return resp.Data;
        }
    }
}
