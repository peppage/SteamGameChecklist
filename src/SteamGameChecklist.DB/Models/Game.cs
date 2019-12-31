namespace SteamGameChecklist.DB.Models
{
    public class Game
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Playtime2Weeks { get; set; }
        public int PlaytimeForever { get; set; }
        public string Image { get; set; }
        public bool Hidden { get; set; }
    }
}
