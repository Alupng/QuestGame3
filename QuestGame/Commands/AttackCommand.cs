using QuestGame.GameCore;

namespace QuestGame.Commands
{
    public class AttackCommand : ICommand
    {
        private Game _game;

        public AttackCommand(Game game)
        {
            _game = game;
        }

        public void Execute(string input)
        {
            _game.AttackEnemy();
        }
    }
}