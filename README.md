# IPA Signer Pro - Windows

Ứng dụng Windows giao diện đơn giản để ký/cài IPA lên iPhone.

## Kiến trúc
GUI này KHÔNG tự triển khai lại giao thức Apple. Nó gọi backend mã nguồn mở `Dadoum/Sideloader`.

Sideloader có chức năng:
- lấy development certificate;
- quản lý App ID / device;
- ký IPA;
- cài app vào iPhone;
- hỗ trợ tài khoản developer miễn phí.

## Bảo mật
- GUI không lưu Apple ID hoặc mật khẩu.
- Thông tin đăng nhập được chuyển trực tiếp vào tiến trình backend đang chạy.
- Không có server riêng của ứng dụng này.

## Build EXE
Upload toàn bộ source lên GitHub.
Vào Actions > `Build IPA Signer Pro Windows` > Run workflow.
Tải Artifact `IPA-Signer-Pro-Windows`.

## Trước khi dùng
Cần tải bản Windows của Sideloader và các DLL runtime đi kèm từ project chính thức:
https://github.com/Dadoum/Sideloader/actions

Đặt chúng vào:
`tools\`

Tối thiểu GUI cần:
`tools\sideloader.exe`

Nếu backend phát hành kèm các DLL, phải đặt chúng cùng thư mục với `sideloader.exe`.

## Cách dùng
1. Cắm iPhone bằng USB.
2. Mở khóa iPhone và Trust máy tính.
3. Chọn IPA.
4. Bấm `KÝ & CÀI VÀO IPHONE`.
5. Khi log yêu cầu Apple ID / password / 2FA, nhập ở ô dưới và gửi.
6. Sau khi cài, iOS 16+ cần bật Developer Mode nếu chưa bật.

Với Apple ID miễn phí, ứng dụng ký bằng Personal Team có thời hạn ngắn và cần ký lại định kỳ.


## Thay đổi v2
- Sửa khung LOG bị co nhỏ.
- Thêm 3 ô riêng: Apple ID, mật khẩu, mã 2FA.
- Mật khẩu hiển thị dạng ẩn.
- Mật khẩu và 2FA tự xóa khỏi ô sau khi gửi.
- Chỉ gửi dữ liệu khi backend đang chạy và người dùng bấm nút.
