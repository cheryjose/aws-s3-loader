using System;
using System.Collections.Generic;
using System.Text;

namespace aws_s3_loader
{
    class Utils
    {
        private static void PromptMessage(Action success, Action failure)
        {

            ReadInputAndAction(success, failure);
            Console.WriteLine("Press enter to exit or 0 to try again!!!");

            Console.Read();
        }

        public static void ReadInputAndAction(Action success, Action failure)
        {
            int option = 0;

            do
            {
                Console.WriteLine("Please enter your option. 1 - for success scenario, 2 - for failure scenario, 3 - to exit");
                int.TryParse(Console.ReadLine(), out option);

                if (option == 1)
                {
                    success();
                }
                else if (option == 2)
                {
                    failure();
                }
                else if (option == 3)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid entry please try again !!!");
                }
            } while (option != 3);
        }
    }
}
