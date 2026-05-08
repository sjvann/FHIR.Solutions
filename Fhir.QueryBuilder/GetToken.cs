using Fhir.QueryBuilder.Platform;

namespace Fhir.QueryBuilder;

/// <summary>JWT 取得對話框（設計工具檔遺失後改為程式建立控制項）。</summary>
public class GetToken : Form
{
    private readonly string? _baseUrl;
    private string? _tokenValue;
    private readonly TextBox txtJWT_ClientId;
    private readonly TextBox txtJWT_PrivateKey;

    public GetToken(string? baseUrl)
    {
        _baseUrl = baseUrl;
        Text = "JWT";
        Size = new Size(420, 180);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        var lblId = new Label { Text = "Client Id:", Location = new Point(12, 16), AutoSize = true };
        txtJWT_ClientId = new TextBox { Location = new Point(120, 12), Width = 260 };
        var lblKey = new Label { Text = "Private Key:", Location = new Point(12, 52), AutoSize = true };
        txtJWT_PrivateKey = new TextBox { Location = new Point(120, 48), Width = 260, Height = 60, Multiline = true };
        var btnOk = new Button { Text = "OK", Location = new Point(220, 120), DialogResult = DialogResult.OK };
        var btnCancel = new Button { Text = "Cancel", Location = new Point(300, 120), DialogResult = DialogResult.Cancel };
        btnOk.Click += BtnJWT_Ok_Click;
        btnCancel.Click += BtnJWT_Cancel_Click;

        Controls.Add(lblId);
        Controls.Add(txtJWT_ClientId);
        Controls.Add(lblKey);
        Controls.Add(txtJWT_PrivateKey);
        Controls.Add(btnOk);
        Controls.Add(btnCancel);
        AcceptButton = btnOk;
        CancelButton = btnCancel;
    }

    public string GetTokenValue() => _tokenValue ?? "";

    private void BtnJWT_Cancel_Click(object? sender, EventArgs e)
    {
        txtJWT_ClientId.Text = "";
        txtJWT_PrivateKey.Text = "";
    }

    private void BtnJWT_Ok_Click(object? sender, EventArgs e)
    {
        if (_baseUrl is null)
        {
            MessageBox.Show("Base URL is not set");
            return;
        }

        if (string.IsNullOrEmpty(txtJWT_ClientId.Text) || string.IsNullOrEmpty(txtJWT_PrivateKey.Text))
        {
            MessageBox.Show("Client Id or Private Key is empty");
            return;
        }

        _tokenValue = LegacyJwtTokenStub.GetTokenJWT(_baseUrl, txtJWT_ClientId.Text, txtJWT_PrivateKey.Text);
        Hide();
    }
}
