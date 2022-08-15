namespace WordleAPI.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Model model = new Model();
            await model.RunGame();
        }
    }
}