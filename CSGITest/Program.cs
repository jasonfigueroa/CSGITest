using System;
using System.Threading;
using CSGSI;
using CSGSI.Nodes;

namespace CSGITest
{
    class Program
    {
        static GameStateListener gsl;
        static void Main(string[] args)
        {
            gsl = new GameStateListener(3000);
            gsl.NewGameState += new NewGameStateHandler(OnNewGameState);
            if (!gsl.Start())
            {
                Environment.Exit(0);
            }
            Console.WriteLine("Listening...");
        }

        static string playerName = "!logical";
        static MatchStats matchStats = new MatchStats();

        static bool stashed = false;
        static int counter = 0;
        static void OnNewGameState(GameState gs)
        {
            if (gs.Round.Phase == RoundPhase.Live && gs.Player.Name == playerName)
            {
                counter++;
                Console.Clear();

                Console.WriteLine($"counter: {counter}");
                //Console.WriteLine(gs.JSON);

                // temp off
                stashed = false;

                //Console.WriteLine($"Player Name: {gs.Player.Name}");
                //Console.WriteLine($"Match Stats: {gs.Player.MatchStats.JSON}");
                //Console.WriteLine($"Player State: {gs.Player.JSON}");
                //System.IO.File.WriteAllText(@"D:\Users\Jason\Desktop\CSGSI.json", $"{gs.AllPlayers.JSON},");

                Console.WriteLine(gs.Player.MatchStats.JSON);

                matchStats.map = gs.Map.Name;
                matchStats.kills = gs.Player.MatchStats.Kills;
                matchStats.assists = gs.Player.MatchStats.Assists;
                matchStats.deaths = gs.Player.MatchStats.Deaths;
                matchStats.mvps = gs.Player.MatchStats.MVPs;
                matchStats.score = gs.Player.MatchStats.Score;
                //matchStats.TimeStart = DateTime.Now;
                //matchStats.PrintMatchStats();
            }

            //if (gs.Round.Phase == RoundPhase.Over && stashed == false)
            //{
            //    stashed = true;

            //}

            if (gs.Map.Phase == MapPhase.GameOver && stashed == false)
            {
                //_player.PrintPlayer();
                stashed = true;
                DbInterface.SaveToDb(matchStats);
                Console.WriteLine("saved to db ...");
            }
        }
    }
}