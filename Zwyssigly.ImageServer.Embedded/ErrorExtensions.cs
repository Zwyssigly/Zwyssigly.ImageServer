namespace Zwyssigly.ImageServer.Embedded
{
    public static class ErrorExtensions
    {
        public static Contracts.Error ToContract(this Error value)
        {
            return value.Message.Match(
                msg => new Contracts.Error((Contracts.ErrorCode)value.Code, msg),
                () => new Contracts.Error((Contracts.ErrorCode)value.Code));
        }
    }
}
