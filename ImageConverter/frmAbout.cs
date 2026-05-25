namespace ImageConverter;

public partial class frmAbout : Form
{
    public frmAbout()
    {
        InitializeComponent();
    }

    private void btnSupport_Click(object? sender, EventArgs e)
    {
        AppSupport.OpenBuyMeACoffee(this);
    }

    private void btnOK_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
