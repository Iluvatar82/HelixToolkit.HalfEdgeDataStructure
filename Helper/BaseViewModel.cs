using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Helper
{
    /// <summary>
    /// BaseClass for all ViewModel Classes.
    /// Implements the INotifyPropertyChanged Interface.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The PropertyChanged Event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Handler for Changed Values.
        /// </summary>
        /// <param name="propertyName">Name of the changed Property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Actual Setter of Values.
        /// </summary>
        /// <typeparam name="T">Value Class.</typeparam>
        /// <param name="backingField">Event Raiser.</param>
        /// <param name="value">Value.</param>
        /// <param name="propertyName">Name of the changed Property.</param>
        /// <returns>True if successful, false otherwise.</returns>
        protected Boolean SetValue<T>(ref T backingField, T value, params String[] propertyNames)
        {
            if (object.Equals(backingField, value))
                return false;
            backingField = value;
            foreach(var name in propertyNames)
                OnPropertyChanged(name);
            return true;
        }

        protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName] String propertyName = "") {
            return SetValue(ref backingField, value, new String[] { propertyName });
        }

        /// <summary>
        /// Is the Value different to the current Value.
        /// </summary>
        /// <typeparam name="T">Type of the Values.</typeparam>
        /// <param name="backingField">The Value to be checked for Equality.</param>
        /// <param name="value">The new Value.</param>
        /// <returns>True if the Value is different, false otherwise.</returns>
        protected Boolean IsValueDifferent<T>(T backingField, T value)
        {
            return !Equals(backingField, value);
        }
    }
}