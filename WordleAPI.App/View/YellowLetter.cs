namespace WordleAPI.App
{
    class YellowLetter : Letter
    {
        public YellowLetter(char letter) : base(letter) {}
        public override void DisplayLetter()
        {
            System.Console.ForegroundColor = System.ConsoleColor.Yellow;
            System.Console.Write(letter);
            System.Console.ResetColor();
        }
    }
}