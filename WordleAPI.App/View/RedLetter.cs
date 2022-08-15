namespace WordleAPI.App
{
    class RedLetter : Letter
    {
        public RedLetter(char letter) : base(letter) {}
        public override void DisplayLetter()
        {
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            System.Console.Write(letter);
            System.Console.ResetColor();
        }
    }
}