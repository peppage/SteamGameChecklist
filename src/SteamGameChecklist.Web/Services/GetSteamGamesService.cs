using Microsoft.Extensions.Configuration;
using Steam.Models.SteamCommunity;
using SteamGameChecklist.DB.Contexts;
using SteamGameChecklist.DB.Models;
using SteamGameChecklist.Web.Helpers;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SteamGameChecklist.Web.Services
{
    public class GetSteamGamesService : IGetSteamGamesService
    {
        private readonly ulong userId;
        private readonly PlayerService playerService;

        public GetSteamGamesService(IConfiguration configuration)
        {
            var apiKey = configuration.GetValue<string>("SteamApiKey");
            userId = configuration.GetValue<ulong>("UserId");

            var webInterfaceFactory = new SteamWebInterfaceFactory(apiKey);
            playerService = webInterfaceFactory.CreateSteamWebInterface<PlayerService>(new HttpClient());
        }

        public void LoadGames()
        {
            var ownedGames = AsyncHelper.RunSync(() => GetGames(userId));

            using var db = new ChecklistContext();

            foreach (var game in ownedGames.OwnedGames)
            {
                var g = db.Find<Game>(Convert.ToInt64(game.AppId));

                var played = 0;
                if (game.PlaytimeLastTwoWeeks.HasValue)
                {
                    played = (int)game.PlaytimeLastTwoWeeks.Value.TotalMinutes;
                }

                if (g == null)
                {
                    db.Add(new Game
                    {
                        Id = game.AppId,
                        Name = game.Name,
                        Playtime2Weeks = played,
                        PlaytimeForever = (int)game.PlaytimeForever.TotalMinutes,
                        Image = game.ImgLogoUrl,
                        Hidden = false,
                    });
                }
                else
                {
                    g.Playtime2Weeks = played;
                    g.PlaytimeForever = (int)game.PlaytimeForever.TotalMinutes;
                    db.Update(g);
                }
            }

            db.SaveChanges();
        }

        private async Task<OwnedGamesResultModel> GetGames(ulong userId)
        {
            var resp = await playerService.GetOwnedGamesAsync(userId, true, true).ConfigureAwait(false);
            return resp.Data;
        }
    }
}
