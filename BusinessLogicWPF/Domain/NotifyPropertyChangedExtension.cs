﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyPropertyChangedExtension.cs" company="SDCWORLD">
//   Sourodeep Chatterjee
// </copyright>
// <summary>
//   Defines the NotifyPropertyChangedExtension type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BusinessLogicWPF.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using BusinessLogicWPF.Properties;

    /// <summary>
    /// The notify property changed extension.
    /// </summary>
    public static class NotifyPropertyChangedExtension
    {
        /// <summary>
        /// The mutate verbose.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        /// <param name="raise">
        /// The raise.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <typeparam name="TField">
        /// T means Entity
        /// </typeparam>
        public static void MutateVerbose<TField>(
            [CanBeNull] this INotifyPropertyChanged instance,
            ref TField field,
            TField newValue,
            [CanBeNull] Action<PropertyChangedEventArgs> raise,
            [CanBeNull] [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TField>.Default.Equals(field, newValue))
            {
                return;
            }

            field = newValue;
            raise?.Invoke(new PropertyChangedEventArgs(propertyName));
        }
    }
}
