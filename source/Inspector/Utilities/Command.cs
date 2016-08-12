using System;
using System.Windows.Input;
using System.Diagnostics.CodeAnalysis;

namespace ChristianMoser.WpfInspector.Utilities
{
    public class Command<T> : ICommand
    {
        #region Private Members

        private readonly Action<T> _executeMethod;
        private readonly Func<T, bool> _canExecuteMethod;
        private static readonly Type TypeOfParameter = typeof(T);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Command{T}"/>.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
        public Command(Action<T> executeMethod)
            : this(executeMethod, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Command{T}"/>.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command.  This can be null.</param>
        /// <exception cref="ArgumentNullException">When both <paramref name="executeMethod"/> and <paramref name="canExecuteMethod"/> ar <see langword="null" />.</exception>
        public Command(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            if ((executeMethod == null) && (canExecuteMethod == null))
            {
                throw new ArgumentException("At least one method has to be specified");
            }

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        #endregion

        #region ICommand Members

        ///<summary>
        ///Occurs when changes occur that affect whether or not the command should execute.
        ///</summary>
        public event EventHandler CanExecuteChanged;

        ///<summary>
        ///Defines the method that determines whether the command can execute in its current state.
        ///</summary>
        ///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        ///<returns>
        ///true if this command can be executed; otherwise, false.
        ///</returns>
        bool ICommand.CanExecute(object parameter)
        {
            if ((parameter == null) && (TypeOfParameter.IsValueType))
            {
                return CanExecuteIfParameterIsNull;
            }

            return CanExecute((T)parameter);
        }

        ///<summary>
        ///Defines the method to be called when the command is invoked.
        ///</summary>
        ///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        void ICommand.Execute(object parameter)
        {
            Execute((T)parameter);
        }

        #endregion
     

        #region Public Functionality

        /// <summary>
        /// Value indicating if command can get executed if parameter is null. False by default.
        /// </summary>
        public bool CanExecuteIfParameterIsNull { get; set; }

        ///<summary>
        ///Defines the method that determines whether the command can execute in its current state.
        ///</summary>
        ///<param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        ///<returns>
        ///<see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        ///</returns>
        public bool CanExecute(T parameter)
        {
            if (_canExecuteMethod == null)
            {
                return true;
            }
            return _canExecuteMethod(parameter);
        }

        ///<summary>
        ///Defines the method to be called when the command is invoked.
        ///</summary>
        ///<param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        public void Execute(T parameter)
        {
            if (_executeMethod == null)
            {
                return;
            }
            _executeMethod(parameter);
        }

        /// <summary>
        /// Raises <see cref="CanExecuteChanged"/> on the UI thread so every command invoker
        /// can requery to check if the command can execute.
        /// <remarks>Note that this will trigger the execution of <see cref="CanExecute"/> once for each invoker.</remarks>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        #endregion

        #region Protected Functionality

        /// <summary>
        /// Raises <see cref="CanExecuteChanged"/> on the UI thread so every command invoker can requery to check if the command can execute.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            EventHandler canExecuteChangedHandler = CanExecuteChanged;
            if (canExecuteChangedHandler != null)
            {
                canExecuteChangedHandler(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
