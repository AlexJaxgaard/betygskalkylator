using System;

namespace pw_rewrite
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please determine the size of your new password by entering the desired amount of letters/numbers in your password.");

            string password = Console.ReadLine();

            char[] toArray = password.ToCharArray();
            

            
        }

        private static string[] ReWriter(string[] password);



    }
}
