using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Navi_UI_WPF.ViewModels
{
    public class NgReasonItem : ObservableObject
    {
        private bool _isSelected;
        public string Text { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    public class NgReasonViewModel : ObservableObject
    {
        private string _customReason;
        private bool? _dialogResult;

        public NgReasonViewModel()
        {
            Reasons = new ObservableCollection<NgReasonItem>
            {
                new NgReasonItem { Text = "Thiếu linh kiện" },
                new NgReasonItem { Text = "Sai linh kiện" },
                new NgReasonItem { Text = "Lỗi lực siết (Torque)" },
                new NgReasonItem { Text = "Trầy xước/Hư hỏng vật lý" },
                new NgReasonItem { Text = "Lỗi lắp ráp/Sai quy trình" },
                new NgReasonItem { Text = "Lỗi thiết bị/Dụng cụ" },
                new NgReasonItem { Text = "Khác (Nhập bên dưới)" }
            };

            ConfirmCommand = new RelayCommand(ExecuteConfirm, CanConfirm);
            CancelCommand = new RelayCommand(ExecuteCancel);

            // Watch for selection changes to update Confirm button state
            foreach (var item in Reasons)
            {
                item.PropertyChanged += (s, e) => {
                    if (e.PropertyName == nameof(NgReasonItem.IsSelected))
                    {
                        ConfirmCommand.NotifyCanExecuteChanged();
                        OnPropertyChanged(nameof(IsOtherSelected));
                    }
                };
            }
        }

        // ─── Properties ──────────────────────────────────────────────────────

        public ObservableCollection<NgReasonItem> Reasons { get; }

        public string CustomReason
        {
            get => _customReason;
            set
            {
                if (SetProperty(ref _customReason, value))
                {
                    ConfirmCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public bool IsOtherSelected => Reasons.Any(r => r.IsSelected && r.Text == "Khác (Nhập bên dưới)");

        public string FinalReason
        {
            get
            {
                var selectedTexts = Reasons
                    .Where(r => r.IsSelected && r.Text != "Khác (Nhập bên dưới)")
                    .Select(r => r.Text)
                    .ToList();

                if (IsOtherSelected && !string.IsNullOrWhiteSpace(CustomReason))
                {
                    selectedTexts.Add(CustomReason);
                }

                return string.Join(" - ", selectedTexts);
            }
        }

        // ─── Commands ────────────────────────────────────────────────────────

        public IRelayCommand ConfirmCommand { get; }
        public IRelayCommand CancelCommand { get; }

        // ─── Methods ─────────────────────────────────────────────────────────

        private bool CanConfirm()
        {
            bool anySelected = Reasons.Any(r => r.IsSelected);
            if (!anySelected) return false;

            if (IsOtherSelected && string.IsNullOrWhiteSpace(CustomReason)) return false;

            return true;
        }

        private void ExecuteConfirm()
        {
            DialogResult = true;
        }

        private void ExecuteCancel()
        {
            DialogResult = false;
        }
    }
}
