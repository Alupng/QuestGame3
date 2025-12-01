namespace QuestGame.Challenges
{
    public abstract class Challenge
    {
        public string Description { get; protected set; } = "";
        public bool IsComplete { get; protected set; } = false;
    }
}