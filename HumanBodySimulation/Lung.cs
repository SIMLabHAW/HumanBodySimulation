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

        static int step=100; //zeitvariable für alle Funktionen 

        static double setco2gehalt( double co2alt)
        {
            //CO2 Partialdruck nach Ablauf der 100ms
            return step * 2 + co2alt; //Wert zwei zu Konstante ändern!
        }

        static double setp_alvO2();

        static double f_breath(double paCO2)
        {
            double CO2rise = 0.5; // Beispielwert: Anstieg der Atemfrequenz pro mmHg paCO2
            int baselinefrequency = 12; // Beispielwert: Grundlegende Atemfrequenz im Normalzustand

            //Atemfrequenz beträgt 12 Atemzüge pro Minute, wenn der CO2-Partialdruck bei 40 mmHg liegt.
            return (int)(baselinefrequency + CO2rise * (paCO2 - 40));
        }

        static double BerechneSauerstoffaufnahme(double sauerstoffgehalt, double tidalvolumen, double f_breath, double paCO2)
        {
            double VO2=3,6/f_breath// Sauerstoffaufnahme pro Minute

            return sauerstoffgehalt / 100.0 * tidalvolumen * atmungsfrequenz;

            /*int RQ = 0.9; //Verhältnis von aufgenommenen O2 zu abgegebenem CO2 in l/min
            int pa_inspO2=160; //mmHg Umgebungsluft
            return pa_inspO2 - paCO2/RQ; // pa_alv_O2*/

            
        }

        static double BerechneCO2Abgabe(double co2gehalt, double tidalvolumen, double atmungsfrequenz)
        {
            // CO2-Abgabe pro Minute
            return co2gehalt / 100.0 * tidalvolumen * atmungsfrequenz;
        }

        
        /*static double BerechneLuftstrom(double tidalvolumen, double atmungsfrequenz)
        {
            // Luftstrom pro Minute
            return tidalvolumen * atmungsfrequenz;
        }
        */

 
        public void init(Dictionary<string, string> parameters)
        {
            parameters["sauerstoffgehalt"] = "21.0"; // in Prozent in der LUFT
            parameters["co2gehalt"] = "0.04"; // in Prozent in der LUFT
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

            int tick = n / step; // das muss abgerundet werden

            for(int i = 0; i<= tick; i++)
            {
                double co2neu = setco2gehalt( co2gehalt);
                double atemfrequenz = f_breath(co2neu);
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
