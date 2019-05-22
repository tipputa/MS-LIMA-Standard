using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metabolomics.Core;

namespace Metabolomics.MsLima.ViewModel
{
    public class ShortMessageVM: ViewModelBase
    {
        private string title;
        public string Title {
            get => title;
            set => OnPropertyChangedIfSet(ref title, value, nameof(Title));
        }
        private string message;
        public string Message {
            get => message;
            set => OnPropertyChangedIfSet(ref message, value, nameof(Message));
        }

    }
}
