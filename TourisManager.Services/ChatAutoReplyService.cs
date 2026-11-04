
using System;
using System.Globalization;
using System.Text;

namespace TourisManager.Services
{
    public class ChatAutoReplyService
    {
        public string GetAutoReply(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return "Bạn cần hỗ trợ thông tin gì về nhà hàng ạ?";
            }

            message = NormalizeText(message);

            // 1. Chào hỏi
            if (ContainsAny(message, "xin chao", "hello", "chao ban",
                "chao nha hang", "chao shop"))
            {
                return "Xin chào bạn! 😊 Bạn muốn tìm hiểu thực đơn, "
                     + "giá cả, đặt bàn hay thông tin gì về nhà hàng ạ?";
            }

            // 2. Giờ mở cửa
            if (ContainsAny(message, "gio mo cua", "may gio mo cua",
                "mo cua luc nao", "dong cua luc nao", "gio hoat dong"))
            {
                return "Dạ, bạn vui lòng xem giờ hoạt động trên trang "
                     + "thông tin nhà hàng nhé. Nếu chưa có thông tin, "
                     + "nhà hàng sẽ xác nhận lại với bạn ạ.";
            }

            // 3. Địa chỉ, đường đi
            if (ContainsAny(message, "dia chi", "o dau", "duong di",
                "chi duong", "gan day khong"))
            {
                return "Dạ, bạn có thể xem địa chỉ và vị trí nhà hàng "
                     + "trên trang chi tiết địa điểm nhé!";
            }

            // 4. Đặt bàn
            if (ContainsAny(message, "dat ban", "giu ban", "con ban khong",
                "dat cho", "dat cho ngoi"))
            {
                return "Dạ, bạn cho nhà hàng biết ngày đến, giờ đến "
                     + "và số lượng người nhé. Nhân viên sẽ kiểm tra "
                     + "và xác nhận tình trạng bàn cho bạn ạ.";
            }

            // 5. Thực đơn, món ăn
            if (ContainsAny(message, "menu", "thuc don", "mon an",
                "co nhung mon gi", "mon dac biet", "mon ban chay"))
            {
                return "Dạ, bạn có thể xem thực đơn trên trang nhà hàng. "
                     + "Bạn đang quan tâm đến món ăn nào để nhân viên "
                     + "tư vấn thêm ạ?";
            }

            // 6. Giá cả
            if (ContainsAny(message, "gia", "bao nhieu tien",
                "gia ca", "gia mot nguoi", "gia trung binh",
                "co dat khong"))
            {
                return "Dạ, giá món ăn có thể khác nhau tùy món và khẩu phần. "
                     + "Bạn cho biết món muốn hỏi giá nhé, nhà hàng sẽ "
                     + "hỗ trợ bạn ạ.";
            }

            // 7. Khuyến mãi
            if (ContainsAny(message, "khuyen mai", "giam gia",
                "uu dai", "voucher", "ma giam gia", "combo"))
            {
                return "Dạ, bạn vui lòng kiểm tra chương trình ưu đãi "
                     + "đang áp dụng trên trang nhà hàng hoặc hỏi nhân viên "
                     + "để được xác nhận điều kiện khuyến mãi nhé!";
            }

            // 8. Số lượng khách, nhóm đông người
            if (ContainsAny(message, "nhom dong nguoi", "doan dong",
                "20 nguoi", "10 nguoi", "di dong nguoi",
                "tiec cong ty", "hop lop", "sinh nhat"))
            {
                return "Dạ, nhà hàng có thể cần sắp xếp chỗ trước "
                     + "cho đoàn đông người. Bạn cho biết số khách, "
                     + "ngày và giờ dự kiến để nhân viên tư vấn nhé!";
            }

            // 9. Chỗ đỗ xe
            if (ContainsAny(message, "cho do xe", "gui xe",
                "bai do xe", "do xe may", "do o to"))
            {
                return "Dạ, thông tin chỗ đỗ xe tùy từng địa điểm. "
                     + "Bạn vui lòng hỏi trực tiếp nhà hàng để xác nhận "
                     + "chỗ gửi xe máy hoặc ô tô nhé!";
            }

            // 10. Trẻ em, ghế trẻ em
            if (ContainsAny(message, "tre em", "tre nho",
                "ghe tre em", "cho be"))
            {
                return "Dạ, bạn vui lòng liên hệ nhà hàng để xác nhận "
                     + "có ghế trẻ em, chỗ ngồi phù hợp hoặc tiện ích "
                     + "dành cho bé không nhé!";
            }

            // 11. Ăn chay
            if (ContainsAny(message, "do chay", "an chay",
                "mon chay", "nguoi an chay"))
            {
                return "Dạ, bạn vui lòng cho biết nhu cầu ăn chay của mình. "
                     + "Nhà hàng sẽ hỗ trợ kiểm tra thực đơn phù hợp ạ.";
            }

