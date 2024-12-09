﻿using Newtonsoft.Json;
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
        private int homerseklet, paratartalom, folyoszint, tartalyszint, allapot,azon;
        //hogy elérje a többi osztály és lehessen dolgozni velük, tulajdonság függvények létrehozása
        public int Homerseklet
        {get; set;}
        public int Paratartalom
        {get; set;}
        public int Folyoszint
        { get; set;}
        public int Tartalyszint
        { get; set;}
        public int Azon
        {get; set;}
        //konstruktor
        public Sensorok(int azon, int homerseklet, int paratartalom, int folyoszint, int tartalyszint)
        {
            Azon = azon;
            Homerseklet = homerseklet;
            Paratartalom = paratartalom;
            Folyoszint = folyoszint;
            Tartalyszint = tartalyszint;
        }
        public override string ToString() //hogy kiírja a konkrét mérési eredményeket, az ellenőrző kiíratás során
        {
            return $"Azonosító: {Azon}, Hőmérséklet: {Homerseklet}°C, Páratartalom: {Paratartalom}%, Folyószint: {Folyoszint}m, Tartályszint: {Tartalyszint}cm";
        }
    }
    internal class Program
    {
        public static event EventHandler JSON_FILE;
        static void OnFileWritten(EventArgs e)
        {
            // Eseménykezelő meghívása
            JSON_FILE?.Invoke(null, e);
        }
        static void FileWrittenHandler(object sender, EventArgs e)
        {
            Console.WriteLine("A fájl sikeresen ki lett írva!");
        }
        static Program()
        {
            // Regisztráljuk az eseménykezelőt
            JSON_FILE += FileWrittenHandler;
        }
        static List<Sensorok> list = new List<Sensorok>();
        static void Results()
        {
            string filePath = "adatok.txt"; // Szöveges fájl elérési útja
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath); // Beolvassa a fájl sorait
                foreach (var line in lines)
                {
                    var parts = line.Split(' '); // Szóközök mentén szétválasztja az adatokat
                    if (parts.Length == 5)
                    {
                        // Adatok konvertálása és objektum hozzáadása a listához
                        int azon = int.Parse(parts[0]);
                        int homerseklet = int.Parse(parts[1]);
                        int paratartalom = int.Parse(parts[2]);
                        int folyoszint = int.Parse(parts[3]);
                        int tartalyszint = int.Parse(parts[4]);

                        list.Add(new Sensorok(azon, homerseklet, paratartalom, folyoszint, tartalyszint));
                    }
                }
                /*foreach (var sensorok in list) //ellenőrzés képpen
                {
                    Console.WriteLine(sensorok);
                }*/
            }
        }//fájl-ból beolvasás
        static void JSON()
        {
            string json = JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented);
            try
            {
                StreamWriter sw = new StreamWriter("json_adatok.txt"); //mérési adatok => JSON fájl
                sw.WriteLine(json);
                sw.Flush();
                sw.Close();

                // Ha sikerült a fájl írása, aktiváljuk az eseményt
                OnFileWritten(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt a fájl írása során: {ex.Message}");
            }
        } //JSON fájl
        static void LINQ_1()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Az alábbi órákban narancssárga riasztás volt érvényben");
            Console.ResetColor();
            var result = from t in list
                         where t.Homerseklet > 35
                         select t;
            foreach (var t in result)
                Console.WriteLine($"Óra: {t.Azon} Hőmérséklet: {t.Homerseklet}");
        } //1. linq lekérdezés - ez rendben van...
        static void LINQ_2()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Az alábbi órákban magas volt a páratartalom:");
            Console.ResetColor();
            var result = from p in list
                         where 60 <= p.Paratartalom
                         select p;
            foreach (var p in result)
                Console.WriteLine($"Óra: {p.Azon} Hőmérséklet: {p.Paratartalom}");
        } //2. linq lekérdezés
        static void LINQ_3() //3. linq lekérdezés
        {
            Console.ForegroundColor= ConsoleColor.Red;
            Console.WriteLine("Az alábbi órákban nagyon magas szárazság volt");
            Console.ResetColor();
            var result = from s in list
                         where 30<s.Homerseklet && 40>s.Paratartalom
                         select s;
            foreach (var s in result)
                Console.WriteLine($"Óra: {s.Azon} Hőmérséklet: {s.Homerseklet} Páratartalom: {s.Paratartalom}");
        }
        static void Main(string[] args)
        {
            Results();
            byte valasztas; //ez a változó a menürendszer kulcs eleme, ez lesz a fh. választsása
            Console.WriteLine("Kérem válasszon az alábbi lehetőségek közül!\n\t1. A mérési adatok kiíratása egy JSON fájlba\n\t2. Az órák és a hozzájuk tartozó hőmérséklet kiíratása ahol narancssárga riasztás volt érvényben\n\t3. Az órák ahol minimum 60% volt a páratartalom\n\t4. (blank)\n\t0. Kilépés"); //
            do
            {
                do //ellenőrzött beolvasás - ide esemény?
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\nAdja meg a választását az alábbi lehetőségek közül:");
                    Console.ResetColor();
                } while (!byte.TryParse(Console.ReadLine(), out valasztas));
                switch (valasztas) //switchekkel, menürendszer
                {
                    case 0: //Kilépés
                        break;
                    case 1:
                        JSON(); //JSON meghívása
                        break;
                    case 2:
                        //LINQ_1 meghívása
                        LINQ_1();
                        break;
                    case 3:
                        //LINQ_2 
                        LINQ_2();
                        break;
                    case 4:
                        //LINQ_3
                        LINQ_3();
                        break;
                    default:
                        Console.WriteLine("Ilyen sorszámú lehetőség nincs!");
                        break;
                }
            } while (valasztas != 0);
        }
    }
}
