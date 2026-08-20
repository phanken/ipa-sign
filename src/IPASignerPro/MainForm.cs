using System.Diagnostics;
using System.Text;

namespace IPASignerPro;

public sealed class MainForm : Form
{
    private readonly TextBox ipaPath = new() { ReadOnly = true };
    private readonly TextBox console = new() {
        Multiline = true,
        ReadOnly = true,
        ScrollBars = ScrollBars.Both,
        WordWrap = false,
        Font = new Font("Consolas", 10),
        BackColor = Color.FromArgb(20, 20, 20),
        ForeColor = Color.Gainsboro,
        Dock = DockStyle.Fill
    };

    private readonly TextBox appleId = new() { PlaceholderText = "Apple ID (email)" };
    private readonly TextBox password = new() { PlaceholderText = "Mật khẩu Apple ID", UseSystemPasswordChar = true };
    private readonly TextBox twoFa = new() { PlaceholderText = "Mã 2FA 6 số", MaxLength = 6 };

    private readonly Button sendAppleId = new() { Text = "Gửi Apple ID", AutoSize = true };
    private readonly Button sendPassword = new() { Text = "Gửi mật khẩu", AutoSize = true };
    private readonly Button send2Fa = new() { Text = "Gửi 2FA", AutoSize = true };

    private readonly Button chooseButton = new() { Text = "Chọn IPA", AutoSize = true };
    private readonly Button installButton = new() { Text = "KÝ & CÀI VÀO IPHONE" };
    private readonly Button backendButton = new() { Text = "Kiểm tra backend", AutoSize = true };
    private readonly Button openBackendPage = new() { Text = "Trang tải Sideloader", AutoSize = true };
    private readonly Label status = new() { Text = "Chưa kiểm tra backend", AutoSize = true, Padding = new Padding(8, 7, 0, 0) };

    private Process? running;

    public MainForm()
    {
        Text = "IPA Signer Pro - Windows";
        Width = 980;
        Height = 760;
        MinimumSize = new Size(820, 650);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10);

        var title = new Label {
            Text = "IPA SIGNER PRO",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            AutoSize = true
        };
        var subtitle = new Label {
            Text = "Ký và cài IPA lên iPhone bằng Apple ID miễn phí (7 ngày)",
            AutoSize = true,
            ForeColor = Color.DimGray
        };

        ipaPath.Dock = DockStyle.Fill;
        ipaPath.Height = 32;

