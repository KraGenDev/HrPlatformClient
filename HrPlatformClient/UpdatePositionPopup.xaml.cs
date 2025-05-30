using CommunityToolkit.Maui.Views; // <-- ���� �������!
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
            Close(null); // ���������
        }

        void OnSaveClicked(object sender, EventArgs e)
        {
            UpdatedName = NewNameEntry.Text;
            Close(UpdatedName); // ��������� ���� �����
        }
    }
}
