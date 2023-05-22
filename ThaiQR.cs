using ThaiQR.Services;

namespace ThaiQR;
public class ThaiQR
{
    public string MerchantPromptpayQRGenerate(PromptpayType promptpayType, string receiveId, string amount, TransactionUsageType usageType)
    {
        return QRCodeService.MerchantPromptpayQRGenerate(promptpayType, receiveId, amount, usageType);
    }
}
