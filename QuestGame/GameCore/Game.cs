using System;
using System.Collections.Generic;
using QuestGame.Rooms;
using QuestGame.Challenges;
using QuestGame.Commands;
using QuestGame.PlayerData;

namespace QuestGame.GameCore
{
    public class Game
    {
        private Room currentRoom;
        private Player player;
        private Dictionary<string, ICommand> commands;
        private bool gameOver;

        public Game()
        {
            player = new Player();
            commands = new Dictionary<string, ICommand>();
            gameOver = false;
        }

        public void Start()
        {
            BuildWorld();
            SetupCommands();

            Console.WriteLine("Welcome to QuestGame");
            Console.WriteLine("--------------------");
            ShowRoom();

            while (!gameOver)
            {
                Console.Write("\n> ");
                var line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                line = line.Trim();

                if (line.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Goodbye");
                    break;
                }

                if (line.Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    PrintHelp();
                    continue;
                }

                if (line.Equals("look", StringComparison.OrdinalIgnoreCase))
                {
                    ShowRoom();
                    continue;
                }

                if (line.Equals("solve", StringComparison.OrdinalIgnoreCase))
                {
                    SolvePuzzle();
                    continue;
                }

                var parts = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                var commandName = parts[0].ToLower();

                if (commands.TryGetValue(commandName, out var command))
                {
                    try
                    {
                        command.Execute(line);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Unknown command. Type help.");
                }

                if (!player.IsAlive)
                {
                    Console.WriteLine();
                    Console.WriteLine("You died.");
                    gameOver = true;
                }
            }
        }

        private void SetupCommands()
        {
            commands.Clear();
            commands["move"] = new MoveCommand(this);
            commands["attack"] = new AttackCommand(this);
        }

        private void PrintHelp()
        {
            Console.WriteLine("move <direction>");
            Console.WriteLine("attack");
            Console.WriteLine("solve");
            Console.WriteLine("look");
            Console.WriteLine("help");
            Console.WriteLine("quit");
        }

        private void BuildWorld()
        {
            var start = new Room("Entrance", "Small room with a wooden door.");
            var hall = new Room("Hallway", "A narrow corridor that smells damp.");
            var riddle = new Room("Riddle Room", "There are numbers scratched into the wall.");
            var lair = new Room("Lair", "You hear something breathing.");
            var last = new Room("Last Room", "Feels like the end of the dungeon.");

            riddle.Challenges.Add(new PuzzleChallenge("What is 4 + 9?", "13"));
            lair.Challenges.Add(new EnemyChallenge("Goblin", 18));
            last.Challenges.Add(new PuzzleChallenge("Type 'finish' to continue.", "finish"));
            last.Challenges.Add(new EnemyChallenge("Guardian", 25));

            start.Exits["north"] = hall;

            hall.Exits["south"] = start;
            hall.Exits["east"] = riddle;

            riddle.Exits["west"] = hall;
            riddle.Exits["north"] = lair;

            lair.Exits["south"] = riddle;
            lair.Exits["north"] = last;

            last.Exits["south"] = lair;

            currentRoom = start;
        }

        public void Move(string direction)
        {
            if (currentRoom == null)
            {
                Console.WriteLine("No room loaded.");
                return;
            }

            if (currentRoom.HasUnfinishedChallenges)
            {
                Console.WriteLine("You need to finish the challenges here first.");
                return;
            }

            var d = direction.ToLower();

            if (currentRoom.Exits.TryGetValue(d, out var next))
            {
                currentRoom = next;
                Console.WriteLine();
                Console.WriteLine("You go " + d + ".");
                ShowRoom();
            }
            else
            {
                Console.WriteLine("You cannot go that way.");
            }
        }

        public void ShowRoom()
        {
            if (currentRoom == null)
            {
                Console.WriteLine("No current room.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine(currentRoom.Name);
            Console.WriteLine(currentRoom.Description);

            if (currentRoom.Challenges.Count > 0)
            {
                Console.WriteLine("Challenges:");
                foreach (var c in currentRoom.Challenges)
                {
                    var status = c.IsComplete ? "done" : "not done";
                    Console.WriteLine("- " + c.Description + " (" + status + ")");
                }
            }
            else
            {
                Console.WriteLine("No challenges in this room.");
            }

            if (currentRoom.Exits.Count > 0)
            {
                Console.WriteLine("Exits: " + string.Join(", ", currentRoom.Exits.Keys));
            }
            else
            {
                Console.WriteLine("No exits from this room.");
            }

            Console.WriteLine("Health: " + player.Health);
        }

        public void SolvePuzzle()
        {
            if (currentRoom == null)
            {
                Console.WriteLine("Nowhere to solve anything.");
                return;
            }

            PuzzleChallenge puzzle = null;

            foreach (var c in currentRoom.Challenges)
            {
                if (!c.IsComplete)
                {
                    var p = c as PuzzleChallenge;
                    if (p != null)
                    {
                        puzzle = p;
                        break;
                    }
                }
            }

            if (puzzle == null)
            {
                Console.WriteLine("No puzzle here.");
                return;
            }

            Console.WriteLine(puzzle.Description);
            Console.Write("Answer: ");
            var input = Console.ReadLine();
            if (input == null) input = "";

            try
            {
                var ok = puzzle.TrySolve(input);
                if (ok)
                {
                    Console.WriteLine("Puzzle solved.");
                }
                else
                {
                    Console.WriteLine("That is not right.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem while solving: " + ex.Message);
            }
        }

        public void AttackEnemy()
        {
            if (currentRoom == null)
            {
                Console.WriteLine("You are not in a room.");
                return;
            }

            EnemyChallenge enemy = null;

            foreach (var c in currentRoom.Challenges)
            {
                if (!c.IsComplete)
                {
                    var e = c as EnemyChallenge;
                    if (e != null)
                    {
                        enemy = e;
                        break;
                    }
                }
            }

            if (enemy == null)
            {
                Console.WriteLine("No enemy to attack.");
                return;
            }

            enemy.TakeDamage(5);
            Console.WriteLine("You hit the " + enemy.Name + ". Enemy HP: " + enemy.Health);

            if (!enemy.IsComplete)
            {
                enemy.AttackPlayer(player);
                Console.WriteLine("The " + enemy.Name + " hits you. HP: " + player.Health);
            }
            else
            {
                Console.WriteLine("You defeated the " + enemy.Name + ".");
            }
        }

        public void StartForTest()
        {
            BuildWorld();
        }

        public string TestMove(string direction)
        {
            Move(direction);
            if (currentRoom == null) return "";
            return currentRoom.Name;
        }
    }
}