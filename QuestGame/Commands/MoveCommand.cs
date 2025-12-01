using QuestGame.GameCore;

namespace QuestGame.Commands
{
    public class MoveCommand : ICommand
    {
        private Game _game;

        public MoveCommand(Game game)
        {
            _game = game;
        }

        public void Execute(string input)
        {
            string direction = input.Substring(5).Trim();
            _game.Move(direction);
        }
    }
}