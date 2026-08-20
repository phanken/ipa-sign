IPA Signer Pro Windows v5

Bản này giữ nguyên UI v2 và chỉ sửa runtime Windows.

Trước khi push GitHub:
1. Tải Sideloader Windows CLI.
2. Đặt file vào:
   tools/sideloader.exe

Workflow sẽ tự tải đúng gói:
libimobiledevice.1.2.1-r1122-win-x64.zip

và chỉ build xanh nếu trong artifact thực sự có:
tools/sideloader.exe
tools/plist.dll

Sau khi Actions xanh, tải:
IPA-Signer-Pro-Windows-v5-Full

Mở thư mục tools của artifact và kiểm tra phải thấy plist.dll cạnh sideloader.exe.
