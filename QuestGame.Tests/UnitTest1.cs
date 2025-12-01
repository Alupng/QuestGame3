using Xunit;
using QuestGame.Rooms;
using QuestGame.Challenges;
using QuestGame.GameCore;
using QuestGame.PlayerData;

namespace QuestGame.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void TestMovement()
        {
            var game = new Game();
            game.StartForTest();
            string result = game.TestMove("north");
            Assert.Contains("Hallway", result);
        }

        [Fact]
        public void TestPuzzleSolving()
        {
            var puzzle = new PuzzleChallenge("What is 2+2?", "4");
            bool ok = puzzle.TrySolve("4");
            Assert.True(ok);
            Assert.True(puzzle.IsComplete);
        }

        [Fact]
        public void TestEnemyDamage()
        {
            var enemy = new EnemyChallenge("Goblin", 20);
            enemy.TakeDamage(5);
            Assert.Equal(15, enemy.Health);
        }

        [Fact]
        public void TestBlockedRoomExit()
        {
            var room = new Room("Test", "A room");
            room.Challenges.Add(new PuzzleChallenge("1+1?", "2"));
            Assert.True(room.HasUnfinishedChallenges);
        }
    }
}