using System;

namespace Links
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Syntax:");
                Console.WriteLine("");
                Console.WriteLine("Links http://www.domain.com");
                return;
            }

            var finder = new LinkFinder(new Uri(args[0], UriKind.Absolute));
            finder.FindLinks();
        }
    }
}
