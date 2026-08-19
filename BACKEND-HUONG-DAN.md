# HƯỚNG DẪN BACKEND SIDELOADER

IPA Signer Pro là giao diện Windows. Phần ký/cài sử dụng Dadoum/Sideloader.

1. Mở:
   https://github.com/Dadoum/Sideloader/actions
2. Chọn workflow build mới nhất thành công.
3. Tải artifact Windows CLI.
4. Giải nén.
5. Copy `sideloader.exe` và toàn bộ DLL đi kèm vào thư mục `tools` cạnh `IPASignerPro.exe`.
6. Mở IPA Signer Pro và bấm `Kiểm tra backend`.

Theo tài liệu Sideloader, Windows cần các runtime MSVC và các thư viện libimobiledevice/libplist/OpenSSL phù hợp đi kèm backend.

Không nhập Apple ID vào website lạ. GUI này không gửi credentials tới server riêng.
