﻿// Copyright 2016 Scandit AG
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

using System;
using ExtendedSample.Helpers;
using Scandit.BarcodePicker.Unified;
using Scandit.BarcodePicker.Unified.Abstractions;

using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace ExtendedSample
{
	public partial class SettingsPage : ContentPage
	{
		private IBarcodePicker _picker;
		private ScanSettings _scanSettings;

		public SettingsPage()
		{
			InitializeComponent();

			_picker = ScanditService.BarcodePicker;
			_scanSettings = _picker.GetDefaultScanSettings();

			initializeGuiElements();

			// restore the currently used settings to those found in
			// the permanent storage.
			updateScanSettings();
			updateScanOverlay();
		}

		private void initializeGuiElements()
		{
			// Adds all the symbology Switches (including inverse Switches)
			// to SymbologySection
			bool isMsiPlesseyThrough = false;
			foreach (string setting in Convert.settingToSymbologies.Keys)
			{
				addSymbologySwitches(setting, isMsiPlesseyThrough ? 0 : 1);
				if (Convert.settingToSymbologies[setting][0] == Symbology.MsiPlessey)
				{
					isMsiPlesseyThrough = true;
				}
			}
			initializeMsiPlesseyChecksumPicker();

			// Initialize Feedback
			initializeSwitch(BeepCell, Settings.BeepString);
			initializeSwitch(VibrateCell, Settings.VibrateString);

			// Initialize Torch
			initializeSwitch(TorchButtonCell, Settings.TorchButtonString);

			// Initialize Camera (gui-button-settings)
			initializeCameraPicker();

			// Initialize Gui Style Picker
			initializeGuiStylePicker();

			// Initialize Hotspot
			initializeSwitch(RestrictedAreaCell, Settings.RestrictedAreaString);
			initializeSlider(HotSpotHeightSlider, Settings.HotSpotHeightString);
			initializeSlider(HotSpotWidthSlider, Settings.HotSpotWidthString);
			initializeSlider(HotSpotYSlider, Settings.HotSpotYString);

			// Initialize ViewFinder
			initializeSlider(ViewFinderPortraitWidth, Settings.ViewFinderPortraitWidthString);
			initializeSlider(ViewFinderPortraitHeight, Settings.ViewFinderPortraitHeightString);
			initializeSlider(ViewFinderLandscapeWidth, Settings.ViewFinderLandscapeWidthString);
			initializeSlider(ViewFinderLandscapeHeight, Settings.ViewFinderLandscapeHeightString);
		}

		// Adds a new switch (two incase the symbology has an inverse) to SymbologySection
		void addSymbologySwitches(String settingString, int offset)
		{
			// add switch for regular symbology
			SwitchCell cell = new SwitchCell { Text = Convert.settingToDisplay[settingString] };
			initializeSwitch(cell, settingString);
			SymbologySection.Insert(SymbologySection.Count - offset, cell);

			if (Helpers.Settings.hasInvertedSymbology(settingString))
			{
				string invString = Helpers.Settings.getInvertedSymboloby(settingString);

				// add switch for inverse symbology
				SwitchCell invCell = new SwitchCell { Text = Convert.settingToDisplay[invString] };
				initializeSwitch(invCell, invString);
				SymbologySection.Insert(SymbologySection.Count - offset, invCell);

				// Gray out the inverse symbology switch if the regular symbology is disabled
				invCell.IsEnabled = Settings.getBoolSetting(settingString);
				cell.OnChanged += (object sender, ToggledEventArgs e) =>
				{
					invCell.IsEnabled = e.Value;
				};
			}
		}

		// Bind an existing Slider to the permanent storage
		// and the currently active settings
		private void initializeSlider(Slider slider, string setting)
		{
			slider.Value = Settings.getDoubleSetting(setting);

			slider.ValueChanged += (object sender, ValueChangedEventArgs e) =>
				{
					Settings.setDoubleSetting(setting, e.NewValue);
					updateScanOverlay();
					updateScanSettings();
				};
		}

		// Bind an existing Slider to the permanent storage
		// and the currently active settings
		private void initializeIntSlider(Slider slider, string setting)
		{
			slider.Value = Settings.getIntSetting(setting);
			slider.ValueChanged += (object sender, ValueChangedEventArgs e) =>
				{
					int newValue = (int) Math.Round(e.NewValue);
					slider.Value = newValue;
					Settings.setIntSetting(setting, newValue);
					updateScanOverlay();
					updateScanSettings();
				};
		}

		// Bind an existing switch cell to the permanent storage
		// and the currently active settings
		private void initializeSwitch(SwitchCell cell, string setting)
		{
			cell.On = Settings.getBoolSetting(setting);

			cell.OnChanged += (object sender, ToggledEventArgs e) =>
				{
					Settings.setBoolSetting(setting, e.Value);
					updateScanOverlay();
					updateScanSettings();
				};
		}

		// Bind an the msi plessey checksum picker to the permanent storage
		// and the currently active settings
		private void initializeMsiPlesseyChecksumPicker()
		{
			MsiPlesseyChecksumPicker.SelectedIndex =
			        Convert.msiPlesseyChecksumToIndex[Settings.getStringSetting(Settings.MsiPlesseyChecksumString)];

			MsiPlesseyChecksumPicker.SelectedIndexChanged += (object sender, EventArgs e) =>
				{
					Settings.setStringSetting(
						Settings.MsiPlesseyChecksumString,
						Convert.indexToMsiPlesseyChecksum[MsiPlesseyChecksumPicker.SelectedIndex]);
					updateScanOverlay();
					updateScanSettings();
				};
		}

		// Bind an the camera picker to the permanent storage
		// and the currently active settings		
		private void initializeCameraPicker()
		{
			CameraButtonPicker.SelectedIndex =
                  Convert.cameraButtonToIndex[Settings.getStringSetting(Settings.CameraButtonString)];

			CameraButtonPicker.SelectedIndexChanged += (object sender, EventArgs e) =>
				{
					Settings.setStringSetting(
						Settings.CameraButtonString,
				        Convert.indexToCameraButton[CameraButtonPicker.SelectedIndex]);
					updateScanOverlay();
					updateScanSettings();
				};
		}

		// Bind an the GUIStyle picker to the permanent storage
		// and the currently active settings
		private void initializeGuiStylePicker()
		{
			GuiStylePicker.SelectedIndex =
				Convert.guiStyleToIndex[Settings.getStringSetting(Settings.GuiStyleString)];

			GuiStylePicker.SelectedIndexChanged += (object sender, EventArgs e) =>
				{
					Settings.setStringSetting(
						Settings.GuiStyleString,
						Convert.indexToGuiStyle[GuiStylePicker.SelectedIndex]);
					updateScanOverlay();
					updateScanSettings();
				};
		}

		// reads the values needed for ScanSettings from the Settings class
		// and applies them to the Picker
		void updateScanSettings()
		{
			foreach (string setting in Convert.settingToSymbologies.Keys)
			{
				bool enabled = Settings.getBoolSetting(setting);
				foreach (Symbology sym in Convert.settingToSymbologies[setting])
				{
					_scanSettings.EnableSymbology(sym, enabled);
					if (Settings.hasInvertedSymbology(setting))
					{
						_scanSettings.Symbologies[sym].ColorInvertedEnabled = Settings.getBoolSetting(
							Settings.getInvertedSymboloby(setting));
					}
				}
			}

			_scanSettings.MaxNumberOfCodesPerFrame = 2;

			_scanSettings.Symbologies[Symbology.MsiPlessey].Checksums = 
				Convert.msiPlesseyChecksumToScanSetting[Settings.getStringSetting(Settings.MsiPlesseyChecksumString)];

			_scanSettings.RestrictedAreaScanningEnabled = Settings.getBoolSetting(Settings.RestrictedAreaString);
			if (_scanSettings.RestrictedAreaScanningEnabled)
			{
				Double HotSpotHeight = Settings.getDoubleSetting(Settings.HotSpotHeightString);
				Double HotSpotWidth = Settings.getDoubleSetting(Settings.HotSpotWidthString);
				Double HotSpotY = Settings.getDoubleSetting(Settings.HotSpotYString);

				Rect restricted = new Rect(0.5f - HotSpotWidth * 0.5f, HotSpotY - 0.5f * HotSpotHeight,
										   HotSpotWidth, HotSpotHeight);

				_scanSettings.ScanningHotSpot = new Scandit.BarcodePicker.Unified.Point(
						0.5, Settings.getDoubleSetting(Settings.HotSpotYString));
				_scanSettings.ActiveScanningAreaPortrait = restricted;
				_scanSettings.ActiveScanningAreaLandscape = restricted;
			}
			_picker.ApplySettingsAsync(_scanSettings);
		}

		// reads the values needed for ScanOverlay from the Settings class
		// and applies them to the Picker
		void updateScanOverlay()
		{
			_picker.ScanOverlay.BeepEnabled = Settings.getBoolSetting(Settings.BeepString);
			_picker.ScanOverlay.VibrateEnabled = Settings.getBoolSetting(Settings.VibrateString);
			_picker.ScanOverlay.TorchButtonVisible = Settings.getBoolSetting(Settings.TorchButtonString);

			_picker.ScanOverlay.ViewFinderSizePortrait = new Scandit.BarcodePicker.Unified.Size(
				(float)Settings.getDoubleSetting(Settings.ViewFinderPortraitWidthString),
				(float)Settings.getDoubleSetting(Settings.ViewFinderPortraitHeightString)
			);
			_picker.ScanOverlay.ViewFinderSizeLandscape = new Scandit.BarcodePicker.Unified.Size(
	   			(float)Settings.getDoubleSetting(Settings.ViewFinderLandscapeWidthString),
	   			(float)Settings.getDoubleSetting(Settings.ViewFinderLandscapeHeightString)
			);

			_picker.ScanOverlay.CameraSwitchVisibility =
				Convert.cameraToScanSetting[Settings.getStringSetting(Settings.CameraButtonString)];

			_picker.ScanOverlay.GuiStyle =
				Convert.guiStyleToScanSetting[Settings.getStringSetting(Settings.GuiStyleString)];
		}

	}
}

