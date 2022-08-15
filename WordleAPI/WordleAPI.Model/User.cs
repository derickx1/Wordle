namespace WordleAPI.Model
{
    public class User
    {
        public string name { get; set; }
        public int streak { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public int[] guessNums { get; set; }

        public User() { }

        public User(string name, int streak = 0, int wins = 0, int losses = 0, int[]? guessNums = null)
        {
            this.name = name;
            this.streak = streak;
            this.wins = wins;
            this.losses = losses;
            if (guessNums == null || guessNums.Length == 0)
            {
                guessNums = new int[] { 0, 0, 0, 0, 0, 0 };
            }
            else
            {
                this.guessNums = guessNums;
            }
        }
    }
}