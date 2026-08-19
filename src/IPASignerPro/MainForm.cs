using System.Diagnostics;
using System.Text;

namespace IPASignerPro;

public sealed class MainForm : Form
{
    private readonly TextBox ipaPath = new() { ReadOnly = true };
    private readonly TextBox console = new() {
        Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both,
        Font = new Font("Consolas", 10), BackColor = Color.FromArgb(20,20,20),
        ForeColor = Color.Gainsboro
    };
    private readonly TextBox input = new() { PlaceholderText = "Nhập Apple ID / mật khẩu / mã 2FA khi backend yêu cầu..." };
    private readonly Button sendButton = new() { Text = "Gửi phản hồi" };
    private readonly Button chooseButton = new() { Text = "Chọn IPA" };
    private readonly Button installButton = new() { Text = "KÝ & CÀI VÀO IPHONE" };
    private readonly Button backendButton = new() { Text = "Kiểm tra backend" };
    private readonly Button openBackendPage = new() { Text = "Trang tải Sideloader" };
    private readonly Label status = new() { Text = "Chưa kiểm tra backend", AutoSize = true };
    private Process? running;

    public MainForm()
    {
        Text = "IPA Signer Pro - Windows";
        Width = 860;
        Height = 650;
        MinimumSize = new Size(760, 560);
        StartPosition = FormStartPosition.CenterScreen;

        var title = new Label {
            Text = "IPA SIGNER PRO",
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            AutoSize = true
        };
        var subtitle = new Label {
            Text = "Ký và cài IPA lên iPhone bằng Apple ID miễn phí (7 ngày)",
            AutoSize = true
        };

        ipaPath.Dock = DockStyle.Fill;
        chooseButton.AutoSize = true;
        installButton.Height = 48;
        installButton.Dock = DockStyle.Fill;
        installButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);

        var filePanel = new TableLayoutPanel { ColumnCount = 2, Dock = DockStyle.Top, Height = 38 };
        filePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        filePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        filePanel.Controls.Add(ipaPath, 0, 0);
        filePanel.Controls.Add(chooseButton, 1, 0);

        var backendPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
        backendPanel.Controls.Add(backendButton);
        backendPanel.Controls.Add(openBackendPage);
        backendPanel.Controls.Add(status);

