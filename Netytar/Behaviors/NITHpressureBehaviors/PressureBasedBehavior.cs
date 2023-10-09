using NITHdmis.NithSensors;
using NITHdmis.Utils;
using System.Windows.Markup;

namespace Netytar.DMIbox.NithBSBehaviors
{
    
    // Un behavior INithSensorBehavior che serve a leggere l'input da un sensore di pressione.
    // // Tale input va a regolare i note on/note off, la channel pressure MIDI, e ad introdurre un po' di modulation (quest'ultima in maniera meno che proporzionale all'input)
    // L'input arriva in forma |press=val/max (esempio |press=200/800). NithModule fa automaticamente la proporzione, e il valore arriva qui in percentuale (nell'es. 25% <- 200/800)
    internal class PressureBasedBehavior : INithSensorBehavior
    {
       
        // Una serie di valori interni che servono per "aggiustare" il valore rilevato (costanti di moltiplicazione)
        private ValueMapperDouble inputMapper; // Questo serve a fare una piccola proporzione: MIDI va in 127esimi, quindi quel 100 di massima deve diventare un 127)
        private float pressureMultiplier; // Moltiplicatore di channel pressure
        private float modulationDivider; // Divisore per gestire la modulation
        private float sensitivity; // Sensibilità generale, un valore che va semplicemente a moltiplicare l'input
        private _NetytarControlModes associatedControlMode; // Control mode associato
        private SegmentMapper CalibrateMaps;

        // Doppia soglia per gestire i Note On/Note Off. Se la pressure sale oltre a 5 -> note on. Se scende sotto a 1 -> Note Off
        private const float UPPERTHRESH = 5f;
        private const float LOWERTHRESH = 1f;

        /// <summary>
        /// Initializes the behavior, with some settings (which include default values).
        /// </summary>
        /// <param name="modulationMultiplier"></param>
        /// <param name="pressureMultiplier">Channel pressure will be multiplied by this constant</param>
        /// <param name="sensitivity">Applies a pre-conversion multiplication constant to the input</param>
        public PressureBasedBehavior(_NetytarControlModes associatedControlMode, float modulationMultiplier = 0.125f, float pressureMultiplier = 1f, float sensitivity = 1f)
        {
            this.modulationDivider = modulationMultiplier;
            this.pressureMultiplier = pressureMultiplier;
            this.sensitivity = sensitivity;
            this.associatedControlMode = associatedControlMode;

            // Inizializzazione del mapper
            inputMapper = new ValueMapperDouble(100, 127);
        }

        // A questo metodo vengono passati tutti i valori in arrivo da NithModule, ogni volta che ne arriva uno, in forma di NithSensorData.
        public void HandleData(NithSensorData nithData)
        {
             
            // (Se è attivo il breath sensor in Netytar)
            if(R.UserSettings.NetytarControlMode == associatedControlMode)
            {
                // Se effettivamente è arrivato un valore di pressione...
                if (nithData.GetArgument(NithArguments.press) != null)
                {
                    double input = 0.0;
                    if (R.MaxSet == true && R.MinSet == true)
                    {
                        CalibrateMaps = new SegmentMapper(R.CalibrateMinValue, R.CalibrateMaxValue, 0.00, 100.00);
                        string value = nithData.GetArgument(NithArguments.press).Value.Base;
                        input = CalibrateMaps.Map(double.Parse(value));

                    }
                    else
                    {
                        // Per tirare fuori i valori di pressione, si usa questo metodo (nithData.GetValue). Siccome va fatto un parsing a double, è meglio mettere CultureInfo.InvariantCulture per specificare in che formato è il numero (hai presente che in America si usa il punto, mentre in Italia la virgola per i decimali...) 
                        input = nithData.GetArgument(NithArguments.press).Value.Proportional;
                    }

                    input = input * sensitivity; // Applicazione Sensitivity
                    if (input > 100) input = 100; // Soglia massima di 100
                    R.NDB.BreathValue = input; // Passo al modulo di logica di strumento il valore "raw" di input, per fare aggiornare un indicatore grafico che "ragiona" in centesimi
                    input = inputMapper.Map(input); // Faccio fare la proporzione a 127 per MIDI
                    R.NDB.Pressure = (int)(input * pressureMultiplier); // Aggiorno la logica dello strumento cambiando la MIDI channel pressure
                    R.NDB.Modulation = (int)(input / modulationDivider); // " cambiando la modulation
                    
                    // Verifica della doppia soglia per vedere se mandare un note on/note off (ovvero quel "Blow" nella logica dello strumento)
                    if ((int)input > UPPERTHRESH && !R.NDB.Blow)
                    {
                        R.NDB.Blow = true;
                    }

                    if ((int)input == LOWERTHRESH)
                    {
                        R.NDB.Blow = false;
                    }
                }
            }
            
        }
    }
}