            // 12. Dị ứng thực phẩm
            if (ContainsAny(message, "di ung", "di ung hai san",
                "di ung lac", "khong an duoc", "di ung thuc pham"))
            {
                return "Dạ, bạn hãy thông báo rõ loại thực phẩm gây dị ứng "
                     + "cho nhân viên trước khi gọi món. Nhà hàng cần xác "
                     + "nhận thành phần và nguy cơ lẫn chéo trước khi phục vụ ạ.";
            }

            // 13. Thanh toán
            if (ContainsAny(message, "thanh toan", "quet qr",
                "chuyen khoan", "the ngan hang", "tien mat",
                "vi dien tu"))
            {
                return "Dạ, phương thức thanh toán có thể khác nhau "
                     + "tùy nhà hàng. Bạn vui lòng xác nhận nhà hàng "
                     + "có hỗ trợ tiền mặt, chuyển khoản, quét QR "
                     + "hoặc thẻ ngân hàng không nhé!";
            }

            // 14. Hóa đơn
            if (ContainsAny(message, "hoa don", "hoa don do",
                "xuat hoa don", "hoa don vat"))
            {
                return "Dạ, nếu cần hóa đơn VAT hoặc hóa đơn công ty, "
                     + "bạn hãy thông báo với nhà hàng và cung cấp "
                     + "thông tin cần thiết để được hướng dẫn nhé!";
            }

            // 15. Đặt món mang về, giao hàng
            if (ContainsAny(message, "mang ve", "mua mang ve",
                "giao hang", "ship do an", "dat mon online"))
            {
                return "Dạ, bạn vui lòng hỏi nhà hàng xem có hỗ trợ "
                     + "mang về hoặc giao hàng tại khu vực của bạn không nhé!";
            }

            // 16. Không gian, chỗ ngồi
            if (ContainsAny(message, "phong rieng", "phong vip",
                "ngoai troi", "may lanh", "cho ngoi"))
            {
                return "Dạ, bạn cho biết loại chỗ ngồi mong muốn nhé. "
                     + "Nhân viên sẽ xác nhận nhà hàng có phòng riêng, "
                     + "khu vực ngoài trời hoặc chỗ ngồi phù hợp không ạ.";
            }

            // 17. Thời gian chờ
            if (ContainsAny(message, "cho lau khong", "thoi gian cho",
                "len mon bao lau", "phuc vu nhanh"))
            {
                return "Dạ, thời gian chờ phụ thuộc vào lượng khách "
                     + "và món bạn gọi. Bạn có thể liên hệ nhà hàng "
                     + "để được cập nhật tình hình thực tế nhé!";
            }

            // 18. Thú cưng
            if (ContainsAny(message, "thu cung", "cho meo",
                "mang cho", "mang meo"))
            {
                return "Dạ, quy định mang thú cưng tùy từng nhà hàng. "
                     + "Bạn vui lòng xác nhận trước với nhân viên "
                     + "để tránh bất tiện khi đến nhé!";
            }

            // 19. Phàn nàn, góp ý
            if (ContainsAny(message, "khong hai long", "phan nan",
                "do an nguoi", "phuc vu kem", "thai do nhan vien",
                "gop y"))
            {
                return "Dạ, nhà hàng rất tiếc khi trải nghiệm của bạn "
                     + "chưa tốt. Bạn vui lòng chia sẻ cụ thể vấn đề "
                     + "để nhân viên kiểm tra và hỗ trợ nhé!";
            }

            // 20. Cảm ơn
            if (ContainsAny(message, "cam on", "thank you",
                "thanks", "cam on nha hang"))
            {
                return "Dạ, nhà hàng rất vui được hỗ trợ bạn! "
                     + "Chúc bạn có một trải nghiệm thật ngon miệng ạ. 😊";
            }

            // 21. Tạm biệt
            if (ContainsAny(message, "tam biet", "hen gap lai",
                "bye", "chao tam biet"))
            {
                return "Dạ, cảm ơn bạn đã quan tâm đến nhà hàng. "
                     + "Hy vọng sớm được đón tiếp bạn nhé!";
            }

            // Câu hỏi chưa có kịch bản
            return "Dạ, nhà hàng đã nhận được tin nhắn của bạn. "
                 + "Bạn vui lòng nói rõ hơn nhu cầu để được hỗ trợ tốt hơn, "
                 + "hoặc chờ nhân viên phản hồi nhé!";
        }

        // Chuẩn hóa chữ thường và bỏ dấu tiếng Việt
        private string NormalizeText(string text)
        {
            text = text.ToLowerInvariant().Trim().Normalize(
                NormalizationForm.FormD);

            var result = new StringBuilder();

            foreach (char c in text)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);

                if (category != UnicodeCategory.NonSpacingMark)
                {
                    result.Append(c);
                }
            }

            return result.ToString()
                .Replace('đ', 'd')
                .Normalize(NormalizationForm.FormC);
        }

        // Kiểm tra tin nhắn có chứa một trong các từ khóa
        private bool ContainsAny(string message, params string[] keywords)
        {
            foreach (var keyword in keywords)
            {
                if (message.Contains(keyword))
                    return true;
            }

            return false;
        }
    }
}
