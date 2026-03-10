using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Navi_UI_WPF.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        public HomeViewModel()
        {
            Title = "Trang Chủ";
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        
        // Dashboard properties can be added here later (e.g., stats)
    }
}
