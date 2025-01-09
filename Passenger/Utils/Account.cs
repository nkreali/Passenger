using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passenger.Utils
{
    public class Account : INotifyPropertyChanged 
    {
        private string? _password;
        private bool _isPasswordVisible;
        public int Id { get; set; }
        public string? Login { get; set; }
        public string? Password 
        {
            get => _isPasswordVisible ? _password : new string('\u2022', 16);
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string? Service { get; set; }
        public int Owner_Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
