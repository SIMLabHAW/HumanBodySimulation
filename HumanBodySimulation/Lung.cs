using System;
using System.Collections.Generic;
using System.IO;
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
            parameters["time_next_breath"] = "5000";                // time to next breathing event in ms ->updated by us
            parameters["time_contact"] = "1000";                    // contct time between blood and lung in ms -> depends on heartbeat

            parameters["pa_o2_insp"] = "160";                       // partialpressure oxygen in inspiratory gas, in mmHg -> constant sorrounding air
            parameters["pa_co2_insp"] = "0.25";                     // partialpressure Co2 in inspiratory gas, in mmHg ->constant sorroundung air
            parameters["pa_o2_exp"] = "";                           // partialpressure oxygen in exspiratory gas, in mmHg  ->irrelevant?
            parameters["pa_co2_exp"] = "";                          // partialpressure Co2 in exspratory gas, in mmHg -> irrelevant?
            parameters["pa_o2_alv"] = "100";                        // partialpressure oxygen in alveolar gas, in mmHg ->updated by us 
            parameters["pa_co2_alv"] = "40";                        // partialpressure Co2 in alveolar gas, in mmHg -> updated by us

            parameters["pa_o2_blood_ven"] = "";                   // partialpressure O2 in venous blood in mmHg -> this value is updated by us
            parameters["pa_co2_blood_ven"] = "";                    // partialpressure Co2 in venous blood in mmHg -> this value is updated by us
            parameters["pa_o2_blood_alv"] = "40";                   // partialpressure O2 in venous blood in mmHg -> this value is updated by us
            parameters["pa_co2_blood_alv"] = "46";                    // partialpressure Co2 in venous blood in mmHg -> this value is updated by us
            parameters["pa_o2_blood_art"] = "40";                  // partialpressure O2 in arterial blood low o2-> this value is updated by other organs
            parameters["pa_co2_blood_art"] = "46";                  // partialpressure Co2 in arterial blood high co2 -> this value is updated by other organs

            parameters["breathing_frequency"] = "12.0";             // baseline breathing frequency in 1/min -> updated by us
            parameters["tidalvolume"] = "0.35";                     // liters per breath, insp and exp -> spontaneous breathing constant oscillation
            parameters["residual_functional_volume"] = "3.0";       // maximum volume of lung in liters remains in lung after breathing -> connstant for now

            parameters["HTV"] = "5.0";                              // heart time volume in L/min, calc from bpm and volume of heart -maybe?

            parameters["SPO2"] = "97";                              //initial value in %

        }
        public void update(int n, Dictionary<string, string> parameters)
        {
            // Parameter
            int time_next_breath = int.Parse(parameters["time_next_breath"]);
            int time_contact = int.Parse(parameters["time_contact"]);

            double pa_o2_alv = double.Parse(parameters["pa_o2_alv"]);
            double pa_co2_alv = double.Parse(parameters["pa_Co2_alv"]);
            double pa_o2_insp = double.Parse(parameters["pa_o2_insp"]);
            double pa_Co2_insp = double.Parse(parameters["pa_Co2_insp"]);

            double pa_o2_blood_alv = double.Parse(parameters["pa_o2_blood_alv"]);
            double pa_co2_blood_alv = double.Parse(parameters["pa_co2_blood_alv"]);
            double pa_o2_blood_art = double.Parse(parameters["pa_o2_blood_art"]);
            double pa_co2_blood_art = double.Parse(parameters["pa_co2_blood_art"]);

            double pa_Co2_blood_in = double.Parse(parameters["pa_Co2"]);

            double tidalvolume = double.Parse(parameters["tidalvolume"]);
            double residual_functional_volume = double.Parse(parameters["residual_functional_volume"]);


            //double D = numeric value here; // diffusion constant of oxygen/ co2 -> research pls
            double A = 100;                // contact area of blood and alveoli of healthy adult in m²
            double dx = 0.3*0.000001;               // thickness of blood gas barrier healthy adult in µm
            double p_ges = 760;            //surrounding pressure in mmHg / equal at alveolar level
        

            //check for breath, update alveolar partial pressures if breath happened, update time to next breathing event

            time_next_breath = time_next_breath - n;

            if (time_next_breath <= 0) {

                time_next_breath = 5000; // ToDO implement breathing frequency / updated breathing frequency -> new time according to our calc

                pa_o2_alv = set_pa_O2_alv_breath(tidalvolume, residual_functional_volume, pa_o2_alv, pa_o2_insp);
                pa_co2_alv = set_pa_Co2_alv_breath(tidalvolume, residual_functional_volume, pa_co2_alv, pa_Co2_insp);
            }


            //check for blood exchange, update partial pressures for blood in lung if heartbeat happened, update time to next heartbeat

            time_contact = time_contact - n;

            if (time_contact <= 0) {

                time_contact = 1000; //ToDo implement heratbeat -> new time according to actual bpm

                double pa_o2_blood_ven = pa_o2_blood_alv;
                double pa_co2_blood_ven = pa_co2_blood_alv;
                pa_o2_blood_alv = pa_o2_blood_art;
                pa_co2_blood_alv = pa_co2_blood_art;

            }

            //calculate exchanged volumes of gas based on magic formula, ficks law, since last update from main function

            double D = 10; // ??? Define a proper value for D
            double exchanged_volume_o2 = Math.Abs((-D * A * (pa_o2_alv - pa_o2_blood_alv) / dx) * n);       //volume flow -> in m³ (*n / 1000 to equalize times) of O2 and Co2
            double exchanged_volume_co2 = Math.Abs((-D * A * (pa_co2_alv - pa_co2_blood_alv) / dx) * n);

            // calculate new partialpressures in blood + lung - update blood values - update 

            double o2_volume_alv = residual_functional_volume * (pa_o2_alv / p_ges); //V1
            pa_o2_alv = (pa_o2_alv * o2_volume_alv / (o2_volume_alv - exchanged_volume_o2));  //p2=p1*V1/V2



            //ToDo find reasonable values to insert into Ficks Law -> update of partialpressures blood and alv gas


            //ToDO calculate haemoglobin saturation -> s curve describes connection between partial pressure
            double n_haemo = 2.8; //Hill coefficient for haemoglobin
            double k_d = 28; //Dissociation constant representing temperature, pH, co2 factor regarding o2 saturation //pCO2 impacts k_d value
            double PAO2n = Math.Pow(pa_o2_alv, n_haemo); //must be in mmHg
            double k_d_n = Math.Pow(k_d, n_haemo);
            double SPO2 = PAO2n / (k_d_n + PAO2n);
            int SPO2Percent = (int) Math.Round(SPO2*100);
         

            //set partial pressures of O2 and Co2 / update parameter dictionary
            parameters["pa_o2_alv"] = pa_o2_alv.ToString();
            parameters["SPO2"] = SPO2Percent.ToString();

            //validation --> plot values

            string csvFilePath = "lungensimulation.csv";
   
            using (StreamWriter writer = new StreamWriter(csvFilePath, true))
            {
                writer.WriteLine(parameters["SPO2"]);
            }
                       
            return;
        }
    }
}

