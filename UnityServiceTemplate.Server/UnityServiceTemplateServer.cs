using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tonari.UnityServiceTemplate
{
    internal class UnityServiceTemplateServer : IDisposable
    {
        private Server _server;

        public UnityServiceTemplateServer()
        {
            this._server = new Server(new[] { new ChannelOption(ChannelOptions.SoReuseport, 0) })
            {
                Services = { ServerServiceDefinition.CreateBuilder().AddMethod(GlobalDuplexStreaming.Method, this.AsyncDuplexStreaming).Build() },
                Ports = { new ServerPort("0.0.0.0", 50051, ServerCredentials.Insecure) }
            };
            this._server.Start();
        }

        public void Dispose()
        {
            this._server.ShutdownAsync().Wait();
        }

        public async Task AsyncDuplexStreaming(IAsyncStreamReader<GlobalStream> requestStream, IServerStreamWriter<GlobalStream> responseStream, ServerCallContext context)
        {
            Console.WriteLine("クライアントが接続されました");
            while (true)
            {
                Console.WriteLine("\nリクエストを待っています");
                await requestStream.MoveNext(context.CancellationToken).ConfigureAwait(false);

                var request = requestStream.Current;
                Console.WriteLine("リクエストを受信しました: {0}", request.Message);

                var response = new GlobalStream { Message = request.Message + "が届きました。" };
                await responseStream.WriteAsync(response).ConfigureAwait(false);
                Console.WriteLine("レスポンスを送信しました: {0}", response.Message);
            }
        }
    }
}