        var inputPanel = new TableLayoutPanel { ColumnCount = 2, Dock = DockStyle.Bottom, Height = 38 };
        inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        inputPanel.Controls.Add(input, 0, 0);
        inputPanel.Controls.Add(sendButton, 1, 0);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(18), RowCount = 8 };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

        layout.Controls.Add(title, 0, 0);
        layout.Controls.Add(subtitle, 0, 1);
        layout.Controls.Add(new Label { Text = "IPA", AutoSize = true, Padding = new Padding(0,12,0,4) }, 0, 2);
        layout.Controls.Add(filePanel, 0, 3);
        layout.Controls.Add(backendPanel, 0, 4);
        layout.Controls.Add(installButton, 0, 5);
        layout.Controls.Add(console, 0, 6);
        layout.Controls.Add(inputPanel, 0, 7);
        Controls.Add(layout);

        chooseButton.Click += ChooseIpa;
        backendButton.Click += async (_,__) => await CheckBackend();
        openBackendPage.Click += (_,__) => OpenUrl("https://github.com/Dadoum/Sideloader/actions");
        installButton.Click += async (_,__) => await Install();
        sendButton.Click += (_,__) => SendInput();
        input.KeyDown += (_,e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; SendInput(); } };

        Log("IPA Signer Pro khởi động.");
        Log("Backend: Dadoum/Sideloader. Ứng dụng này không lưu Apple ID hoặc mật khẩu.");
    }

    private string BackendPath()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "tools", "sideloader.exe"),
            Path.Combine(AppContext.BaseDirectory, "sideloader.exe"),
            Path.Combine(Environment.CurrentDirectory, "tools", "sideloader.exe")
        };
        return candidates.FirstOrDefault(File.Exists) ?? candidates[0];
    }

    private void ChooseIpa(object? sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog { Filter = "IPA files (*.ipa)|*.ipa", Title = "Chọn file IPA" };
        if (dlg.ShowDialog() == DialogResult.OK) ipaPath.Text = dlg.FileName;
    }

    private async Task CheckBackend()
    {
        var exe = BackendPath();
        if (!File.Exists(exe))
        {
            status.Text = "Thiếu sideloader.exe";
            Log("Không tìm thấy backend: " + exe);
            Log("Bấm 'Trang tải Sideloader', tải bản Windows từ GitHub Actions rồi đặt sideloader.exe và các DLL đi kèm vào thư mục tools.");
            return;
        }
        status.Text = "Đang kiểm tra...";
        var code = await RunOneShot(exe, "version");
        status.Text = code == 0 ? "Backend OK" : "Backend có lỗi";
    }

    private async Task Install()
    {
        if (running is { HasExited: false })
        {
            MessageBox.Show("Đang có tiến trình chạy.");
            return;
        }
        if (!File.Exists(ipaPath.Text))
        {
            MessageBox.Show("Anh hãy chọn file IPA trước.");
            return;
        }
        var exe = BackendPath();
        if (!File.Exists(exe))
        {
            MessageBox.Show("Chưa có sideloader.exe trong thư mục tools.");
            return;
        }

        console.Clear();
        Log("=== BẮT ĐẦU KÝ & CÀI ===");
        Log("IPA: " + ipaPath.Text);
        Log("Hãy cắm iPhone bằng USB, mở khóa và bấm Trust nếu iPhone hỏi.");
        Log("Khi backend hỏi Apple ID, mật khẩu hoặc mã 2FA, nhập ở ô dưới rồi bấm 'Gửi phản hồi'.");
        installButton.Enabled = false;

        var psi = new ProcessStartInfo {
            FileName = exe,
            Arguments = $"install \"{ipaPath.Text}\" -i",
            WorkingDirectory = Path.GetDirectoryName(exe)!,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        running = new Process { StartInfo = psi, EnableRaisingEvents = true };
        running.OutputDataReceived += (_,e) => { if (e.Data != null) Log(e.Data); };
        running.ErrorDataReceived += (_,e) => { if (e.Data != null) Log("[ERR] " + e.Data); };

        try
        {
            running.Start();
            running.BeginOutputReadLine();
            running.BeginErrorReadLine();
            await running.WaitForExitAsync();
            Log(running.ExitCode == 0 ? "=== HOÀN TẤT ===" : $"=== KẾT THÚC VỚI MÃ LỖI {running.ExitCode} ===");
        }
        catch (Exception ex)
        {
            Log("[EXCEPTION] " + ex.Message);
        }
        finally
        {
            installButton.Enabled = true;
            running?.Dispose();
            running = null;
        }
    }

    private void SendInput()
    {
        var text = input.Text;
        if (string.IsNullOrEmpty(text) || running == null || running.HasExited) return;
        try
        {
            running.StandardInput.WriteLine(text);
            running.StandardInput.Flush();
            Log(">> [đã gửi phản hồi]");
            input.Clear();
        }
        catch (Exception ex) { Log("[ERR] Không gửi được phản hồi: " + ex.Message); }
    }

    private async Task<int> RunOneShot(string exe, string args)
    {
        var psi = new ProcessStartInfo {
            FileName = exe, Arguments = args, WorkingDirectory = Path.GetDirectoryName(exe)!,
            UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true,
            CreateNoWindow = true
        };
        try
        {
            using var p = Process.Start(psi)!;
            var o = await p.StandardOutput.ReadToEndAsync();
            var e = await p.StandardError.ReadToEndAsync();
            await p.WaitForExitAsync();
            if (!string.IsNullOrWhiteSpace(o)) Log(o.Trim());
            if (!string.IsNullOrWhiteSpace(e)) Log("[ERR] " + e.Trim());
            return p.ExitCode;
        }
        catch (Exception ex) { Log("[EXCEPTION] " + ex.Message); return -1; }
    }

    private void Log(string text)
    {
        if (InvokeRequired) { BeginInvoke(() => Log(text)); return; }
        console.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
    }

    private static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
