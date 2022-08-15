using System.Text.Json;
using System.Net.Http;

namespace WordleAPI.App
{
    class Model
    {
        private string word;
        private uint numGuess;
        private const uint maxGuess = 6;
        private Letter[] validity;
        private bool won;
        // private IRepository repo;
        public HashSet<string> wordList;
        public User? currUser;
        public const uint WORD_SIZE = 5;
        public static readonly HttpClient client = new HttpClient();
        public static string uri = "https://localhost:7295/api/Users/";

        public Model(uint maxGuess=6) {
            wordList = new HashSet<string>(File
                .ReadLines(@"./wordlist.txt")
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line)), StringComparer.OrdinalIgnoreCase);
            this.numGuess = 0;
            this.validity = new Letter[WORD_SIZE];
            this.won = false;
        }

        public async Task RunGame()
        {
            // Setup current user.
            string name = "";
            bool userError = true;
            bool newUser = false;

            while (userError)
            {
                try
                {
                    View.AskName();
                    name = Controller.CheckUser();
                    string response = await client.GetStringAsync(uri + name);
                    currUser = JsonSerializer.Deserialize<User>(response);
                    // currUser = repo.GetUser(Controller.CheckUser());
                    View.DeletePrompt();
                    if (Controller.YesNo())
                    {
                        await client.DeleteAsync(uri + name);
                    } 
                    else 
                    { 
                        userError = false;
                    }
                }
                catch (HttpRequestException ex)
                {
                    currUser = new User(name);
                    currUser.guessNums = new int[] { 0, 0, 0, 0, 0, 0 };
                    userError = false;
                    newUser = true;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    View.OutOfRangeMessage(ex);
                }
            }

            

            // Game Loop.
            View.StartPrompt();
            bool play = true;
            do
            {
                // Variable initialization that needs to be done before every new game.
                SelectWord();
                won = false;
                numGuess = 0;
                Array.Clear(validity, 0, validity.Length);
            
                while (numGuess < maxGuess && !won)
                {
                    MainLoop();
                }

                // Stats get pushed to the database after every game.
                UpdateStats(won);
                // repo.InsertUser(currUser);
                if (newUser)
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, uri);
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    request.Content = new StringContent(JsonSerializer.Serialize(currUser));
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    await client.SendAsync(request);
                }
                else
                {
                    var request = new HttpRequestMessage(HttpMethod.Put, uri);
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    request.Content = new StringContent(JsonSerializer.Serialize(currUser));
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    await client.SendAsync(request);
                }
                View.DisplayStats(currUser);

                // Asking to play again.
                View.EndPrompt(won);
                bool endError = true;
                while (endError) {
                    try 
                    {
                        play = Controller.YesNo();
                        endError = false;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        View.OutOfRangeMessage(ex);
                    }
                }
            } while (play);
            
        }

        private void MainLoop()
        {
            // Check validity of guess, then evaluate.
            View.MainPrompt(numGuess);
            try
            {
                string guess = Controller.GetGuess(wordList);
                CheckGuess(guess);
                numGuess++;
                View.DisplayResult(validity);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                View.OutOfRangeMessage(ex);
            }
        }

        private void SelectWord()
        {
            Random random = new Random();
            word = wordList.ElementAt(random.Next(wordList.Count));
            // FOR DEMO
            // word = "water";
            System.Console.WriteLine(word);
        }
        private void CheckGuess(string guess)
        {
            // Marks if letter was already used to grant yellow status to a letter in the guess.
            bool[] marked = {false, false, false, false, false};
            
            for (int i = 0; i < WORD_SIZE; i++)
            {
                // Letter is in correct spot.
                if (guess[i] == word[i]) {
                    validity[i] = new GreenLetter(guess[i]);
                }
                else
                {
                    // See if letter has an unmarked version in main word.
                    string copy = word;
                    bool set = false;
                    int index = -1;
                    while (!set)
                    {
                        index = copy.IndexOf(guess[i], index + 1);
                        if (index != -1) {
                            if (guess[index] != word[index] && !marked[index])
                            {
                                validity[i] = new YellowLetter(guess[i]);
                                marked[index] = true;
                                set = true;
                                break;
                            }
                        } 
                        else {
                            validity[i] = new RedLetter(guess[i]);
                            set = true;
                            break;
                        }
                    }
                }
            }

            // Check if guess was a win.
            for (int i = 0; i < WORD_SIZE; i++)
            {
                if (!(validity[i] is GreenLetter)) {
                    return;
                }
            }
            won = true;
        }

        private void UpdateStats(bool won)
        {
            if (won)
            {
                currUser.wins++;
                currUser.streak++;
                currUser.guessNums[numGuess - 1]++;
            }
            else
            {
                currUser.losses++;
                currUser.streak = 0;
            }
        }

    }
}