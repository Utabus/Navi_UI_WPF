using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Navi_UI_WPF.ViewModels
{
    /// <summary>
    /// ViewModel cho một bước lắp ráp
    /// ViewModel for an assembly instruction step
    /// </summary>
    public class AssemblyStepViewModel : ObservableObject
    {
        private int _id;
        private string _description;
        private string _note;
        private string _bolts;
        private string _force;
        private string _images;
        private string _type;
        private bool _isCompleted;
        private bool _isNg;
        private bool _isCurrent;
        private bool _isLocked;
        private int _stepNumber;
        private int? _grease;
        private int? _forceBit;
        private int? _timer;
        private string _po;
        private string _historyNote;
        private int? _count;
        private bool? _ok;
        private bool? _ng;
        private int? _itemAuditId;
        private int? _productId;

        public int? ItemAuditId
        {
            get => _itemAuditId;
            set => SetProperty(ref _itemAuditId, value);
        }

        public int? ProductId
        {
            get => _productId;
            set => SetProperty(ref _productId, value);
        }

        public bool IsLocked
        {
            get => _isLocked;
            set => SetProperty(ref _isLocked, value);
        }

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public int StepNumber
        {
            get => _stepNumber;
            set => SetProperty(ref _stepNumber, value);
        }

        public string Description
        {
            get => _description;
            set
            {
                if (SetProperty(ref _description, value))
                {
                    OnPropertyChanged(nameof(HasDetail));
                }
            }
        }

        public string Note
        {
            get => _note;
            set
            {
                if (SetProperty(ref _note, value))
                {
                    OnPropertyChanged(nameof(RequiresConfirmation));
                }
            }
        }

        public string Bolts
        {
            get => _bolts;
            set
            {
                if (SetProperty(ref _bolts, value))
                {
                    OnPropertyChanged(nameof(RequiresConfirmation));
                }
            }
        }

        public string Force
        {
            get => _force;
            set
            {
                if (SetProperty(ref _force, value))
                {
                    OnPropertyChanged(nameof(RequiresConfirmation));
                }
            }
        }

        public string Images
        {
            get => _images;
            set => SetProperty(ref _images, value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (SetProperty(ref _isCompleted, value))
                {
                    OnPropertyChanged(nameof(RequiresConfirmation));
                    OnPropertyChanged(nameof(IsLocked));
                }
            }
        }

        public bool IsNg
        {
            get => _isNg;
            set
            {
                if (SetProperty(ref _isNg, value))
                {
                    OnPropertyChanged(nameof(RequiresConfirmation));
                    OnPropertyChanged(nameof(IsLocked));
                }
            }
        }

        public bool IsCurrent
        {
            get => _isCurrent;
            set => SetProperty(ref _isCurrent, value);
        }

        public int? Grease
        {
            get => _grease;
            set => SetProperty(ref _grease, value);
        }

        public int? ForceBit
        {
            get => _forceBit;
            set => SetProperty(ref _forceBit, value);
        }

        public int? Timer
        {
            get => _timer;
            set => SetProperty(ref _timer, value);
        }

        public string PO
        {
            get => _po;
            set
            {
                if (SetProperty(ref _po, value))
                {
                    OnPropertyChanged(nameof(IsLocked));
                }
            }
        }

        public string HistoryNote
        {
            get => _historyNote;
            set => SetProperty(ref _historyNote, value);
        }

        public int? Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        public bool? OK
        {
            get => _ok;
            set => SetProperty(ref _ok, value);
        }

        public bool? NG
        {
            get => _ng;
            set => SetProperty(ref _ng, value);
        }

        /// <summary>
        /// Có nội dung chi tiết công đoạn không
        /// </summary>
        public bool HasDetail => !string.IsNullOrWhiteSpace(Description);

        /// <summary>
        /// Có thông tin ốc vít không
        /// </summary>
        public bool HasBolts => !string.IsNullOrWhiteSpace(Bolts);

        /// <summary>
        /// Có thông tin lực siết không
        /// </summary>
        public bool HasForce => !string.IsNullOrWhiteSpace(Force);

        /// <summary>
        /// Có ghi chú không
        /// </summary>
        public bool HasNote => !string.IsNullOrWhiteSpace(Note);

        /// <summary>
        /// Có hình ảnh không
        /// </summary>
        public bool HasImages => !string.IsNullOrWhiteSpace(Images);

        /// <summary>
        /// Bước này có yêu cầu xác nhận OK/NG không (Luôn trả về true để hiển thị nút OK/NG)
        /// </summary>
        public bool RequiresConfirmation => true;
    }
}
