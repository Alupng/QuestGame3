using QuestGame.PlayerData;

namespace QuestGame.Challenges
{
    public class EnemyChallenge : Challenge
    {
        public string Name { get; }
        public int Health { get; private set; }

        public EnemyChallenge(string name, int hp)
        {
            Name = name;
            Health = hp;
            Description = $"Enemy: {name} (HP: {hp})";
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                Health = 0;
                IsComplete = true;
            }
        }

        public void AttackPlayer(Player player)
        {
            player.TakeDamage(5);
        }
    }
}