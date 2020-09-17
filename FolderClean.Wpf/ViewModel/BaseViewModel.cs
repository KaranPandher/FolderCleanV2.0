using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using FolderClean.Wpf.Handlers;

namespace FolderClean.Wpf.ViewModel
{
    /// <summary>
    /// Base View Model with some basic common functions and Action Type
    /// </summary>
    public class BaseViewModel<T> : INotifyPropertyChanged where T : Enum
    {
        /// <summary>
        /// Shows Loading
        /// </summary>
        /// <param name="text"></param>
        public void ShowLoading(string text = "Processing...Please wait.")
        {
            LoadingText = text;
            IsLoading = Visibility.Visible;
        }
        /// <summary>
        /// Hides Loading
        /// </summary>
        public void HideLoading()
        {
            IsLoading = Visibility.Collapsed;
        }

        /// <summary>
        /// Bindable Property
        /// </summary>
        public Visibility IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Bindable Property
        /// </summary>
        public string LoadingText
        {
            get { return _loadingText; }
            set
            {
                _loadingText = value;
                OnPropertyChanged();
            }
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
        private CommandHandler<T> _actionCommand;
        private string _loadingText = "Processing...";
        private Visibility _isLoading = Visibility.Collapsed;
        public CommandHandler<T> ActionCommand => _actionCommand;
        
        /// <summary>
        /// Perform Actions based on Action Type
        /// </summary>
        protected virtual Task PerformAction(T action)
        {
            return Task.CompletedTask;
        }
        
        public BaseViewModel()
        {
            _actionCommand = new CommandHandler<T>(async action => await PerformAction(action), true);
        }

        public void CanExecuteActionCommand(bool result)
        {
            _actionCommand.CanExecute(result);
        }
        
        public virtual void Init()
        {
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}