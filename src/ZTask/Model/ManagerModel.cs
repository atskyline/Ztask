using System;
using GalaSoft.MvvmLight;

namespace ZTask.Model
{
    public class ManagerModel : ViewModelBase
    {
        
        internal virtual Int64 ListId { get; set; }

        private String _title;
        public virtual String Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        public virtual Boolean IsHideWindow { get; set; }
    }
}
