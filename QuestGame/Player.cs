namespace QuestGame.PlayerData
{
    public class Player
    {
        public int Health { get; private set; } = 20;

        public bool IsAlive => Health > 0;

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health < 0)
            {
                Health = 0;
            }
        }
    }
}