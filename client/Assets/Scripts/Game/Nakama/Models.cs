namespace Game.Nakama
{
    public class EndMatchData
    {
        public string GameId { get; set; }
        public string WinnerId { get; set; }
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}