namespace WordleAPI.App
{
    class GreenLetter : Letter
    {
        public GreenLetter(char letter) : base(letter) {}
        public override void DisplayLetter()
        {
            System.Console.ForegroundColor = System.ConsoleColor.Green;
            System.Console.Write(letter);
            System.Console.ResetColor();
        }
    }
}