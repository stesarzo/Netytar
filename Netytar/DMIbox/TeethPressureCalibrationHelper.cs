using NITHdmis.NithSensors;
using NITHdmis.Utils;
using NITHdmis.Mappers;
using System.IO;

namespace Netytar.DMIbox.NithBSBehaviors
{
    public class TeethPressureCalibrationHelper
    {
        public  bool MinSet { get; set; } = false;
        public  bool MaxSet { get; set; } = false;

        public double CalibrateMinValue { get; set; } = 1024;
        public double CalibrateMaxValue { get; set; } = 0;
        
        private SegmentMapper CalibrateMaps;
        public double Calibration(string valueBase, double valueProportional)
        {

            double input = 0.0;

            if (MaxSet == true && MinSet == true)
            {
                CalibrateMaps = new SegmentMapper(CalibrateMinValue, CalibrateMaxValue, 0.00, 100.00);
                //string value = nithData.GetArgument(NithArguments.press).Value.Base;
                input = CalibrateMaps.Map(double.Parse(valueBase));
            }
            else
            {
                // Per tirare fuori i valori di pressione, si usa questo metodo (nithData.GetValue). Siccome va fatto un parsing a double, è meglio mettere CultureInfo.InvariantCulture per specificare in che formato è il numero (hai presente che in America si usa il punto, mentre in Italia la virgola per i decimali...) 
                input = valueProportional;
            }
            return input;
        }
    }
}
