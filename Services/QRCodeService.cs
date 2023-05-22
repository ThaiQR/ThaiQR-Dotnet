using System.Text;
using NullFX.CRC;

namespace ThaiQR.Services;

public static class QRCodeService
{
    internal static string MerchantPromptpayQRGenerate(PromptpayType promptpayType, string receiveId, string amount, TransactionUsageType usageType)
    {
        switch (promptpayType)
        {
            case PromptpayType.MOBILE_NUMBER:
                receiveId = "0066" + receiveId.Substring(1);
                receiveId = preprocessValue("01", receiveId);
                break;
            case PromptpayType.NATIONAL_ID:
                receiveId = preprocessValue("02", receiveId);
                break;
            case PromptpayType.E_WALLET_ID:
                receiveId = preprocessValue("03", receiveId);
                break;
            case PromptpayType.BANK_ACCOUNT:
                receiveId = preprocessValue("04", receiveId);
                break;
            default:
                receiveId = string.Empty;
                break;
        }

        string pimValue = string.Empty;
        switch (usageType)
        {
            case TransactionUsageType.ONETIME:
                pimValue = "12";
                break;
            case TransactionUsageType.MANY_TIME:
                pimValue = "11";
                break;
            default:
                pimValue = string.Empty;
                break;
        }

        string PFI = preprocessValue("00", "01");
        string PIM = preprocessValue("01", amount);

        #region Merchant Identifier
        string AID = preprocessValue("00", "A000000677010111");
        string merchantSum = AID + receiveId;
        string merchantIdentifier = preprocessValue("29", merchantSum);
        #endregion

        string Currency = preprocessValue("53", "764");
        amount = preprocessAmount(amount);
        string CountryCode = preprocessValue("58", "TH");

        string CRC = "6304";

        string data = PFI + PIM + merchantIdentifier + Currency + amount + CountryCode + CRC;
        var dataBuffer = Encoding.UTF8.GetBytes(data);
        var crc16Result = NullFX.CRC.Crc16.ComputeChecksum(Crc16Algorithm.CcittInitialValue0xFFFF, dataBuffer);

        data += crc16Result.ToString("X");

        return data;
    }

    private static string preprocessValue(string prefix, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            if (value.Length < 10)
                value = prefix + "0" + value.Length.ToString() + value;
            else
                value = prefix + value.Length.ToString() + value;
        }
        else
        {
            value = string.Empty;
        }

        return value;
    }

    private static string preprocessAmount(string amount)
    {
        var check_amount = amount.Split('.');

        if (check_amount.Length > 1)
        {
            if (check_amount[1] == "" || check_amount[1].Count() == 0)
                check_amount[1] = "00";
            else if (check_amount[1].Count() == 1)
                check_amount[1] += '0';
            else if (check_amount[1].Count() > 2)
                check_amount[1] = CharKeep(check_amount[1], 2);

            amount = check_amount[0] + '.' + check_amount[1];
        }
        else if (check_amount.Length == 1)
        {
            amount = amount + '.' + "00";
        }

        amount = preprocessValue("54", amount);

        return amount;
    }

    private static string CharKeep(string str, int index)
    {
        string result = "";
        int i = 0;

        while (i < index)
        {
            char c = str[i];
            result += c;
            i++;
        }

        return result;
    }
}