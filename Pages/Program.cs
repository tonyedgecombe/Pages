using System;
using System.Linq;

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
                Console.WriteLine("Pages http://www.domain.com");
                return;
            }

            var finder = new PageFinder(new Uri(args[0], UriKind.Absolute));
            foreach(var page in finder.FindPages().OrderBy(uri => uri))
            {
                Console.WriteLine(page);
            }
        }
    }
}
