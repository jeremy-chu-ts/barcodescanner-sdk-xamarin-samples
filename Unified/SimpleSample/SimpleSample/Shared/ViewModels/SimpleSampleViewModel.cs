using SimpleSample.ViewModels.Abstract;
using Scandit.BarcodePicker.Unified;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Diagnostics;

namespace SimpleSample.ViewModels
{
    public class SimpleSampleViewModel:BaseViewModel
    {
        private string _recognizedCode;

        public string RecognizedCode
        {
            get { return _recognizedCode; }

            set
            {
				if (_recognizedCode != value)
				{
					_recognizedCode = value;
					OnPropertyChanged("RecognizedCode");
				}	
            }
        }

        public SimpleSampleViewModel()
        {
            ScanditService.BarcodePicker.DidScan += OnCodesScanned;
			this.RecognizedCode = "No code scanned";
        }

        private void OnCodesScanned(ScanSession session)
        {
			// this method is invoked from an internal thread. To perform any UI work, you will 
			// need to use Device Device.BeginInvokeOnMainThread(() => ...), to move execution to them 
			// main thread.

			// It is preferred to call stop scanning on the session, because it makes sure that the 
			// scanning immediately stops and no further codes are scanned.
			session.StopScanning();

			var firstCode = session.NewlyRecognizedCodes.First();
			var text = String.Format("{0} ({1})", firstCode.Data, firstCode.SymbologyString.ToUpper());
			this.RecognizedCode = text;

        }

        public ICommand StartScanningCommand => new Command(async () => await StartScanning());

        private async Task StartScanning()
        {
            await ScanditService.BarcodePicker.StartScanningAsync(false);
        }
    }
}
