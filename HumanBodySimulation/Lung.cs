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

        public double randomizer(double con_parameter, int seed)
        {
            Random random = new Random();

            int percent =(int) random.Next(0, seed);
            int sign=(int) random.Next(0, 1);
            return con_parameter * (1 + sign * percent / 100);
        }

        public double breathingperiod(double pa_co2_alv)
        {
            double k = randomizer(1,5);
            int ref_co2 = 40;
            double breathingfrequency = 12 + k * (pa_co2_alv - ref_co2);
            return 1000 / (breathingfrequency / 60); // in ms

        }

        public double SPO2Calc(double pa_o2_blood_alv)
        {
            double n_haemo = 2.7; //Hill coefficient for haemoglobin
            double k_d = randomizer(26,10); //Dissociation constant representing temperature, pH, co2 factor regarding o2 saturation //pCO2 impacts k_d value
            double PAO2n = Math.Pow(pa_o2_blood_alv, n_haemo); //must be in mmHg
            double k_d_n = Math.Pow(k_d, n_haemo);
            return PAO2n / (k_d_n + PAO2n);
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
      
            parameters["pa_o2_alv"] = "100";                        // partialpressure oxygen in alveolar gas, in mmHg ->updated by us 
            parameters["pa_co2_alv"] = "40";                        // partialpressure Co2 in alveolar gas, in mmHg -> updated by us

            parameters["pa_o2_blood_ven"] = "";                   // partialpressure O2 in venous blood in mmHg -> this value is updated by us
            parameters["pa_co2_blood_ven"] = "";                    // partialpressure Co2 in venous blood in mmHg -> this value is updated by us
            parameters["pa_o2_blood_alv"] = "40";                   // partialpressure O2 in venous blood in mmHg -> this value is updated by us
            parameters["pa_co2_blood_alv"] = "46";                    // partialpressure Co2 in venous blood in mmHg -> this value is updated by us
            parameters["pa_o2_blood_art"] = "40";                  // partialpressure O2 in arterial blood low o2-> this value is updated by other organs
            parameters["pa_co2_blood_art"] = "46";                  // partialpressure Co2 in arterial blood high co2 -> this value is updated by other organs

            parameters["breathing_frequency"] = "12.0";             // baseline breathing frequency in 1/min -> updated by us
            parameters["tidalvolume"] = "250";                     // liters per breath, insp and exp -> spontaneous breathing constant oscillation
            parameters["residual_functional_volume"] = "3000";       // maximum volume of lung in liters remains in lung after breathing -> constant for now

            parameters["SPO2Heartbeat"] = "97";

        }
        public void update(int n, Dictionary<string, string> parameters)
        {
            // Parameter
            double time_next_breath = double.Parse(parameters["time_next_breath"]);
            int time_contact = int.Parse(parameters["time_contact"]);

            double pa_o2_alv = double.Parse(parameters["pa_o2_alv"]);
            double pa_co2_alv = double.Parse(parameters["pa_co2_alv"]);
            double pa_o2_insp = 160; // partialpressure oxygen in inspiratory gas, in mmHg -> constant sorrounding air
            double pa_Co2_insp = 0.25; // partialpressure Co2 in inspiratory gas, in mmHg ->constant sorroundung air

            double pa_o2_blood_alv = double.Parse(parameters["pa_o2_blood_alv"]);
            double pa_co2_blood_alv = double.Parse(parameters["pa_co2_blood_alv"]);
            double pa_o2_blood_art = double.Parse(parameters["pa_o2_blood_art"]);
            double pa_co2_blood_art = double.Parse(parameters["pa_co2_blood_art"]);
            double SPO2Heartbeat = double.Parse(parameters["SPO2Heartbeat"]);

            double tidalvolume = double.Parse(parameters["tidalvolume"]);
            double residual_functional_volume = double.Parse(parameters["residual_functional_volume"]);


            //double D = numeric value here; // diffusion constant of oxygen/ co2 -> research pls
            double A = 70;                // contact area of blood and alveoli of healthy adult in m²
            double dx = 0.000001;               // thickness of blood gas barrier healthy adult in µm
            double p_ges = 760;            //surrounding pressure in mmHg / equal at alveolar level
            double D_o2 = 0.000004167; // diffusion constant of oxygen/ co2 
            double D_co2 = 23 * D_o2;           // diffusion of Co2 ist 23 times higher 
            double Hb = 15; // Haemoglobin concentration [g/100 ml]

            //check for breath, update alveolar partial pressures if breath happened, update time to next breathing event

            time_next_breath = time_next_breath - n;

            if (time_next_breath <= 0) {

                time_next_breath = breathingperiod(pa_co2_alv); // ToDo implement breathing frequency / updated breathing frequency -> new time according to our calc

                pa_o2_alv = set_pa_O2_alv_breath(tidalvolume, residual_functional_volume, pa_o2_alv, pa_o2_insp);
                pa_co2_alv = set_pa_Co2_alv_breath(tidalvolume, residual_functional_volume, pa_co2_alv, pa_Co2_insp);
            }


            //check for blood exchange, update partial pressures for blood in lung if heartbeat happened, update time to next heartbeat

            time_contact = time_contact - n;

            if (time_contact <= 0) {

                time_contact = 1000; //ToDo implement heratbeat -> new time according to actual bpm

                SPO2Heartbeat = (int)Math.Round(SPO2Calc(pa_o2_blood_alv) * 100);

                parameters["pa_o2_blood_ven"] = pa_o2_blood_alv.ToString();
                parameters["pa_co2_blood_ven"] = pa_co2_blood_alv.ToString();

                pa_o2_blood_art = randomizer(40,10);
                pa_co2_blood_art = randomizer(46,10);

                pa_o2_blood_alv = pa_o2_blood_art;
                pa_co2_blood_alv = pa_co2_blood_art; 
            }

            //calculate exchanged volumes of gas based on magic formula, ficks law, since last update from main function

            double exchanged_volume_o2 = (D_o2 * A * ((pa_o2_alv - pa_o2_blood_alv) / 760) / dx) * n / 1000;      //volume flow -> in m³ (*n / 1000 due to n is in ms) of O2 and Co2
            double exchanged_volume_co2 = (D_co2 * A * ((pa_co2_blood_alv- pa_co2_alv) / 760) / dx) * n / 1000;

            // calculate new partialpressures in blood + lung - update blood values - update 

            //determine oxygen volume in lung and blood
            double o2_volume_alv = residual_functional_volume * (pa_o2_alv / p_ges);
            double co2_volume_alv = residual_functional_volume * (pa_co2_alv / p_ges);


            double SO2 = SPO2Calc(pa_o2_blood_alv);
            double o2_volume_alv_blood = SO2 * Hb * 1.39; // volume diluted in blood - based on oxygen saturation PAO2*0.0003 is neglected in the formula
            double co2_volume_alv_blood = Math.Exp((0.396 * Math.Log(pa_co2_blood_alv)) + 2.38);

            //calc new partialpressures+

       
            pa_o2_alv = (o2_volume_alv - exchanged_volume_o2) * p_ges / residual_functional_volume;
            pa_co2_alv = (co2_volume_alv + exchanged_volume_co2) * p_ges / residual_functional_volume;
            SO2 = (o2_volume_alv_blood + exchanged_volume_o2) / (Hb * 1.39);  // new oxygen saturation
            pa_o2_blood_alv = 26 * Math.Pow((SO2 / (1 - SO2)),(1 / 2.7));
            pa_co2_blood_alv = Math.Exp((Math.Log(co2_volume_alv_blood - exchanged_volume_co2) - 2.38) / 0.396);

            if (pa_o2_alv - pa_o2_blood_alv < 0)
            {
                pa_o2_blood_alv = pa_o2_alv; //avoids overshooting:pa_o2_blood_alv must not be higher than pa_o2_alv
            }

            if (pa_co2_alv - pa_co2_blood_alv > 0)
            {
                pa_co2_blood_alv = pa_co2_alv; //avoids overshooting:pa_o2_blood_alv must not be higher than pa_o2_alv
            }


            //set partial pressures of O2 and Co2 / update parameter dictionary
            parameters["pa_o2_alv"] = pa_o2_alv.ToString();
            parameters["pa_co2_alv"] = pa_co2_alv.ToString();

            parameters["pa_o2_blood_alv"] = pa_o2_blood_alv.ToString();
            parameters["pa_co2_blood_alv"] = pa_co2_blood_alv.ToString();

            parameters["pa_o2_blood_art"] = pa_o2_blood_art.ToString();
            parameters["pa_co2_blood_art"] = pa_co2_blood_art.ToString();


            parameters["time_next_breath"] = time_next_breath.ToString();
            parameters["time_contact"] = time_contact.ToString();

            parameters["SPO2Heartbeat"] = SPO2Heartbeat.ToString();


            //validation --> plot values

            string csvFilePath = "D:/LungValueValidation.csv";

            using (StreamWriter writer = new StreamWriter(csvFilePath, true))
            {               
                writer.Write(parameters["SPO2Heartbeat"]);

                writer.Write(';');

                writer.Write(parameters["time_next_breath"]);

                writer.Write(';');

                writer.Write(parameters["time_contact"]);

                writer.Write(';');

                writer.Write(parameters["pa_o2_alv"]);

                writer.Write(';');

                writer.Write(parameters["pa_o2_blood_alv"]);

                writer.Write(';');

                writer.Write(parameters["pa_co2_alv"]);

                writer.Write(';');

                writer.Write(parameters["pa_co2_blood_alv"]);

                //writer.Write(',');

                writer.WriteLine();
            }

            return;
        }
    }
}

