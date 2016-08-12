using System.ComponentModel;
using System.Windows.Input;
using ChristianMoser.WpfInspector.Utilities;
using System;
using System.Windows.Controls.Primitives;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public class CommandPropertyItem : PropertyItem
    {
        private readonly ICommand _command;
        private readonly Command<object> _executeCommand;

        #region Construction

        public CommandPropertyItem(PropertyDescriptor property, object instance)
            :base(property, instance)
        {
            _command = property.GetValue(instance) as ICommand;
            if (_command != null)
            {
                _command.CanExecuteChanged += CanExecuteChanged;
            }

            _executeCommand = new Command<object>(o => _command.Execute(GetCommandParameter()), o => _command != null && _command.CanExecute(GetCommandParameter()));
        }

        #endregion

        /// <summary>
        /// Gets or sets the execute command.
        /// </summary>
        public ICommand ExecuteCommand
        {
            get { return _executeCommand; }
        }

        public override void Dispose()
        {
            if( _command != null)
            {
                _command.CanExecuteChanged -= CanExecuteChanged;
            }
            base.Dispose();
        }

        #region Private Helpers

        private object GetCommandParameter()
        {
            var commandSource = Instance as ICommandSource;
            if (commandSource != null)
            {
                return commandSource.CommandParameter;
            }
            return null;
        }

        private void CanExecuteChanged( object sender, EventArgs e)
        {
            _executeCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}
