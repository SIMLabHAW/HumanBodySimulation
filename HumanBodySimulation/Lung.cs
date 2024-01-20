/*
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
            return pa_inspO2 - paCO2/RQ; // pa_alv_O2

            
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
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HumanBodySimulation
{
    internal class Lung2 : IOrgan
    {
        public Lung2()
        {

        }

        static double set_pa_O2_alv_breath(double tidalvolume, double residual_functional_volume, double pa_alv_o2, double pa_o2_insp)
        {
            // updates partial pressures of O2 through breathing

            return (residual_functional_volume * pa_alv_o2 + tidalvolume * pa_o2_insp) / (residual_functional_volume + tidalvolume);

        }

        static double set_pa_Co2_alv_breath(double tidalvolume, double residual_functional_volume, double pa_alv_Co2, double pa_Co2_insp)
        {
            //  updates partial pressures of  Co2 through breathing

            return (residual_functional_volume * pa_alv_Co2 + tidalvolume * pa_Co2_insp) / (residual_functional_volume + tidalvolume);

        }

        public void init(Dictionary<string, string> parameters)
        {
            parameters['time_next_breath'] = '5000';                // time to next breathing event in ms ->updated by us
            parameters['time_contact'] = '1000';                    // contct time between blood and lung in ms -> depends on heartbeat

            parameters["pa_o2_insp"] = "160";                       // partialpressure oxygen in inspiratory gas, in mmHg -> constant sorrounding air
            parameters["pa_co2_insp"] = "0.25";                     // partialpressure Co2 in inspiratory gas, in mmHg ->constant sorroundung air
            parameters["pa_o2_exp"] = "";                           // partialpressure oxygen in exspiratory gas, in mmHg  ->irrelevant?
            parameters["pa_co2_exp"] = "";                          // partialpressure Co2 in exspratory gas, in mmHg -> irrelevant?
            parameters["pa_o2_alv"] = "100";                        // partialpressure oxygen in alveolar gas, in mmHg ->updated by us 
            parameters["pa_co2_alv"] = "40";                        // partialpressure Co2 in alveolar gas, in mmHg -> updated by us

            parameters['pa_o2_blood_ven'] = '';                   // partialpressure O2 in venous blood in mmHg -> ths value is updated by us
            parameters['pa_co2_blood_ven'] = '';                    // partialpressure Co2 in venous blood in mmHg -> ths value is updated by us
            parameters['pa_o2_blood_alv'] = '40';                   // partialpressure O2 in venous blood in mmHg -> ths value is updated by us
            parameters['pa_co2_blood_alv'] = '46';                    // partialpressure Co2 in venous blood in mmHg -> ths value is updated by us
            parameters['pa_o2_blood_art'] = '40'                   // partialpressure O2 in arterial blood low o2-> ths value is updated by other organs
            parameters['pa_co2_blood_art'] = '46'                  // partialpressure Co2 in arterial blood high co2 -> ths value is updated by other organs

            parameters["breathing_frequency"] = "12.0";             // baseline breathing frequency in 1/min -> updated by us
            parameters["tidalvolume"] = "0.35";                     // liters per breath, insp and exp -> spontaneous breathing constant oscillation
            parameters["residual_functional_volume"] = "3.0";       // maximum volume of lung in liters remains in lung after breathing -> connstant for now

            parameters['HTV'] = '5.0';                              // heart time volume in L/min, calc from bpm and volume of heart -maybe?

        }
        public void update(int n, Dictionary<string, string> parameters)
        {
            // Parameter
            int time_next_breath = int.Parse(parameters['time_next_breath']);
            int time_contact = double.Parse(parameters['time_contact']);

            double pa_o2_alv = double.Parse(parameters['pa_o2_alv']);
            double pa_co2_alv = double.Parse(parameters['pa_Co2_alv']);
            double pa_o2_insp = double.Parse(parameters['pa_o2_insp']);
            double pa_Co2_insp = double.Parse(parameters['pa_Co2_insp']);

            double pa_o2_blood_alv = double.Parse(parameters['pa_o2_blood_alv']);
            double pa_co2_blood_alv = double.Parse(parameters['pa_co2_blood_alv']);
            double pa_o2_blood_art = double.Parse(parameters['pa_o2_blood_art']);
            double pa_co2_blood_art = double.Parse(parameters['pa_co2_blood_art']);

            double pa_Co2_blood_in = double.Parse(parameters['pa_Co2']);

            double tidalvolume = double.Parse(parameters["tidalvolume");
            double residual_functional_volume = double.Parse(parameters["residual_functional_volume"]);


            double D = numeric value here; // diffusion constant of oxygen/ co2 -> research pls
            double A = 100;                // contact area of blood and alveoli of healthy adult in m²
            double dx = 0.3;               // thickness of blood gas barrier healthy adult in µm
            double p_ges = 760;            //surrounding pressure in mmHg / equal at alveolar level


            //check for breath, update alveolar partial pressures if breath happened, update time to next breathing event

            time_next_breath = time_next_breath - n;

            if time_next_breath <= 0 {

                time_next_breath = 5000; // ToDO implement breathing frequency / updated breathing frequency -> new time according to our calc

                pa_o2_alv = set_pa_O2_alv_breath(tidalvolume, residual_functional_volume, pa_o2_alv, pa_o2_insp);
                pa_co2_alv = set_pa_Co2_alv_breath(tidalvolume, residual_functional_volume, pa_Co2_alv, pa_Co2_insp);

            }


            //check for blood exchange, update partial pressures for blodd in lung if heartbeat happened, update time to next heartbeat

            time_contact = time_contact - n;

            if time_contact <= 0 {

                time_contact = 1000; //ToDo implement heratbeat -> new time according to actual bpm

                double pa_o2_blood_ven = pa_o2_blood_alv;
                double pa_co2_blood_ven = pa_co2_blood_alv;
                pa_o2_blood_alv = pa_o2_blood_art;
                pa_co2_blood_alv = pa_co2_blood_art;

            }

            //calculate exchanged volumes of gas based on magic formula, ficks law, since last update from main function


            double exchanged_volume_o2 = abs((-D * A * (pa_o2_alv - pa_o2_blood_alv) / dx) * n);       //volume flow -> in m³ (*n / 1000 to equalize times) of O2 and Co2
            double exchanged_volume_co2 = abs((-D * A * (pa_co2_alv - pa_co2_blood_alv) / dx) * n);

            // calculate new partialpressures in blood + lung - update blood values - update 

            double o2_volume_alv = residual_functional_volume * (pa_o2_alv / p_ges);
            pa_o2_alv = (pa_o2_alv * o2_volume_alv / (o2_volume_alv - exchanged_volume_o2);



            //ToDo find reasonable values to insert into Ficks Law -> update of partialpressures blood and alv gas


            //ToDO calc hemoglobin saturation -> s curve describes connection between partial pressure


            //set partial pressures of O2 and Co2 / update parameter dictionary


            return;
        }
    }
}

