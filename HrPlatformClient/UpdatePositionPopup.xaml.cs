using CommunityToolkit.Maui.Views; // <-- дуже важливо!
using System;

namespace HrPlatformClient
{
    public partial class UpdatePositionPopup : Popup
    {
        public string UpdatedName { get; private set; }

        public UpdatePositionPopup(string currentName)
        {
            InitializeComponent();
            NewNameEntry.Text = currentName;
            CanBeDismissedByTappingOutsideOfPopup = false; 
        }

        void OnCancelClicked(object sender, EventArgs e)
        {
            Close(null); // скасувати
        }

        void OnSaveClicked(object sender, EventArgs e)
        {
            UpdatedName = NewNameEntry.Text;
            Close(UpdatedName); // повернути нову назву
        }
    }
}
