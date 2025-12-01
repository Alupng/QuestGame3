namespace QuestGame.Challenges
{
    public class PuzzleChallenge : Challenge
    {
        private string Question { get; }
        private string Answer { get; }

        public PuzzleChallenge(string question, string answer)
        {
            Question = question;
            Answer = answer.ToLower().Trim();
            Description = $"Puzzle: {question}";
        }

        public bool TrySolve(string input)
        {
            if (input.ToLower().Trim() == Answer)
            {
                IsComplete = true;
                return true;
            }

            return false;
        }
    }
}