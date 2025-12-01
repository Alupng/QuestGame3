using System.Collections.Generic;
using System.Linq;
using QuestGame.Challenges;

namespace QuestGame.Rooms
{
    public class Room
    {
        public string Name { get; }
        public string Description { get; }
        public Dictionary<string, Room> Exits { get; } = new();
        public List<Challenge> Challenges { get; } = new();

        public bool HasUnfinishedChallenges =>
            Challenges.Any(c => !c.IsComplete);

        public Room(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}