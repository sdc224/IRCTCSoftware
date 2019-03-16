﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddStation.xaml.cs" company="SDCWORLD">
//   Sourodeep Chatterjee
// </copyright>
// <summary>
//   Interaction logic for AddStations.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BusinessLogicWPF.View.Admin.UserControls
{
    using System;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using BusinessLogicWPF.Annotations;
    using BusinessLogicWPF.Helper;
    using BusinessLogicWPF.Model;

    /// <summary>
    /// Interaction logic for AddStations.XAML
    /// </summary>
    public partial class AddStation : UserControl
    {
        /// <summary>
        /// The regex.
        /// </summary>
        [NotNull]
        private static readonly Regex Regex = new Regex("[^0-9]+"); // regex that matches disallowed text

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:BusinessLogicWPF.View.Admin.UserControls.AddStation" /> class.
        /// </summary>
        public AddStation()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The is text allowed.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsTextAllowed([NotNull] string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return !Regex.IsMatch(text);
        }

        /// <summary>
        /// The text OTP preview text input.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxPinCodePreviewTextInput([NotNull] object sender, [NotNull] TextCompositionEventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            e.Handled = !IsTextAllowed(e.Text);
        }

        /// <summary>
        /// The text OTP on pasting.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxPinCodeOnPasting([NotNull] object sender, [NotNull] DataObjectPastingEventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text ?? throw new InvalidOperationException()))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// The button accept on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonAcceptOnClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TextBoxZoneName.Text)
                || string.IsNullOrWhiteSpace(this.TextBoxDivisionName.Text)
                || string.IsNullOrWhiteSpace(this.TextBoxStationCode.Text)
                || string.IsNullOrWhiteSpace(this.TextBoxStationName.Text)
                || string.IsNullOrWhiteSpace(this.TextBoxPinCode.Text))
            {
                MessageBox.Show("Please fill up all the fields!");
                return;
            }

            if (this.TextBoxPinCode.Text.Length < 6)
            {
                MessageBox.Show("Pin Code must have SIX digits!");
                return;
            }
            
            var s = MessageBox.Show(
                "Are you sure you want to continue? You cannot undo this operation",
                "Question",
                MessageBoxButton.YesNo);
            
            if (s == MessageBoxResult.No)
            {
                return;
            }

            var station = new Station
                              {
                                  RailwayDivision = this.TextBoxDivisionName.Text,
                                  StationCode = this.TextBoxStationCode.Text,
                                  StationName = this.TextBoxStationName.Text,
                                  StationPinCode = Convert.ToInt32(this.TextBoxPinCode.Text)
                              };

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (o, args) => this.Dispatcher.Invoke(
                () =>
                    {
                        this.ProgressBar.Visibility = Visibility.Visible;
                        StaticDbContext.ConnectFireStore?.AddCollectionDataAsync(
                            station,
                            "Root",
                            "Stations",
                            "StationDetails",
                            this.TextBoxZoneName.Text,
                            station.RailwayDivision,
                            station.StationCode);
                    });

            backgroundWorker.RunWorkerCompleted += (o, args) =>
                {
                    this.ProgressBar.Visibility = Visibility.Hidden;
                    MessageBox.Show("Data Successfully added");
                };

            try
            {
                backgroundWorker.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}
