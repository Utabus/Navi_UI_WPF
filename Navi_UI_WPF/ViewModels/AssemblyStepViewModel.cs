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
        private bool _isCurrent;
        private int _stepNumber; // Backing field for StepNumber

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
            set => SetProperty(ref _description, value);
        }

        public string Note
        {
            get => _note;
            set => SetProperty(ref _note, value);
        }

        public string Bolts
        {
            get => _bolts;
            set => SetProperty(ref _bolts, value);
        }

        public string Force
        {
            get => _force;
            set => SetProperty(ref _force, value);
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
            set => SetProperty(ref _isCompleted, value);
        }

        public bool IsCurrent
        {
            get => _isCurrent;
            set => SetProperty(ref _isCurrent, value);
        }

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
    }
}
