namespace Mandara.Business.Bus.Messages.Base
{
    public enum MessageStatusCode
    {
        Ok = 200,
        UpdateData = 201,
        AddData = 202,
        RemoveData = 203,
        ResetData = 204,

        UpdateOK=400,
        UpdateFailed=401,
        UpdateInProgress=402,
        ResultRequest=403
    }
}