using CommunityToolkit.Maui.Views;

namespace HrPlatformClient;

public partial class UpdateDepartmentPopup : Popup
{
    public string UpdatedName { get; private set; }

    public UpdateDepartmentPopup(string currentName)
    {
        InitializeComponent();
        NewNameEntry.Text = currentName;
        CanBeDismissedByTappingOutsideOfPopup = false;
    }

    void OnCancelClicked(object sender, EventArgs e)
    {
        Close(null); // �� ���������
    }

    void OnSaveClicked(object sender, EventArgs e)
    {
        UpdatedName = NewNameEntry.Text;
        Close(UpdatedName); // ��������� ���� �����
    }
}