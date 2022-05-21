namespace Eventnet.Api.Models.Authentication;

public class VerifyPasswordCodeResultModel
{
    public bool Status { get; }

    public VerifyPasswordCodeResultModel(bool status)
    {
        Status = status;
    }
}