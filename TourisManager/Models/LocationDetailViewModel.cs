namespace TourisManager.Models;

public record ReviewItem(string Author, int Rating, string Date, string Content);

public class LocationExtra
{
    public string Description { get; set; } = "Thông tin đang được cập nhật.";
    public string OpenHours { get; set; } = "Đang cập nhật";
    public string Price { get; set; } = "Đang cập nhật";
    public string BestTime { get; set; } = "Quanh năm";
    public List<string> Highlights { get; set; } = new();
    public string Tip { get; set; } = "";
    public List<ReviewItem> Reviews { get; set; } = new();

    public double AvgRating => Reviews.Count == 0 ? 0 : Math.Round(Reviews.Average(r => r.Rating), 1);
}

public class LocationDetailViewModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string CategoryName { get; set; } = "Địa điểm";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? ImageUrl { get; set; }
    public LocationExtra Extra { get; set; } = new();
}

/// <summary>
/// Dữ liệu mô tả + đánh giá MẪU cho loc-01 … loc-20.
/// Giá vé / giờ mở cửa chỉ mang tính tham khảo, hãy kiểm tra lại trước khi dùng thật.
/// Sau này nên chuyển vào database (bảng Location + bảng Review).
/// </summary>
public static class LocationContent
{
    public static LocationExtra Get(string? id) =>
        id != null && Data.TryGetValue(id.ToLower(), out var x) ? x : new LocationExtra();

    private static LocationExtra Make(string desc, string hours, string price, string best,
        string highlights, string tip, params ReviewItem[] reviews) => new()
        {
            Description = desc,
            OpenHours = hours,
            Price = price,
            BestTime = best,
            Highlights = highlights.Split('|').ToList(),
            Tip = tip,
            Reviews = reviews.ToList()
        };

    private static ReviewItem R(string a, int s, string d, string c) => new(a, s, d, c);

