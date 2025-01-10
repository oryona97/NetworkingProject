using System.Text;
using System.Web;

public class EmailTemplateGenerator
{
    private readonly string _storeName;
    private readonly string _storeUrl;
    private readonly string _supportEmail;

    public EmailTemplateGenerator(string storeName, string storeUrl, string supportEmail)
    {
        _storeName = storeName;
        _storeUrl = storeUrl;
        _supportEmail = supportEmail;
    }

    public string GenerateOrderConfirmationEmail(string customerName, List<CartItem> items, string orderId, string currency)
    {
        var total = items.Sum(item => item.Amount / 100m);

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            text-align: center;
            padding: 20px;
            background-color: #f8f9fa;
            margin-bottom: 20px;
        }}
        .order-details {{
            background-color: #ffffff;
            padding: 20px;
            border: 1px solid #dee2e6;
            border-radius: 5px;
        }}
        .item {{
            margin-bottom: 15px;
            padding-bottom: 15px;
            border-bottom: 1px solid #dee2e6;
        }}
        .total {{
            font-size: 18px;
            font-weight: bold;
            text-align: right;
            margin-top: 20px;
            padding-top: 20px;
            border-top: 2px solid #dee2e6;
        }}
        .footer {{
            margin-top: 30px;
            text-align: center;
            font-size: 14px;
            color: #6c757d;
        }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>{_storeName}</h1>
        <p>Order Confirmation #{orderId}</p>
    </div>

    <p>Dear {customerName},</p>
    <p>Thank you for your purchase! Your order has been confirmed and your books are now available in your personal library.</p>

    <div class='order-details'>
        <h2>Order Summary</h2>
        {GenerateItemsHtml(items, currency)}
        <div class='total'>
            Total: {total.ToString("C")} {currency}
        </div>
    </div>

    <div class='access-instructions'>
        <h3>Next Steps</h3>
        <ol>
            <li>Visit your <a href='{_storeUrl}/library'>personal library</a> to access your books</li>
            <li>Download your preferred format (PDF, EPUB, or MOBI)</li>
            <li>Start reading and enjoy!</li>
        </ol>
    </div>

    <div class='footer'>
        <p>If you have any questions about your order, please contact us at {_supportEmail}</p>
        <p>© {DateTime.Now.Year} {_storeName}. All rights reserved.</p>
    </div>
</body>
</html>";
    }

    private string GenerateItemsHtml(List<CartItem> items, string currency)
    {
        var itemsHtml = new StringBuilder();
        foreach (var item in items)
        {
            itemsHtml.Append($@"
                <div class='item'>
                    <h3>{HttpUtility.HtmlEncode(item.ProductName)}</h3>
                    <p>{HttpUtility.HtmlEncode(item.ProductDescription)}</p>
                    <p style='text-align: right;'>{(item.Amount / 100m).ToString("C")} {currency}</p>
                </div>");
        }
        return itemsHtml.ToString();
    }
}
