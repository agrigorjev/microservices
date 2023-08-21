namespace Mandara.Entities.Enums
{
    public enum TransferErrorType
    {
        UnknownProductAlias = 1,
        UnknownSecurityDefinition = 2,
        UnknownStripName = 3,
        TradeTransferFailure = 4,
        UnknownPortfolio = 5,
        TradeOnHolidayDate = 6,
        MissingOrInvalidData = 7,
    }
}