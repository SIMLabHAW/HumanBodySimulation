using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanBodySimulation
{
    internal class Lung
    {
        Lung()
        {
            // Simulationsparameter
            double sauerstoffgehalt = 21.0; // in Prozent
            double co2gehalt = 0.04; // in Prozent
            double atmungsfrequenz = 12.0; // Atemzüge pro Minute
            double tidalvolumen = 0.5; // in Litern pro Atemzug
            double lungenvolumen = 6.0; // Gesamtvolumen der Lunge in Litern
                                        // Simulation für eine Atemperiode

            for (int atemzug = 1; atemzug <= 2; atemzug++) // 2 Atemzüge für eine Atemperiode (Einatmen + Ausatmen)
            {
                // Berechnung der Sauerstoffaufnahme
                double sauerstoffaufnahme = BerechneSauerstoffaufnahme(sauerstoffgehalt, tidalvolumen, atmungsfrequenz);
                // Berechnung des Luftstroms
                double luftstrom = BerechneLuftstrom(tidalvolumen, atmungsfrequenz);
                // Berechnung der CO2-Abgabe
                double co2abgabe = BerechneCO2Abgabe(co2gehalt, tidalvolumen, atmungsfrequenz);
                // Ausgabe der Simulationsergebnisse
                Console.WriteLine($"Atemzug {atemzug}: Sauerstoffaufnahme = {sauerstoffaufnahme} L/min, Luftstrom = {luftstrom} L/min, CO2-Abgabe = {co2abgabe} L/min");
                // Wartezeit zwischen den Atemzügen
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
    }
}
