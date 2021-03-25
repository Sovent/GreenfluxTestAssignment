using System;
using System.Runtime.Serialization;

namespace Nager.Date.ApiGateway
{
    [Serializable]
    public class NagerDateApiGatewayException : Exception
    {
        public NagerDateApiGatewayException()
        {
        }

        public NagerDateApiGatewayException(string message) : base(message)
        {
        }

        public NagerDateApiGatewayException(string message, Exception inner) : base(message, inner)
        {
        }

        protected NagerDateApiGatewayException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}