namespace ImageConverter;

public partial class frmHowToUse : Form
{
    private const string HelpRtfFileName = "HelpHowToUse.rtf";

    public frmHowToUse()
    {
        InitializeComponent();
        LoadHelpRtf();
    }

    private void LoadHelpRtf()
    {
        var path = Path.Combine(AppContext.BaseDirectory, HelpRtfFileName);
        try
        {
            if (!File.Exists(path))
            {
                richTextHelp.Text = $"Help file not found:\r\n{path}";
                return;
            }

            richTextHelp.LoadFile(path, RichTextBoxStreamType.RichText);
            richTextHelp.ReadOnly = true;
        }
        catch (Exception ex)
        {
            richTextHelp.Text = $"Could not load help:\r\n{ex.Message}";
        }
    }

    private void btnClose_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
