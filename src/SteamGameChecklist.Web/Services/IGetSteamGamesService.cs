namespace SteamGameChecklist.Web.Services
{
    public interface IGetSteamGamesService
    {
        /// <summary>
        /// Load a users's steam games from the config
        /// </summary>
        void LoadGames();
    }
}