        installButton.Dock = DockStyle.Fill;
        installButton.Height = 52;
        installButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);

        var filePanel = new TableLayoutPanel {
            ColumnCount = 2, Dock = DockStyle.Fill, Height = 38, Margin = new Padding(0)
        };
        filePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        filePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        filePanel.Controls.Add(ipaPath, 0, 0);
        filePanel.Controls.Add(chooseButton, 1, 0);

        var backendPanel = new FlowLayoutPanel {
            Dock = DockStyle.Fill, AutoSize = true, WrapContents = false, Margin = new Padding(0)
        };
        backendPanel.Controls.Add(backendButton);
        backendPanel.Controls.Add(openBackendPage);
        backendPanel.Controls.Add(status);

        var credentials = new GroupBox {
            Text = "Thông tin ký (chỉ gửi khi phần LOG yêu cầu)",
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };

        var credGrid = new TableLayoutPanel {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 3,
            Padding = new Padding(4)
        };
        credGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        credGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        credGrid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        for (int i = 0; i < 3; i++) credGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 33.333f));

        appleId.Dock = DockStyle.Fill;
        password.Dock = DockStyle.Fill;
        twoFa.Dock = DockStyle.Fill;

        credGrid.Controls.Add(new Label { Text = "Apple ID", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        credGrid.Controls.Add(appleId, 1, 0);
        credGrid.Controls.Add(sendAppleId, 2, 0);

        credGrid.Controls.Add(new Label { Text = "Mật khẩu", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        credGrid.Controls.Add(password, 1, 1);
        credGrid.Controls.Add(sendPassword, 2, 1);

        credGrid.Controls.Add(new Label { Text = "Mã 2FA", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
        credGrid.Controls.Add(twoFa, 1, 2);
        credGrid.Controls.Add(send2Fa, 2, 2);

        credentials.Controls.Add(credGrid);

        var logGroup = new GroupBox {
            Text = "LOG - xem backend đang yêu cầu gì",
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };
        logGroup.Controls.Add(console);

        var note = new Label {
            Text = "Không nhập sẵn mật khẩu. Chỉ bấm nút gửi tương ứng khi LOG yêu cầu. Mật khẩu và mã 2FA không được lưu.",
            Dock = DockStyle.Fill,
            ForeColor = Color.DarkRed,
            AutoSize = true
        };

        var layout = new TableLayoutPanel {
            Dock = DockStyle.Fill,
            Padding = new Padding(22),
            RowCount = 9,
            ColumnCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // title
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // subtitle
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));   // spacer
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));   // ipa
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));   // backend
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));   // install
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // log
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));  // creds
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));   // note

        layout.Controls.Add(title, 0, 0);
        layout.Controls.Add(subtitle, 0, 1);
        layout.Controls.Add(filePanel, 0, 3);
        layout.Controls.Add(backendPanel, 0, 4);
        layout.Controls.Add(installButton, 0, 5);
        layout.Controls.Add(logGroup, 0, 6);
        layout.Controls.Add(credentials, 0, 7);
        layout.Controls.Add(note, 0, 8);
        Controls.Add(layout);

        chooseButton.Click += ChooseIpa;
        backendButton.Click += async (_,__) => await CheckBackend();
        openBackendPage.Click += (_,__) => OpenUrl("https://github.com/Dadoum/Sideloader/actions");
        installButton.Click += async (_,__) => await Install();

        sendAppleId.Click += (_,__) => SendSensitive(appleId, "Apple ID", clearAfterSend: false);
        sendPassword.Click += (_,__) => SendSensitive(password, "mật khẩu", clearAfterSend: true);
        send2Fa.Click += (_,__) => SendSensitive(twoFa, "mã 2FA", clearAfterSend: true);

        appleId.KeyDown += (_,e) => SendOnEnter(e, appleId, "Apple ID", false);
        password.KeyDown += (_,e) => SendOnEnter(e, password, "mật khẩu", true);
        twoFa.KeyDown += (_,e) => SendOnEnter(e, twoFa, "mã 2FA", true);

        Log("IPA Signer Pro v5.1 khởi động.");
        Log("1) Chọn IPA và cắm iPhone.");
        Log("2) Bấm KÝ & CÀI VÀO IPHONE.");
        Log("3) Đọc LOG. Khi backend yêu cầu gì thì bấm nút gửi tương ứng.");
        Log("Ứng dụng không lưu Apple ID, mật khẩu hoặc mã 2FA.");
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
        using var dlg = new OpenFileDialog {
            Filter = "IPA files (*.ipa)|*.ipa",
            Title = "Chọn file IPA"
        };
        if (dlg.ShowDialog() == DialogResult.OK)
            ipaPath.Text = dlg.FileName;
    }

    private async Task CheckBackend()
    {
        console.Clear();
        Log("=== KIỂM TRA BACKEND ===");

        var exe = BackendPath();
        Log("Sideloader: " + exe);

        if (!File.Exists(exe))
        {
            status.Text = "Thiếu sideloader.exe";
            Log("[FAIL] Không tìm thấy sideloader.exe.");
            return;
        }

        var toolDir = Path.GetDirectoryName(exe)!;
        var mustExist = new[]
        {
            "plist.dll"
        };

        var missing = new List<string>();
        foreach (var name in mustExist)
        {
            var path = Path.Combine(toolDir, name);
            if (File.Exists(path))
                Log("[OK] " + name);
            else
            {
                Log("[MISSING] " + name);
                missing.Add(name);
            }
        }

        var openssl = Directory.GetFiles(toolDir, "libcrypto*.dll")
            .Concat(Directory.GetFiles(toolDir, "libssl*.dll"))
            .ToArray();
        if (openssl.Length == 0)
        {
            Log("[WARN] Không thấy libcrypto/libssl DLL. Sideloader hiện cần OpenSSL runtime trên Windows.");
        }
        else
        {
            foreach (var dll in openssl) Log("[OK] " + Path.GetFileName(dll));
        }

        if (missing.Count > 0)
        {
            status.Text = "Thiếu runtime";
            Log("[FAIL] Backend chưa đủ DLL.");
            return;
        }

        status.Text = "Đang chạy thử...";
        Log("Chạy: sideloader version");

        var result = await RunOneShotDetailed(exe, "version");
        if (result.ExitCode == 0)
        {
            status.Text = "Backend OK";
            Log("[OK] Backend chạy được.");
        }
        else
        {
            status.Text = "Backend có lỗi";
            Log($"[FAIL] Exit code: {result.ExitCode}");
            if (string.IsNullOrWhiteSpace(result.StdOut) && string.IsNullOrWhiteSpace(result.StdErr))
            {
                Log("Không có output. Thường là thiếu DLL phụ thuộc hoặc Microsoft Visual C++ Runtime.");
                Log("Hãy dùng artifact v5.1 Full; nếu vẫn lỗi, cài Microsoft Visual C++ Redistributable x64.");
            }
        }
    }

    private async Task Install()
    {
        if (running is { HasExited: false })
        {
            MessageBox.Show("Đang có tiến trình ký/cài chạy.");
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
        Log("Cắm iPhone bằng USB, mở khóa và bấm Trust nếu máy hỏi.");
        Log("Không nhập thông tin đăng nhập cho đến khi LOG yêu cầu.");
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
            Log(running.ExitCode == 0
                ? "=== HOÀN TẤT ==="
                : $"=== KẾT THÚC VỚI MÃ LỖI {running.ExitCode} ===");
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

    private void SendOnEnter(KeyEventArgs e, TextBox box, string label, bool clear)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        SendSensitive(box, label, clear);
    }

    private void SendSensitive(TextBox box, string label, bool clearAfterSend)
    {
        var text = box.Text.Trim();
        if (string.IsNullOrEmpty(text))
        {
            MessageBox.Show($"Chưa nhập {label}.");
            return;
        }
        if (running == null || running.HasExited)
        {
            MessageBox.Show("Chưa có tiến trình ký/cài đang chờ phản hồi.");
            return;
        }

        try
        {
            running.StandardInput.WriteLine(text);
            running.StandardInput.Flush();
            Log($">> Đã gửi {label}.");
            if (clearAfterSend) box.Clear();
        }
        catch (Exception ex)
        {
            Log("[ERR] Không gửi được " + label + ": " + ex.Message);
        }
    }

    private sealed record ProcessResult(int ExitCode, string StdOut, string StdErr);

    private async Task<ProcessResult> RunOneShotDetailed(string exe, string args)
    {
        var psi = new ProcessStartInfo {
            FileName = exe,
            Arguments = args,
            WorkingDirectory = Path.GetDirectoryName(exe)!,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        try
        {
            using var p = Process.Start(psi)!;
            var outTask = p.StandardOutput.ReadToEndAsync();
            var errTask = p.StandardError.ReadToEndAsync();
            await p.WaitForExitAsync();
            var stdout = await outTask;
            var stderr = await errTask;

            if (!string.IsNullOrWhiteSpace(stdout))
                foreach (var line in stdout.Replace("\r","").Split('\n'))
                    if (!string.IsNullOrWhiteSpace(line)) Log(line);

            if (!string.IsNullOrWhiteSpace(stderr))
                foreach (var line in stderr.Replace("\r","").Split('\n'))
                    if (!string.IsNullOrWhiteSpace(line)) Log("[ERR] " + line);

            return new ProcessResult(p.ExitCode, stdout, stderr);
        }
        catch (Exception ex)
        {
            Log("[EXCEPTION] " + ex.Message);
            return new ProcessResult(-1, "", ex.ToString());
        }
    }

    private async Task<int> RunOneShot(string exe, string args)
    {
        var psi = new ProcessStartInfo {
            FileName = exe,
            Arguments = args,
            WorkingDirectory = Path.GetDirectoryName(exe)!,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        try
        {
            using var p = Process.Start(psi)!;
            var output = await p.StandardOutput.ReadToEndAsync();
            var error = await p.StandardError.ReadToEndAsync();
            await p.WaitForExitAsync();

            if (!string.IsNullOrWhiteSpace(output)) Log(output.Trim());
            if (!string.IsNullOrWhiteSpace(error)) Log("[ERR] " + error.Trim());
            return p.ExitCode;
        }
        catch (Exception ex)
        {
            Log("[EXCEPTION] " + ex.Message);
            return -1;
        }
    }

    private void Log(string text)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => Log(text));
            return;
        }
        console.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
        console.SelectionStart = console.TextLength;
        console.ScrollToCaret();
    }

    private static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
