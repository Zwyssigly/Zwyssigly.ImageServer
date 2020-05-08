using Zwyssigly.Functional;

namespace Zwyssigly.ImageServer.Contracts
{
    public class Error
    {
        public ErrorCode Code { get; }
        public Option<string> Message { get; }

        public Error(ErrorCode code, string message)
        {
            Code = code;
            Message = Option.Some(message);
        }

        public Error(ErrorCode code)
        {
            Code = code;
        }

        public static implicit operator Error(ErrorCode code) => new Error(code);

        public override string ToString() => Message.Match(m => $"{Code}: {m}", () => Code.ToString());
    }

    public enum ErrorCode
    {
        ImplementationError,
        NoSuchRecord,
        NotSupported,
        ValidationError,
    }
}
