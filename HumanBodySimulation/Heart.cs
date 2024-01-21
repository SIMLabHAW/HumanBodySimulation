using System;
using System.Collections.Generic;
using System.Globalization;

namespace HumanBodySimulation
{
    // Define the Organ base class
    public abstract class Organ
    {
        public abstract void Update(int n);
    }

    // Implement the Heart class
    public class Heart : Organ, IOrgan
    {
        // Properties as private fields
        private double _heartRate = 70; // Default heart rate
        private double _strokeVolume = 70; // Default stroke volume
        private double _lastBloodPumped;
        private double _averageSPO2;
        private double _minSystolicPressure;
        private double _maxSystolicPressure;
        private double _minDiastolicPressure;
        private double _maxDiastolicPressure;
        private double _biggestO2Desaturation;
        private double _averageO2Desaturation;
        private double _maximumHR = 100; // Default maximum heart rate
        private double _minimumHR = 60; // Default minimum heart rate

        // Existing methods
        public double GetBloodPumped()
        {
            return _lastBloodPumped;
        }

        public override void Update(int n)
        {
            double beatsSinceLastUpdate = (_heartRate / 60) * (n / 1000.0);
            _lastBloodPumped = beatsSinceLastUpdate * _strokeVolume;
        }

        // New method to calculate Heart Rate Variability (HRV)
        public double GetHeartRateVariability()
        {
            return _maximumHR - _minimumHR;
        }

        // Method to initialize heart parameters
        public void init(Dictionary<string, string> parameters)
        {
            foreach (var key in parameters.Keys)
            {
                if (double.TryParse(parameters[key], NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValue))
                {
                    switch (key)
                    {
                        case "HeartRate": _heartRate = parsedValue; break;
                        case "AverageSPO2": _averageSPO2 = parsedValue; break;
                            // Add cases for other parameters as needed
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid input for {key}. Using default value.");
                }
            }
        }

        public void update(int n, Dictionary<string, string> parameters)
        {
            // Update logic for Heart
        }

        // Calculated properties as methods
        public double MeanAtrialPressure()
        {
            return ((2 * _minDiastolicPressure) + _maxSystolicPressure) / 3;
        }

        public double PulsePressure()
        {
            return _maxSystolicPressure - _minDiastolicPressure;
        }

        public double AverageBloodPressure()
        {
            return _minDiastolicPressure == 0 ? 0 : _maxSystolicPressure / _minDiastolicPressure;
        }

        public double RatePressureProduct()
        {
            return _heartRate * _maxSystolicPressure;
        }

        public double AverageHR()
        {
            return (_maximumHR + _minimumHR) / 2;
        }

        public double SystolicBloodPressureRange()
        {
            return _maxSystolicPressure - _minSystolicPressure;
        }

        public double DiastolicBloodPressureRange()
        {
            return _maxDiastolicPressure - _minDiastolicPressure;
        }

        public double OxygenSaturationVariability()
        {
            return _averageSPO2 - (_averageSPO2 * (_biggestO2Desaturation / 100));
        }
    }
}
