using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HumanBodySimulation
{
    internal class Lung : IOrgan
    {
        public Lung()
        {
            // Parameter
            double sauerstoffgehalt = 21.0; // in Prozent
            double co2gehalt = 0.04; // in Prozent
            double atmungsfrequenz = 12.0; // Atemzüge pro Minute
            double tidalvolumen = 0.5; //  Liter pro Atemzug
            double lungenvolumen = 6.0; // Gesamtvolumen Lunge in Litern
                                        

            for (int atemzug = 1; atemzug <= 2; atemzug++) // Atemperiode = 2 Atemzüge (Einatmen + Ausatmen)
            {
                // Sauerstoffaufnahme
                double sauerstoffaufnahme = BerechneSauerstoffaufnahme(sauerstoffgehalt, tidalvolumen, atmungsfrequenz);
                // Luftstrom
                double luftstrom = BerechneLuftstrom(tidalvolumen, atmungsfrequenz);
                // CO2-Abgabe
                double co2abgabe = BerechneCO2Abgabe(co2gehalt, tidalvolumen, atmungsfrequenz);
                // Simulationsergebnisse
                Console.WriteLine($"Atemzug {atemzug}: Sauerstoffaufnahme = {sauerstoffaufnahme} L/min, Luftstrom = {luftstrom} L/min, CO2-Abgabe = {co2abgabe} L/min");
                // Wartezeit zwischen Atemzügen
                System.Threading.Thread.Sleep(1000 / (int)atmungsfrequenz * 60);
            }
        }
        static double BerechneSauerstoffaufnahme(double sauerstoffgehalt, double tidalvolumen, double atmungsfrequenz)
        {
            // Sauerstoffaufnahme pro Minute
            return sauerstoffgehalt / 100.0 * tidalvolumen * atmungsfrequenz;
        }
        static double BerechneLuftstrom(double tidalvolumen, double atmungsfrequenz)
        {
            // Luftstrom pro Minute
            return tidalvolumen * atmungsfrequenz;
        }
        static double BerechneCO2Abgabe(double co2gehalt, double tidalvolumen, double atmungsfrequenz)
        {
            // CO2-Abgabe pro Minute
            return co2gehalt / 100.0 * tidalvolumen * atmungsfrequenz;
        }

        public void init(Dictionary<string, string> parameters)
        {
            parameters["sauerstoffgehalt"] = "21.0"; // in Prozent
            parameters["co2gehalt"] = "0.04"; // in Prozent
            parameters["atmungsfrequenz"] = "12.0"; // Atemzüge pro Minute
            parameters["tidalvolumen"] = "0.5"; //  Liter pro Atemzug
            parameters["lungenvolumen"] = "6.0"; // Gesamtvolumen Lunge in Litern

        }
        public void update(int n, Dictionary<string, string> parameters)
        {
            //update kommentar commit test.....
            int i =0;

        }
    }
}
