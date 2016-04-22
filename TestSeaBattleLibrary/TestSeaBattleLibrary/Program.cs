using SeaBattleLibrary;
using System;

namespace TestSeaBattleLibrary
{
    class Program
    {
        const int N = 10;

        static void Main(string[] args)
        {
            for (int a = 0; a < 10; a++)
            {
                Bot bot = new Bot();
                bot.SetShips();

                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        Console.Write(bot.Map[i, j] + "\t");
                    }
                    Console.WriteLine();
                }

                Console.ReadLine();
            }
        }
    }
}
