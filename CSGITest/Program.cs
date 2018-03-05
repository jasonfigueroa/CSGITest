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
        static MatchTime matchTime = new MatchTime();

        static bool stashed = false;
        static int counter = 0;
        static void OnNewGameState(GameState gs)
        {
            if (gs.Round.Phase == RoundPhase.Live && gs.Player.Name == playerName)
            {
                counter++;

                // initial match start
                if (counter == 1)
                {
                    matchTime.MatchStart = DateTime.Now;
                }

                Console.Clear();

                Console.WriteLine($"counter: {counter}");

                stashed = false;

                Console.WriteLine(gs.Player.MatchStats.JSON);

                matchStats.map = gs.Map.Name;
                matchStats.kills = gs.Player.MatchStats.Kills;
                matchStats.assists = gs.Player.MatchStats.Assists;
                matchStats.deaths = gs.Player.MatchStats.Deaths;
                matchStats.mvps = gs.Player.MatchStats.MVPs;
                matchStats.score = gs.Player.MatchStats.Score;
            }

            // the stashed flag is needed because without it the program will try 
            // go save, to the db, the match stats multiple times for one match
            if (gs.Map.Phase == MapPhase.GameOver && stashed == false)
            {
                stashed = true;
                matchTime.MatchStop = DateTime.Now;
                matchTime.MatchTotal = matchTime.MatchStop - matchTime.MatchStart;

                matchStats.minutes_played = matchTime.MatchTotal.Minutes;

                // reset counter to help track a round change
                counter = 0;

                try
                {
                    DbInterface.SaveToDb(matchStats);
                    Console.WriteLine("saved to the database");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("something went wrong while saving to the database");
                }
            }
        }
    }
}