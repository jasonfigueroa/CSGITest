using System;
using System.Threading;
using CSGSI;
using CSGSI.Nodes;

namespace CSGITest
{
    class Program
    {
        static GameStateListener gsl;
        static UserAuth userAuth;
        static string steamId;
        static void Main(string[] args)
        {
            bool isAuthenticated = false;

            while (isAuthenticated == false)
            {
                Console.Write("username: ");
                string username = Console.ReadLine();

                Console.Write("password: ");
                string password = "";
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey(true);

                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        password += key.KeyChar;
                        Console.Write("*");
                    }
                    else
                    {
                        if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                        {
                            password = password.Substring(0, (password.Length - 1));
                            Console.Write("\b \b");
                        }
                    }
                } while (key.Key != ConsoleKey.Enter);

                Console.WriteLine();

                if (DbInterface.IsValidUser(username, password) == true)
                {
                    userAuth = new UserAuth(username, password);
                    steamId = DbInterface.GetSteamId(userAuth);
                    isAuthenticated = true;
                }
                else
                {
                    Console.Clear();
                    Console.Write("invalid password, please try again ");
                    Dots();
                    Console.Clear();
                }
            }            

            gsl = new GameStateListener(3000);
            gsl.NewGameState += new NewGameStateHandler(OnNewGameState);
            if (!gsl.Start())
            {
                Environment.Exit(0);
            }

            Console.Clear();
            Console.Write("user authenticated ");
            Dots();
            Console.Clear();

            Console.Write("Listening ");
            Dots();
        }

        static void Dots()
        {
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
            Console.Write(".");
            Thread.Sleep(500);
        }

        static Match match = new Match();
        static MatchStats matchStats = new MatchStats();
        static MatchTime matchTime = new MatchTime();

        // helper variables to to help control the flow of the application
        static bool stashed = false;
        static int counter = 0;

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

                    // stored as UTC timestamp
                    match.datetime_start = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    match.map_name = gs.Map.Name;
                }

                Console.Clear();

                // NOTE: application does not account for the player switching teams 
                // and will only post to the db the team the player was on in the final round
                match.team = gs.Player.Team.ToString();

                matchStats.kills = gs.Player.MatchStats.Kills;
                matchStats.assists = gs.Player.MatchStats.Assists;
                matchStats.deaths = gs.Player.MatchStats.Deaths;
                matchStats.mvps = gs.Player.MatchStats.MVPs;
                matchStats.score = gs.Player.MatchStats.Score;

                Console.WriteLine($"counter: {counter}");
                Console.WriteLine($"team: {match.team}");
                Console.WriteLine($"kills: {matchStats.kills}");
                Console.WriteLine($"assists: {matchStats.assists}");
                Console.WriteLine($"deaths: {matchStats.deaths}");
                Console.WriteLine($"mvps: {matchStats.mvps}");
                Console.WriteLine($"score: {matchStats.score}");
            }

            // the stashed flag is needed because without it the program will try 
            // go save the match stats multiple times for one match
            if (gs.Map.Phase == MapPhase.GameOver && stashed == false)
            {
                Console.Clear();

                stashed = true;
                matchTime.MatchStop = DateTime.Now;
                matchTime.MatchTotal = matchTime.MatchStop - matchTime.MatchStart;

                match.minutes_played = matchTime.MatchTotal.Minutes;
                match.round_win_team = gs.Round.WinTeam.ToString();

                // reset counter to help track a round change
                counter = 0;

                try
                {
                    DbInterface.SaveToDb(userAuth, match, matchStats);
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