namespace MyGiHub
{
    public class GitHubSummary
    {
        public int total_count { get; set; }
    }

    public class GitHubRepository
    {
        public string name { get; set; }
    }

    public class GitHubSubscription
    {
        public bool subscribed { get; set; }
    }

    public class GitHubWatcher
    {
        public string login { get; set; }
    }
}
