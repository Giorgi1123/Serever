using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FOR_SERVER
{
    class Program
    {
        static void Main(string[] args)
        {

            GetRequest("http://localhost:3030/");
            

            Console.ReadKey();

        }
        async static Task GetRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        string mycontent = await content.ReadAsStringAsync();
                        Console.WriteLine(mycontent);
                    }
                    
                }
                
            }
        }
    }
}
