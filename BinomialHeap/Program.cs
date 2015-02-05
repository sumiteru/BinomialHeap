using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinomialHeap
{
    class Program
    {
        public static BinomialHeap heap = new BinomialHeap();
        static void Main(string[] args)
        {
            string option = string.Empty;

            while(true)
            {
                Console.Clear();
                DisplayMenu();
                option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        Mode("Podaj liczbę do wstawienia: ", heap.insert);
                        break;
                    case "2":
                        Mode("Podaj liczbę do usunięcia: ", heap.deleteNode);
                        break;
                    case "3":
                        Mode("Podaj ścieżkę do pliku z kopcem: ", heap.readHeapFromFile);
                        break;
                    case "4":
                        heap.saveHeapToFile("file.out");
                        break;
                    case "9":
                        return;
                    default:
                        break;
                }
            }
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("1. Dodaj element");
            Console.WriteLine("2. Usuń element");
            Console.WriteLine("3. Wczytaj kopiec z pliku");
            Console.WriteLine("4. Zapisz");
            Console.WriteLine("9. Wyjście");
        }

        private static void Mode(string message, Predicate<string> function)
        {
            string text = string.Empty;
            while(true)
            {
                Console.Clear();
                Console.WriteLine(heap);
                Console.WriteLine("Wpisz Q aby wyjść.");
                Console.Write(message);
                text = Console.ReadLine();
                if (text.ToLower() == "q")
                    break;

                function(text);
            }
        }
    }
}
