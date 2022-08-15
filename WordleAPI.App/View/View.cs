namespace WordleAPI.App
{
    static class View
    {
        public static void StartPrompt()
        {
            System.Console.WriteLine("Welcome to Wordle!");
        }

        public static void MainPrompt(uint numGuess)
        {
            System.Console.Write($"{numGuess + 1}/6 ");
        }

        public static void EndPrompt(bool won)
        {
            if (won) {
                System.Console.WriteLine("Congratulations, Play Again? (y/n)");
            }
            else
            {
                System.Console.WriteLine("Sorry, Try Again? (y/n)");
            }
        }
        
        public static void DisplayResult(Letter[] validity)
        {
            for (int i = 0; i < Model.WORD_SIZE; i++)
            {
                validity[i].DisplayLetter();
            }
            System.Console.WriteLine();
        }

        public static void OutOfRangeMessage(ArgumentOutOfRangeException ex)
        {
            Console.WriteLine(ex.Message);
        }

        public static void DisplayStats(User user)
        {
            Console.WriteLine($"Wins: {user.wins}, Losses: {user.losses}, Current Streak: {user.streak}");
            Console.WriteLine("|Guess 1|Guess 2|Guess 3|Guess 4|Guess 5|Guess 6|");
            Console.WriteLine($"    {user.guessNums[0]}       {user.guessNums[1]}       {user.guessNums[2]}       {user.guessNums[3]}       {user.guessNums[4]}       {user.guessNums[5]}");
        }

        public static void AskName()
        {
            Console.WriteLine("Please enter your username.");
        }

        public static void DeletePrompt()
        {
            Console.WriteLine("Would you like to delete your account? (y/n)");
        }
    }
}