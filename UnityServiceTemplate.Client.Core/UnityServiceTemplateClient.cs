using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx;

namespace UnityServiceTemplate
{
    public class UnityServiceTemplateClient : ClientBase<UnityServiceTemplateClient>, IDisposable
    {
        private AsyncDuplexStreamingCall<GlobalStream, GlobalStream> _defaultInvoker;
        private SingleAssignmentDisposable _subscription;

        public UnityServiceTemplateClient() : base(new Channel("localhost:50001", ChannelCredentials.Insecure))
        {
            this._defaultInvoker = CallInvoker.AsyncDuplexStreamingCall(GlobalDuplexStreaming.Method, null, new CallOptions(null, null, default(CancellationToken)));
            this._subscription = new SingleAssignmentDisposable();
            this._subscription.Disposable = this.WaitReceiveAsync()
                .ToObservable()
                .Subscribe();
        }

        private async Task WaitReceiveAsync()
        {
            await this._defaultInvoker.ResponseStream.MoveNext(CancellationToken.None);
            this._onReceiveResponse.OnNext(this._defaultInvoker.ResponseStream.Current.Message);

            await WaitReceiveAsync();
        }

        public void Dispose()
        {
            this._subscription.Dispose();
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

        private Subject<string> _onReceiveResponse = new Subject<string>();
        public IObservable<string> OnReceiveResponseAsObservable()
        {
            return _onReceiveResponse;
        }
    }
}
