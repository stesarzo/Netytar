using Newtonsoft.Json;
using CsvHelper;
using Netytar.Settings;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System;

namespace Netytar
{
    internal class SavingCalibration
    {
        public const string DEFAULTFILENAME = "Calibration.json";


        private string SettingsFilename { get; set; }

        public void SaveCalibration(string min, string max)
        {


            string json = JsonConvert.SerializeObject(new { Minimo = min, Massimo = max });
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DEFAULTFILENAME);
            File.WriteAllText(path, json);


        }

        public (string,string) LoadCalibration()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DEFAULTFILENAME);
            string json = File.ReadAllText(path);

            // Deserializza il JSON in variabili
            dynamic data = JsonConvert.DeserializeObject(json);

            string min = data.Minimo;
            string max = data.Massimo;

            return (min, max);
            
        }
    }
}

