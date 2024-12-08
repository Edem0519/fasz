using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Beadando_Szenzorhalozat
{
    class Sensorok
    {
        //privát adattagok az egységbezárás és az adatok védelme miatt
        private int homerseklet, valami, azon;
        //hogy elérje a többi osztály és lehessen dolgozni velük, tulajdonság függvények létrehozása
        public int Homerseklet
        {
            get; set;
        }
        public int Valami
        {
            get; set;
        }
        public int Azon
        {
            get; set;
        }
        //konstruktor
        public Sensorok(int azon, int homerseklet, int valami)
        {
            Azon = azon;
            Homerseklet = homerseklet;
            Valami = valami;
        }
        public override string ToString() //hogy kiírja a konkrét mérési eredményeket, az ellenőrző kiíratás során
        {
            return $"Azonosító: {Azon}, Hőmérséklet: {Homerseklet}°C, Valami: {Valami}";
        }
    }
    internal class Program
    {
        static List<Sensorok> list = new List<Sensorok>();
        static void Results()
        {
            Random rnd = new Random(); //Random number generator
                                       //Ebben tároljuk a sensorokat
            int i = 0; //ezzel léptetjük és állítjuk a ciklust meg
            int x = 0; //azonositó counter basically
            while (i < 20) //20 sensor példány készítése
            {
                list.Add(new Sensorok(x + 1, rnd.Next(30, 100), rnd.Next(30, 100))); //objektum példányosítás és hozzáadása a listához
                i++;
                x++;
            }
            /*foreach (var sensorok in list) //ellenőrzés képpen
            {
                Console.WriteLine(sensorok);
            }*/
        } //Mérési adatok generálása - majd fájlból olvasás fog kelleni!!!
        static void LINQ_1()
        {
            Console.WriteLine("A mérőpontok és azok eredményei ahol a hőmérséklet 50°C fölé ment...");
            var result = from m in list
                         where m.Homerseklet > 50
                         select m;
            foreach (var m in result)
                Console.WriteLine(m);
        } //1. linq lekérdezés - ez rendben van...
        static void JSON()
        {
            string json = JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented);
            StreamWriter sw = new StreamWriter("json_adatok.txt"); //mérési adatok => JSON fájl
            sw.WriteLine(json); sw.Flush(); sw.Close();
        } //JSON fájl
        static void Main(string[] args)
        {
            Results();
            byte valasztas; //ez a változó a menürendszer kulcs eleme, ez lesz a fh. választsása
            Console.WriteLine("Kérem válasszon az alábbi lehetőségek közül!\n\t1. A mérési adatok kiíratása egy JSON fájlba\n\t2. Mérőpontok kiíratása ahol a biztonságos szint fölé (50°C) ment a hőmérséklet\n\t3. (blank)\n\t4. (blank)\n\t0. Kilépés"); //
            do
            {
                do //ellenőrzött beolvasás - ide esemény?
                {
                    Console.Write("Adja meg a választását az alábbi lehetőségek közül:");
                } while (!byte.TryParse(Console.ReadLine(), out valasztas));
                switch (valasztas) //switchekkel, menürendszer
                {
                    case 0: //Kilépés
                        break;
                    case 1:
                        JSON(); //JSON meghívása
                        Console.WriteLine("Megtörtént a json fájlba írás...");
                        break;
                    case 2:
                        //LINQ_1 meghívása
                        LINQ_1();
                        break;
                    case 3:
                        //linq2 - Átlagolja a hőmérsékleteket
                        var atlag = list.Average(x => x.Homerseklet);
                        Console.Write("A hőméréskletek átlaga:" + atlag + "\n");
                        break;
                    case 4:
                        //linq3

                        Console.WriteLine("itt egy LINQ lekérdezés lesz");
                        break;
                    default:
                        Console.WriteLine("Ilyen sorszámú lehetőség nincs!");
                        break;
                }
            } while (valasztas != 0);
        }
    }
}
