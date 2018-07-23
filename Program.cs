using System;

namespace Back_end_Test_BigData_Company
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please, input your url :");
            String url = Console.ReadLine();

            Console.WriteLine("Please, input the wished recursing depth :");
            uint depth = (uint) Math.Abs(Convert.ToInt16(Console.ReadLine()));
            Page rootPage = new Page(url);

            rootPage.dig(depth);

            Console.WriteLine("Here is the total of non white-space chars among all the paragraphs among all crawled pages : " + rootPage.getParagraphSizeRecurs());
        }
    }
}
