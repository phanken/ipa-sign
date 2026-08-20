IPA Signer Pro Windows v5.1

Bản này sửa việc chỉ hiện 'Backend có lỗi'.

GitHub Actions:
- đóng plist.dll + toàn bộ libimobiledevice DLL;
- đóng libcrypto/libssl OpenSSL x64;
- chạy thực tế `sideloader.exe version` ngay trên Windows runner;
- CHỈ upload artifact nếu Sideloader chạy thành công.

Artifact đúng:
IPA-Signer-Pro-Windows-v5.1-TESTED

Trước khi push:
đặt Sideloader Windows tại tools/sideloader.exe

Trong app:
bấm 'Kiểm tra backend' sẽ xóa LOG cũ và hiển thị từng kiểm tra chi tiết.
Nếu artifact v5.1 đã build xanh mà máy cá nhân vẫn không chạy, cài:
Microsoft Visual C++ Redistributable 2015-2022 x64.
