using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WebBanTrangSuc.Models;

namespace WebBanTrangSuc.Areas.Admin.Views.Orders
{
    public class InvoiceDocument : IDocument
    {
        private readonly Order _order;

        public InvoiceDocument(Order order) => _order = order;

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text($"🧾 HÓA ĐƠN BÁN HÀNG - Mã đơn: #{_order.Id}")
                    .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text($"👤 Khách hàng: {_order.CustomerName}");
                    col.Item().Text($"📞 SĐT: {_order.PhoneNumber}");
                    col.Item().Text($"🏠 Địa chỉ: {_order.ShippingAddress}");
                    col.Item().Text($"🗓 Ngày đặt: {_order.OrderDate:dd/MM/yyyy}");
                    col.Item().Text($"📝 Ghi chú: {_order.Notes}");
                    col.Item().PaddingTop(10);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.ConstantColumn(40);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Sản phẩm").Bold();
                            header.Cell().Text("SL").Bold();
                            header.Cell().Text("Đơn giá").Bold();
                            header.Cell().Text("Thành tiền").Bold();
                        });

                        foreach (var item in _order.OrderDetails)
                        {
                            table.Cell().Text(item.Product.Name);
                            table.Cell().Text(item.Quantity.ToString());
                            table.Cell().Text($"{item.Price:N0} ₫");
                            table.Cell().Text($"{item.Price * item.Quantity:N0} ₫");
                        }
                    });

                    col.Item().AlignRight().PaddingTop(15)
                       .Text($"💰 Tổng cộng: {_order.TotalPrice:N0} ₫").Bold();
                });

                page.Footer()
                    .AlignCenter()
                    .Text(txt => txt.Span("Cảm ơn quý khách đã mua hàng!").Italic());
            });
        }
    }
}
