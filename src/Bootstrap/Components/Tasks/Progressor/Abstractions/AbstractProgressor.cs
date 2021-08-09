using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Tasks.Progressor.Abstractions.Models;
using Bootstrap.Components.Tasks.Progressor.Abstractions.Models.Constants;
using Bootstrap.Extensions;

namespace Bootstrap.Components.Tasks.Progressor.Abstractions
{
    public abstract class AbstractProgressor<TProgress> : IProgressor where TProgress : ProgressorProgress, new()
    {
        public virtual string Key => GetType().Name;

        private readonly IProgressNotifier _progressNotifier;

        private CancellationTokenSource _internalCts;
        private readonly Stopwatch _sw = new();

        /// <summary>
        /// For public usage
        /// </summary>
        public ProgressorState State { get; } = new();

        public object Progress { get; } = new TProgress();

        protected AbstractProgressor(IProgressNotifier progressNotifier)
        {
            _progressNotifier = progressNotifier;
        }

        private async Task UpdateState(Action<ProgressorState> update)
        {
            update(State);
            await SendStateToClient();
        }

        public async Task UpdateProgress(Action<TProgress> update)
        {
            update(Progress as TProgress);
            await SendProgressToClient();
        }

        private void StartCountingInBackground(CancellationToken ct)
        {
            Task.Run(async () =>
            {
                while (!ct.IsCancellationRequested)
                {
                    await UpdateProgress(a => a.ElapsedMilliseconds = _sw.ElapsedMilliseconds);
                    await Task.Delay(1000, ct);
                }
            }, ct);
        }

        public async Task Start(string jsonParams, CancellationToken ct)
        {
            if (State.Status.CanStart())
            {
                _sw.Restart();
                await UpdateState(a =>
                {
                    a.Status = ProgressorStatus.Running;
                    a.Message = default;
                });

                var cts = _internalCts = new CancellationTokenSource();
                var mixedCt = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ct).Token;

                try
                {
                    StartCountingInBackground(mixedCt);
                    await StartInternal(jsonParams, mixedCt);
                    _sw.Stop();
                    cts.Cancel();
                    await UpdateState(a => { a.Status = ProgressorStatus.Complete; });
                    await UpdateProgress(_ => { });
                }
                catch (Exception e)
                {
                    _sw.Stop();
                    string message = null;
                    if (!mixedCt.IsCancellationRequested)
                    {
                        cts.Cancel();
                        message = e.BuildFullInformationText();
                    }

                    await UpdateState(a =>
                    {
                        a.Status = ProgressorStatus.Suspended;
                        a.Message = message;
                    });
                }
            }
        }

        protected abstract Task StartInternal(string jsonParams, CancellationToken ct);

        public async Task Stop()
        {
            if (State.Status == ProgressorStatus.Running)
            {
                _sw.Stop();
                _internalCts.Cancel();
                await StopInternal();
                State.Status = ProgressorStatus.Idle;
                State.Message = default;
                await SendStateToClient();
            }
        }

        protected virtual Task StopInternal()
        {
            return Task.CompletedTask;
        }

        private async Task SendStateToClient()
        {
            await _progressNotifier.Send(ProcessorClientMethod.State, Key, State);
        }

        private async Task SendProgressToClient()
        {
            await _progressNotifier.Send(ProcessorClientMethod.Progress, Key, Progress);
        }
    }
}