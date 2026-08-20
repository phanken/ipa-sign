IPA SIGNER PRO WINDOWS v4 - BẢN ĐÓNG GÓI LẠI TỪ SOURCE v2 ĐẦY ĐỦ

Cấu trúc source bắt buộc:
.github/workflows/build-windows.yml
src/IPASignerPro/IPASignerPro.csproj
src/IPASignerPro/Program.cs
src/IPASignerPro/MainForm.cs
src/IPASignerPro/app.manifest
tools/PUT_SIDELOADER_HERE.txt
README.md
BACKEND-HUONG-DAN.md

BƯỚC DUY NHẤT ANH CẦN LÀM TRƯỚC KHI PUSH:
- Copy file Sideloader Windows đã tải vào tools/sideloader.exe

GITHUB ACTIONS SẼ:
- kiểm tra source;
- build IPASignerPro.exe;
- tự tải latest libimobiledevice/libplist Windows runtime;
- gom DLL vào publish/tools;
- copy sideloader.exe;
- kiểm tra có plist DLL;
- xuất artifact IPA-Signer-Pro-Windows-v4-Full.

Nếu Actions báo lỗi trước khi Upload artifact thì KHÔNG dùng bản build đó.
