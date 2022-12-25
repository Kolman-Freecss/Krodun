namespace Model
{
    public class Player
    {
        public Player(ulong clientId)
        {
            this.Id = clientId;
            this.IsReady = false;
        }

        public Player(ulong clientId, string playerName)
        {
            this.Id = clientId;
            this.Name = playerName != null && playerName.Length > 0 ? playerName : "Player_" + clientId;
            this.IsReady = false;
        }

        public ulong Id { get; set; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        public bool IsReady { get; set; }
    }
}