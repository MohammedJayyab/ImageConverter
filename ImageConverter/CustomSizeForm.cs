namespace ImageConverter;

internal partial class CustomSizeForm : Form
{
    internal int OutputWidth => (int)nudWidth.Value;

    internal int OutputHeight => (int)nudHeight.Value;

    internal string OutputFolder { get; private set; } = string.Empty;

    internal CustomSizeForm(string initialOutputFolder)
    {
        InitializeComponent();
        txtOutputFolder.Text = initialOutputFolder;
    }

    private void btnBrowse_Click(object? sender, EventArgs e)
    {
        if (FolderPicker.TryPick(this, "Choose output folder", txtOutputFolder.Text, out var folder))
        {
            txtOutputFolder.Text = folder;
        }
    }

    private void btnOK_Click(object? sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtOutputFolder.Text))
            {
                throw new ArgumentException();
            }

            var folder = Path.GetFullPath(txtOutputFolder.Text.Trim());
            OutputFolder = Path.TrimEndingDirectorySeparator(folder);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch
        {
            MessageBox.Show(
                this,
                "Enter a valid output folder.",
                "Set Custom Size",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            txtOutputFolder.Focus();
        }
    }
}
