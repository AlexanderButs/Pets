using System;

using Microsoft.Owin.Hosting;

namespace Pets
{
    /// <summary>
    /// Thease problems are not solved:
    /// 1. Parallel changing pet attributes from PetLifetimeService and WebApi. Didn't want to over complicated solution and emulate database CAS algorythm.
    /// 2. Parallel user creation with the same name.
    /// 3. Solution with PetLifetimeService need to be more designed, because for big amount of data this is not a proper solution.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var baseAddress = "http://localhost:9000/";

            using (WebApp.Start<Startup>(baseAddress))
            {
                Console.WriteLine($"Listening on {baseAddress}");
                Console.WriteLine($"Visit {baseAddress}swagger");
                Console.ReadLine();
            }
        }
    }
}