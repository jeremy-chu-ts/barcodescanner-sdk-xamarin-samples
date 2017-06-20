// Copyright 2016 Scandit AG
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Xamarin.Forms;
using Scandit.BarcodePicker.Unified;
using Scandit.BarcodePicker.Unified.Abstractions;
using System.Linq;

namespace ExtendedSample
{
public partial class ExtendedSamplePage : ContentPage
{
	private string ean13 = "";

	void OnDidScan(ScanSession session)
	{

		// guaranteed to always have at least one element, so we don't have to 
		// check the size of the NewlyRecognizedCodes array.
		var firstCode = session.NewlyRecognizedCodes.First();

		if (session.NewlyRecognizedCodes.Count() == 1 &&
			firstCode.Symbology == Symbology.Ean13 &&
			firstCode.Data.Length == 13)
		{
			ean13 = firstCode.Data;
			return;
		}


		if (firstCode.Symbology != Symbology.Ean13 && firstCode.Symbology != Symbology.TwoDigitAddOn)
		{
			ean13 = "";
		}

		var barcodeAsString = "";

		if (firstCode.Symbology == Symbology.TwoDigitAddOn && !string.IsNullOrEmpty(ean13))
		{
			barcodeAsString = ean13 + firstCode.Data;
		}
		else
		{
			ean13 = "";
			foreach (Barcode bc in session.NewlyRecognizedCodes)
			{
				barcodeAsString += bc.Data;
			}
		}



		if (barcodeAsString.Length < 15) return;

		var message = string.Format("Code Scanned:\n {0}\n({1})", barcodeAsString,
									firstCode.SymbologyString.ToUpper());



		// Because this event handler is called from an scanner-internal thread, 
		// you must make sure to dispatch to the main thread for any UI work.
		Device.BeginInvokeOnMainThread(() => this.ResultLabel.Text = message);
		session.StopScanning();
	}

	async void OnSettingsClicked(object sender, System.EventArgs e)
	{
		await Navigation.PushAsync(_settingsPage);
	}

	async void OnScanButtonClicked(object sender, System.EventArgs e)
	{

		// The scanning behavior of the barcode picker is configured through scan
		// settings. We start with empty scan settings and enable a very generous
		// set of symbologies. In your own apps, only enable the symbologies you
		// actually need.
		await _picker.StartScanningAsync(false);
	}

	IBarcodePicker _picker;

		SettingsPage _settingsPage;
		public static string appKey = @"AW7KFTtOIZJtEL1w3CfgX/cet/ClBNVHUSbsoIhLiX3ye6OnYXnJ83cZnFe1BQN0KGR+2WFekZ4mX6Ikq1g1ds8kilDHY+CJdTir/bVd9/RIBdr3wSkoPvaq8khmLr+ja6J3iuhyoSSTWl+S0m/2Pr3mlczgeW8eTDqu+HNOPJTXEUoT9pla5TYktQaUZmhGvr4kZ6Vh3yDin/aBPKW9GmDdwEeKoeuxmfqnBGuHbGcsC38lcKLDjs7O7JZEpC7AnWGtWpv0YktzY6jTtWyKBx5QitL9/kmTIkskOm3ejDT8lqhj4Ca94YjGH4zsj9O7M7soMD/FCCTNg5W4+gsb/VQEIsyFts9zZ9x8CZN0AmZJ+ApzuP9jmqTkXBgwu+XmeYLn6byPz90I8ppqOUUikHEnyO2QDsLi7yNJf2DJ5Ebkubw34KDGyFMkeSQVvo/EbXkAfm9pvN9bGEBaCd7jl1uVK2IRmGV4j+Qd83V965FIJGO4ZHf+lJrVj6itqq721UPxGsw0QIX+PajrjIV4oUQyA6T8fl6nM3OAlyjza/YjU4y+29r5VzXDQ2nXGOPM2ZujsuxKE/mfArOoDhDTiUZuqTo+zX/OzKSrMWqF+sw03nHKuP1KvDDD9dVkIDSvF+UNCrmMly0HxxBLivF91znNmoXOiZNRlJdpRHPN8l/gTW1s6dXD/guQRJ3KTtDBODH6ehswELLXqp1hPei3m1/A8SDmMJ0yV6QKx8AfZrRkIFJI2TfpEE5alJVcgbRG6SO5FA4YxkNZ5CukBxiZRabGnT4702W/0A==";

//public static string appKey = @"AdjqdiROPmRtGM2PdB2IVH8nv9iOBz79y1+mALwd/R2QL0E8Km7iFUp4MalHOkqEQGjttsFKQCoTSogO/l7hOmhFCbl9YQr+vBwDgxpGH4LQYTpYhzy+/Q9OGvHSdygUhkKRnuxjTWlMGqFFPG51RldVo+j6Z0+x0k2qiNhNC9coAhujFFLXC/hR6t3+bKywyVcO5OBT/51fWS4bJCoxcGxvyT2ccBC5W2apyXQgY8OuQ6E5jWjOqxhrTE73W83OmGEKT3RY9KpMfUQHp3yzmHZw0YktI2/ZbSQRDSJfWBooFFgfeQpAC7R2q4PwDsCc8hgs1+wQ0B9YuZHKgvztrEoVuKjZfbG/XHoZc9E/3zJxAe0OA9K7Zzu0zQkekIslf+5LzJb6z0ZGa/zSZhcKu7ZxFMZxD5zLrBbNganDBpzNTLV1SJLnYl5r5OHKC9oxBbx9jKZ8PiGrteNVmeWa2MmDCSy3JIX1UVbYXDsYYFOatyJyMkV2ZfgO2/WY4SWJZRsT8NeWOe7U+igH7y7cEZo/pW/8th6yQQh1kGms6U8KjQPKjxr05IVTXx+DIFpv/Aoa5c0ZhPP//M4OGNBdef3K/KsDNpBk76rIIRry/FRtkPNG32mjukTgNQt1HtyIZsizDuFgp4TSnQQJikCwZ+qFuQ//EO1UL9/nYANLCzwpjUWHwu+9d48KcuwsYbMx3BglbrXO4nv8HWs24Jz2Fpb2cw9Nlfu/wnxAUcpN0K+Uqhq6TdBhmeHwXuyZ027JoB8ybkiCgF7UNCnNlbdFSQW3t76ON9OhhRhqBEM4W1Y1DAH+Hb2UXepgKRqhXMA90Y+vzGFbHrcgcNp+fa2iJmD/8rMDALO6WVHP/c0b3qCSP/3HRcEwrwgzSToxxsqxVnAgdIsjeP1OdY9lgRLYmvVy8I9egWoGPHWWDShgzeVktcgL7rC2TZ+C6cNTzjfoic2xUpcaXuOg/oSHqCruQoooUUI=";

		public ExtendedSamplePage()
		{
			InitializeComponent();
			// must be set before you use the picker for the first time.
			ScanditService.ScanditLicense.AppKey = appKey;
			// retrieve the actual native barcode picker implementation. The concrete implementation 
			// will be different depending on the platform the code runs on.
			_picker = ScanditService.BarcodePicker;
			// will get invoked whenever a code has been scanned.
			_picker.DidScan += OnDidScan;

			// set up the settings page
			_settingsPage = new SettingsPage();
		}
	}
}

