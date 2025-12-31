namespace CyberHejmiBot.Data.Entities.JobRelated
{
    public class MrStreamerCheckerLogs
    {
        public int Id { get; set; }
        public string? JobName { get; set; }
        public DateTime LastSuccessfullCheck { get; set; }
    }
}
