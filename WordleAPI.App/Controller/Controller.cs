namespace WordleAPI.App
{
    static class Controller
    {
        public static string GetGuess(HashSet<string> wordList)
        {
            string? guess = System.Console.ReadLine();
            if (guess == null)
            {
                throw new ArgumentOutOfRangeException(nameof(guess), "Guess cannot be null");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(guess, @"^[a-zA-Z]+$"))
            {
                throw new ArgumentOutOfRangeException(nameof(guess), "Guess must only be alphabetic characters");
            }
            else if (guess.Length != 5)
            {
                throw new ArgumentOutOfRangeException(nameof(guess), "Guess must be a 5-letter word");
            }
            else if (!wordList.Contains(guess))
            {
                throw new ArgumentOutOfRangeException(nameof(guess), "Guess not in word list");
            }
            return guess.ToLower();
        }

        public static bool YesNo()
        {
            string? response = System.Console.ReadLine();
            if (response != null) {
                response = response.ToLower();
            }
            if (response == "y")
            {
                return true;
            }
            else if (response == "n")
            {
                return false;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(response), "Invalid response (y/n)");
            }
        }

        public static string CheckUser()
        {
            string? name = System.Console.ReadLine();
            if (name == null)
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Name cannot be null");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z]+$"))
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Name must only be alphabetic characters");
            }
            else if (name.Length > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Name cannot be longer than 10 characters");
            }

            return name;
        }
    }
}