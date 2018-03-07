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

        // creating Player
        //static string playerName = "!logical"; // temporarily hard coded
        static string steamId = "76561197994895226"; // temporarily hard coded
        //Player player = new Player(playerName, steamId);

        // TODO need user_id, the following is a temporary placeholder
        //static int userId = 1;

        // creating Match
        static Match match = new Match();

        // TODO need match_id, the following is a temporary placeholder 
        //static int matchId = 1;

        // creating MatchStats
        static MatchStats matchStats = new MatchStats();

        // creating MatchTime, for calculating minutes played per game match
        static MatchTime matchTime = new MatchTime();

        // helper variables for the magic
        static bool stashed = false;
        static int counter = 0;

        // NOTE: application does not account for the player switching teams 
        // and will only capture the initial team the player joins
        static void OnNewGameState(GameState gs)
        {
            if (gs.Round.Phase == RoundPhase.Live && gs.Player.SteamID == steamId)
            {
                counter++;
                stashed = false;

                // first round played for each match
                if (counter == 1)
                {
                    matchTime.MatchStart = DateTime.Now;
                    match.datetime_start = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    match.map_name = gs.Map.Name;
                }

                Console.Clear();

                Console.WriteLine($"counter: {counter}");

                // TODO need better printing
                Console.WriteLine("MatchStats.JSON");
                Console.WriteLine(gs.Player.MatchStats.JSON);
                Console.WriteLine();
                Console.WriteLine("Map.JSON");
                Console.WriteLine(gs.Map.JSON);
                Console.WriteLine();
                Console.WriteLine("Player.JSON");
                Console.WriteLine(gs.Player.JSON);
                Console.WriteLine();

                match.team = gs.Player.Team.ToString();
                Console.WriteLine($"player team: {match.team}");
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

                match.minutes_played = matchTime.MatchTotal.Minutes;
                match.round_win_team = gs.Round.WinTeam.ToString();
                Console.WriteLine($"winning team: {match.round_win_team}");

                // reset counter to help track a round change
                counter = 0;

                try
                {
                    DbInterface.SaveToDb(match, matchStats);
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