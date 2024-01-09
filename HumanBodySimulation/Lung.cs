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

        static double setco2gehalt( double co2alt)
        {
            //CO2 Gehalt nach Ablauf der 100ms
            return 100 * 2 + co2alt; //Wert zwei zu Konstante ändern!
        }

        static double Atmungsfrequenz(double co2gehalt )
        {

            return 0;

        }

        public void init(Dictionary<string, string> parameters)
        {
            parameters["sauerstoffgehalt"] = "21.0"; // in Prozent
            parameters["co2gehalt"] = "0.04"; // in Prozent
            parameters["atmungsfrequenz"] = "12.0"; // Atemzüge pro Minute
            parameters["tidalvolumen"] = "0.5"; //  Liter pro Atemzug
            parameters["lungenvolumen"] = "4.5"; // Gesamtvolumen Lunge in Litern

        }
        public void update(int n, Dictionary<string, string> parameters)
        {
            // Parameter
            double sauerstoffgehalt = double.Parse(parameters["sauerstoffgehalt"]); // in Prozent
            double co2gehalt = double.Parse(parameters["co2gehalt"]); // in Prozent
            //double atmungsfrequenz = double.Parse(parameters["atmungsfrequenz"]); // Atemzüge pro Minute
            double tidalvolumen = double.Parse(parameters["tidalvolumen"]); //  Liter pro Atemzug
            double lungenvolumen = double.Parse(parameters["lungenvolumen"]); // Gesamtvolumen Lunge in Litern

            int tick = n / 100; // das muss abgerundet werden

            for(int i = 0; i< tick; i++)
            {
                double co2neu = setco2gehalt( co2gehalt);
                double atemfrequenz = Atmungsfrequenz(co2neu);
                    //Sauerstoffaufnahme
                double sauerstoffaufnahme = BerechneSauerstoffaufnahme(sauerstoffgehalt, tidalvolumen, atemfrequenz);
                   // Luftstrom
                double luftstrom = BerechneLuftstrom(tidalvolumen, atemfrequenz);
                   // CO2-Abgabe
                double co2abgabe = BerechneCO2Abgabe(co2gehalt, tidalvolumen, atemfrequenz);
            }

            
               
        }
    }
}
