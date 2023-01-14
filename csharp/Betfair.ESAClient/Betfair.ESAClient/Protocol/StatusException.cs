using Betfair.ESASwagger.Model;
using System;

namespace Betfair.ESAClient.Protocol
{
    /// <summary>
    /// Exception used by api to raise a status fail.
    /// </summary>
    public class StatusException : Exception
    {
        public readonly StatusMessage.ErrorCodeEnum ErrorCode;
        public readonly string ErrorMessage;

        public StatusException(StatusMessage message) : base(message.ErrorCode +": " +message.ErrorMessage)
        {
            ErrorCode = (StatusMessage.ErrorCodeEnum)message.ErrorCode;
            ErrorMessage = message.ErrorMessage;
        }
    }
}
