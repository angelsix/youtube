using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new instance of our number guesser class
            var numberGuesser = new NumberGuesser();

            // Change the default maximum number to 200
            numberGuesser.MaximumNumber = 200;

            // Ask the user to think of a number
            numberGuesser.InformUser();

            // Discover the number the user is thinking of
            numberGuesser.DiscoverNumber();

            // Announce the results
            numberGuesser.AnnounceResults();
        }
    }
}
