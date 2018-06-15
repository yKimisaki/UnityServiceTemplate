using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tonari.UnityServiceTemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new UnityServiceTemplateClient("localhost", 50051))
            {
                while (true)
                {
                    var message = Console.ReadLine();
                    client.SendRequestAsync(message).Wait();
                }
            }
        }
        
        public class UnityServiceTemplateClient : ClientBase<UnityServiceTemplateClient>, IDisposable
        {
            private AsyncDuplexStreamingCall<GlobalStream, GlobalStream> _defaultInvoker;

            public UnityServiceTemplateClient(string host, int port) : base(new Channel(host, port, ChannelCredentials.Insecure))
            {
                this._defaultInvoker = CallInvoker.AsyncDuplexStreamingCall(GlobalDuplexStreaming.Method, null, new CallOptions(null, null, default(CancellationToken)));
            }

            public void Dispose()
            {
                this._defaultInvoker.Dispose();
            }

            protected UnityServiceTemplateClient(ClientBaseConfiguration configuration) : base(configuration) { }

            protected override UnityServiceTemplateClient NewInstance(ClientBaseConfiguration configuration)
            {
                return new UnityServiceTemplateClient(configuration);
            }

            public async Task SendRequestAsync(string request)
            {
                await this._defaultInvoker.RequestStream.WriteAsync(new GlobalStream() { Message = request });
            }
        }
    }
}