    private static readonly Dictionary<string, LocationExtra> Data = new()
    {
        ["a"] = Make(
            "Trái tim của thủ đô với mặt hồ xanh bao quanh Tháp Rùa. Cầu Thê Húc màu đỏ dẫn vào đền Ngọc Sơn, nơi thờ Văn Xương Đế Quân và Trần Hưng Đạo.",
            "Hồ: cả ngày · Đền: khoảng 7:00 – 18:00", "Hồ miễn phí · Vé vào đền khoảng 30.000đ",
            "Sáng sớm hoặc cuối tuần (phố đi bộ)", "Tháp Rùa|Cầu Thê Húc|Phố đi bộ quanh hồ",
            "Cuối tuần phố đi bộ rất đông, nên đến trước 7h sáng để chụp ảnh thoáng.",
            R("Minh Anh", 5, "12/09/2026", "Đi dạo quanh hồ buổi sáng rất dễ chịu, không khí trong lành."),
            R("Quốc Bảo", 4, "03/09/2026", "Cảnh đẹp, cuối tuần hơi đông nhưng không khí rất vui.")),

        ["loc-02"] = Make(
            "Quần thể di tích gắn với kinh thành Thăng Long qua nhiều triều đại, được UNESCO công nhận là Di sản Văn hóa Thế giới.",
            "Khoảng 8:00 – 17:00", "Khoảng 30.000đ",
            "Buổi sáng, thời tiết mát", "Đoan Môn|Khu khảo cổ 18 Hoàng Diệu|Cột cờ Hà Nội",
            "Nên thuê hướng dẫn viên hoặc nghe thuyết minh để hiểu rõ các lớp di tích.",
            R("Thu Hà", 5, "20/08/2026", "Rất đáng đi cho ai thích lịch sử, khu khảo cổ được trưng bày kỹ."),
            R("Hoàng Nam", 4, "02/08/2026", "Không gian rộng, yên tĩnh, tuy nhiên nên đi sáng để tránh nắng.")),

        ["loc-03"] = Make(
            "Trường đại học đầu tiên của Việt Nam, xây dựng từ năm 1070 với khuôn viên cổ kính, nổi tiếng với Khuê Văn Các và bia tiến sĩ.",
            "Khoảng 8:00 – 17:00", "Khoảng 70.000đ",
            "Mùa thi cử và dịp Tết", "Khuê Văn Các|82 bia tiến sĩ|Sân Thái Học",
            "Mùa thi và dịp Tết rất đông, nên đến ngay khi mở cửa.",
            R("Lan Phương", 5, "15/09/2026", "Kiến trúc đẹp, rất hợp chụp ảnh kỷ yếu và tìm hiểu văn hóa."),
            R("Tuấn Kiệt", 4, "28/08/2026", "Không gian yên bình giữa phố, bia tiến sĩ rất ấn tượng.")),

        ["loc-04"] = Make(
            "Công trình tưởng niệm Chủ tịch Hồ Chí Minh tại Quảng trường Ba Đình, nơi diễn ra nghi thức thượng cờ - hạ cờ mỗi ngày.",
            "Sáng khoảng 7:30 – 10:30 (đóng cửa thứ Hai, thứ Sáu)", "Miễn phí",
            "Buổi sáng sớm", "Quảng trường Ba Đình|Nhà sàn Bác Hồ|Chùa Một Cột",
            "Lăng thường đóng cửa bảo trì định kỳ vào cuối năm, hãy kiểm tra lịch trước khi đi. Ăn mặc lịch sự, không mang máy ảnh vào trong lăng.",
            R("Ngọc Mai", 5, "10/09/2026", "Không khí trang nghiêm, tổ chức rất trật tự và chu đáo."),
            R("Đức Huy", 4, "21/08/2026", "Nên đi sớm vì hàng người khá dài, nhớ mang giấy tờ tùy thân.")),

        ["loc-05"] = Make(
            "Ngôi chùa cổ nhất Hà Nội với hơn 1.500 năm tuổi, nằm trên bán đảo nhỏ giữa Hồ Tây, nổi bật với tháp Bảo Tháp 11 tầng.",
            "Khoảng 8:00 – 17:00", "Miễn phí",
            "Chiều muộn ngắm hoàng hôn Hồ Tây", "Bảo tháp 11 tầng|Cây bồ đề|Cảnh hoàng hôn Hồ Tây",
            "Mặc trang phục kín đáo, giữ yên tĩnh khi vào khu thờ.",
            R("Thảo Vy", 5, "05/09/2026", "Cảnh hồ rất đẹp, chùa thanh tịnh, đi chiều là hợp lý nhất."),
            R("Gia Hân", 5, "18/08/2026", "Một nơi yên bình để tạm rời xa ồn ào của thành phố.")),

        ["loc-06"] = Make(
            "Nhà thờ kiến trúc Gothic xây cuối thế kỷ 19 ngay trung tâm phố cổ, xung quanh là dãy quán cà phê và điểm check-in nổi tiếng.",
            "Mở cửa ngoài giờ lễ (kiểm tra giờ lễ tại chỗ)", "Miễn phí",
            "Chiều tối, dịp lễ Giáng sinh", "Mặt tiền Gothic|Phố Nhà Chung|Quán cà phê view nhà thờ",
            "Giữ trật tự và không vào bên trong khi đang có lễ.",
            R("Bảo Châu", 4, "11/09/2026", "Check-in đẹp, ngồi cà phê nhìn nhà thờ rất chill."),
            R("Văn Long", 5, "30/08/2026", "Kiến trúc ấn tượng, nhất là về đêm khi lên đèn.")),

        ["loc-07"] = Make(
            "Chợ đầu mối lớn nhất Hà Nội, bán sỉ lẻ đủ loại hàng từ vải vóc, đồ lưu niệm đến thực phẩm khô.",
            "Khoảng 6:00 – 18:00", "Miễn phí vào cửa",
            "Buổi sáng", "Tầng 1 đồ lưu niệm|Khu ẩm thực|Phố Hàng Giày lân cận",
            "Hãy mặc cả và giữ kỹ tư trang khi đi chợ đông.",
            R("Khánh Linh", 4, "07/09/2026", "Hàng hóa đa dạng, giá mềm hơn các cửa hàng trong phố cổ."),
            R("Anh Tuấn", 4, "25/08/2026", "Rất náo nhiệt, mua quà về làm quà tặng khá tiện.")),

        ["loc-08"] = Make(
            "Cây cầu thép lịch sử bắc qua sông Hồng, hơn 120 năm tuổi, là chứng nhân của nhiều giai đoạn lịch sử Thủ đô.",
            "Cả ngày", "Miễn phí",
            "Hoàng hôn", "Kiến trúc thép cổ|Ngắm sông Hồng|Bãi giữa sông Hồng",
            "Cầu có tàu hỏa và xe máy chạy chung, hãy cẩn thận khi đi bộ.",
            R("Mai Trang", 5, "13/09/2026", "Hoàng hôn trên cầu cực đẹp, rất hợp chụp ảnh."),
            R("Phúc Lâm", 4, "27/08/2026", "Cảnh hoài cổ, nhưng nên chú ý an toàn giao thông.")),

        ["loc-09"] = Make(
            "Bảo tàng giới thiệu 54 dân tộc Việt Nam với khu trưng bày trong nhà và nhiều ngôi nhà truyền thống ngoài trời.",
            "Khoảng 8:30 – 17:30 (đóng cửa thứ Hai)", "Khoảng 40.000đ",
            "Cuối tuần có hoạt động văn hóa", "Nhà rông Tây Nguyên|Nhà mồ|Múa rối nước",
            "Dành ít nhất 2 giờ để tham quan cả khu trong nhà và ngoài trời.",
            R("Hải Yến", 5, "09/09/2026", "Rất đáng để dẫn trẻ nhỏ đi, vừa học vừa chơi."),
            R("Công Minh", 5, "22/08/2026", "Khu ngoài trời đẹp, hiện vật được trưng bày rất có tâm.")),

        ["loc-10"] = Make(
            "Khu 36 phố phường nổi tiếng với những con phố nghề cổ, ẩm thực đường phố và kiến trúc nhà ống đặc trưng.",
            "Cả ngày (sôi động nhất chiều tối)", "Miễn phí",
            "Tối cuối tuần", "Ẩm thực đường phố|Chợ đêm cuối tuần|Nhà cổ 87 Mã Mây",
            "Đi bộ là cách tốt nhất để khám phá, nên mang giày thoải mái.",
            R("Thanh Tâm", 5, "14/09/2026", "Đồ ăn ngon, mỗi con phố một nét riêng, đi hoài không chán."),
            R("Việt Anh", 4, "01/09/2026", "Hơi đông và ồn nhưng đáng để trải nghiệm.")),

        ["loc-11"] = Make(
            "Khu rừng nguyên sinh dưới chân núi Ba Vì với khí hậu mát mẻ, phù hợp trekking, picnic và nghỉ cuối tuần.",
            "Khoảng 7:00 – 17:00", "Vé vào cổng khoảng 60.000đ (tham khảo)",
            "Từ tháng 4 đến tháng 10", "Đền Thượng|Rừng thông|Đỉnh Tản Viên",
            "Mang áo khoác mỏng vì trên núi khá lạnh, đi sớm để tránh đông.",
            R("Minh Anh", 5, "29/08/2026", "Không khí mát, thích hợp đi nghỉ ngắn ngày."),
            R("Quốc Bảo", 4, "12/08/2026", "Đường lên núi đẹp, nên đi xe riêng cho tiện.")),

        ["loc-12"] = Make(
            "Làng cổ với hơn 800 ngôi nhà cổ xây bằng đá ong, cổng làng và đình cổ, nổi bật với đặc sản chè lam, kẹo dồi.",
            "Cả ngày", "Khoảng 30.000đ",
            "Mùa thu, ngày cuối tuần", "Cổng làng Mông Phụ|Nhà cổ đá ong|Chè lam, kẹo dồi",
            "Nên thuê xe đạp để đi quanh làng cho thoải mái.",
            R("Thu Hà", 5, "03/09/2026", "Yên bình như bước vào làng quê xưa, ăn chè lam rất ngon."),
            R("Hoàng Nam", 4, "17/08/2026", "Đẹp và gần Hà Nội, phù hợp đi trong ngày.")),

        ["loc-13"] = Make(
            "Di sản thiên nhiên thế giới với hàng nghìn đảo đá vôi, hang động kỳ vĩ và du thuyền nghỉ đêm trên vịnh.",
            "Tour thường 7:30 – 16:00 hoặc qua đêm", "Vé tham quan vịnh từ khoảng 290.000đ (chưa gồm tour)",
            "Tháng 3 – 5 và 9 – 11", "Hang Sửng Sốt|Đảo Ti Tốp|Du thuyền ngủ đêm",
            "Đặt tour trước và kiểm tra thời tiết, biển động có thể hủy lịch.",
            R("Lan Phương", 5, "26/08/2026", "Cảnh đẹp mê hồn, ngủ đêm trên vịnh là trải nghiệm đáng nhớ."),
            R("Tuấn Kiệt", 5, "10/08/2026", "Dịch vụ tàu tốt, nên đặt sớm vào dịp lễ.")),

        ["loc-14"] = Make(
            "Quần thể danh thắng với sông, hang động xuyên núi và núi đá vôi, ngồi thuyền khám phá như bức tranh thủy mặc.",
            "Khoảng 7:00 – 15:00", "Khoảng 250.000đ/người (tham khảo)",
            "Tháng 5 – 6 (mùa lúa chín)", "Hang Địa Linh|Hang Sáng|Đền Trình",
            "Đội mũ và mang áo mưa vì thuyền lộ thiên.",
            R("Ngọc Mai", 5, "08/09/2026", "Ngồi thuyền xuyên hang cực thích, cảnh rất yên."),
            R("Đức Huy", 5, "19/08/2026", "Rất đáng đi, nên đi sớm để tránh đông.")),

        ["loc-15"] = Make(
            "Đỉnh núi cao nhất Việt Nam (3.143m) với cáp treo đến gần đỉnh, khung cảnh biển mây hùng vĩ.",
            "Khoảng 7:30 – 17:30", "Cáp treo khoảng 800.000đ – 900.000đ (tham khảo)",
            "Tháng 9 – 11 và 3 – 4", "Cáp treo Fansipan|Cột mốc đỉnh 3.143m|Biển mây",
            "Trên đỉnh lạnh và gió mạnh, mang áo ấm dù trời dưới phố nắng.",
            R("Thảo Vy", 5, "02/09/2026", "Săn mây đỉnh Fansipan thật tuyệt, nên đi sáng sớm."),
            R("Gia Hân", 4, "16/08/2026", "Cảnh đẹp nhưng đông, nhớ mang đồ ấm.")),

        ["loc-16"] = Make(
            "Thương cảng cổ được UNESCO công nhận, nổi bật với nhà cổ, đèn lồng, ẩm thực cao lầu, bánh mì và cơm gà.",
            "Cả ngày (đẹp nhất tối)", "Vé tham quan phố cổ khoảng 120.000đ (tham khảo)",
            "Đêm rằm hằng tháng", "Chùa Cầu|Nhà cổ Tấn Ký|Thả đèn hoa đăng sông Hoài",
            "Nên thuê xe đạp và thử thả đèn trên sông Hoài buổi tối.",
            R("Bảo Châu", 5, "20/09/2026", "Phố cổ lung linh về đêm, đồ ăn rất ngon."),
            R("Văn Long", 5, "04/09/2026", "Rất đáng để ở lại ít nhất 2 ngày.")),

        ["loc-17"] = Make(
            "Cây cầu nổi tiếng với đôi bàn tay khổng lồ nâng đỡ, nằm trong khu du lịch Bà Nà Hills trên đỉnh núi.",
            "Khoảng 8:00 – 22:00 (tùy ngày)", "Vé Bà Nà Hills khoảng 900.000đ (tham khảo, gồm cáp treo)",
            "Sáng sớm để tránh đông", "Cầu Vàng|Làng Pháp|Fantasy Park",
            "Đến sớm trước 9h để chụp ảnh cầu ít người.",
            R("Khánh Linh", 5, "24/08/2026", "Khung cảnh như cổ tích, chụp ảnh cực đẹp."),
            R("Anh Tuấn", 4, "06/08/2026", "Rất đông vào cuối tuần, vé hơi đắt nhưng đáng trải nghiệm.")),

        ["loc-18"] = Make(
            "Cố đô triều Nguyễn với Đại Nội, lăng tẩm các vua và dòng sông Hương thơ mộng, di sản văn hóa thế giới.",
            "Khoảng 8:00 – 17:00", "Vé Đại Nội khoảng 200.000đ (tham khảo)",
            "Tháng 1 – 4", "Đại Nội|Lăng Khải Định|Chùa Thiên Mụ",
            "Mùa hè nắng gắt, nên đi sáng sớm hoặc chiều muộn.",
            R("Mai Trang", 5, "11/09/2026", "Không gian lịch sử sâu lắng, nên thuê xích lô hoặc xe điện."),
            R("Phúc Lâm", 4, "23/08/2026", "Rất đẹp, nhưng nên dành cả ngày để tham quan hết.")),

        ["loc-19"] = Make(
            "Bãi biển nước trong, cát trắng mịn ở phía Nam đảo Phú Quốc, nổi tiếng với hoàng hôn và nước biển xanh ngọc.",
            "Cả ngày", "Miễn phí (có thể mất phí ghế, dịch vụ)",
            "Tháng 11 – 4", "Cát trắng mịn|Nước biển trong xanh|Hoàng hôn",
            "Mang kem chống nắng và đi tour 4 đảo để kết hợp lặn ngắm san hô.",
            R("Hải Yến", 5, "15/09/2026", "Biển đẹp nhất mình từng thấy, cát rất mịn."),
            R("Công Minh", 4, "01/09/2026", "Nước trong, hơi nhiều khách vào cuối tuần.")),

        ["loc-20"] = Make(
            "Chợ nổi sầm uất bậc nhất miền Tây, nơi ghe thuyền buôn bán nông sản và ẩm thực ngay trên sông.",
            "Khoảng 5:00 – 8:00 sáng", "Thuê thuyền khoảng 150.000đ – 300.000đ (tham khảo)",
            "Sáng sớm", "Ghe bán trái cây|Ăn hủ tiếu trên thuyền|Cây bẹo treo hàng",
            "Đặt thuyền từ chiều hôm trước, đến sớm để thấy chợ đông nhất.",
            R("Thanh Tâm", 5, "09/09/2026", "Trải nghiệm rất đặc trưng miền Tây, ăn sáng trên sông cực vui."),
            R("Việt Anh", 4, "21/08/2026", "Nên đi sớm vì chợ vãn rất nhanh sau 8h.")),
    };
}