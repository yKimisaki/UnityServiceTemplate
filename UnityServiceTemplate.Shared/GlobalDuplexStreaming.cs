using Grpc.Core;
using MessagePack;

namespace UnityServiceTemplate
{
    internal class GlobalDuplexStreaming
    {
        private static readonly Marshaller<GlobalStream> _marshallerRequest = Marshallers.Create(arg => MessagePackSerializer.Serialize(arg), arg => MessagePackSerializer.Deserialize<GlobalStream>(arg));
        private static readonly Marshaller<GlobalStream> _marshallerReply = Marshallers.Create(arg => MessagePackSerializer.Serialize(arg), arg => MessagePackSerializer.Deserialize<GlobalStream>(arg));
        public static readonly Method<GlobalStream, GlobalStream> Method = new Method<GlobalStream, GlobalStream>(
            MethodType.DuplexStreaming,
            "GlobalDuplexStreaming.UnityServiceTemplate",
            "GlobalDuplexStreaming",
            _marshallerRequest,
            _marshallerReply);
    }
}
