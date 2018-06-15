using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace UnityServiceTemplate
{
    internal class UnityServiceTemplateServer : IDisposable
    {
        private Server _server;

        public UnityServiceTemplateServer()
        {
            this._server = new Server
            {
                Services = { ServerServiceDefinition.CreateBuilder().AddMethod(GlobalDuplexStreaming.Method, this.AsyncDuplexStreaming).Build() },
                Ports = { new ServerPort("localhost", 50001, ServerCredentials.Insecure) }
            };
            this._server.Start();
        }

        public void Dispose()
        {
            this._server.ShutdownAsync().Wait();
        }

        public async Task AsyncDuplexStreaming(IAsyncStreamReader<GlobalStream> requestStream, IServerStreamWriter<GlobalStream> responseStream, ServerCallContext context)
        {
            while (true)
            {
                await requestStream.MoveNext(context.CancellationToken);
                var request = requestStream.Current;
                Console.WriteLine("リクエストを受信しました: {0}", request.Message);

                var response = new GlobalStream { Message = request.Message + "が届きました。" };
                await responseStream.WriteAsync(response);
                Console.WriteLine("レスポンスを送信しました: {0}", response.Message);
            }
        }
    }
}
