﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Tasks.Progressor.Abstractions.Models;
using Bootstrap.Components.Tasks.Progressor.Abstractions.Models.Constants;
using Bootstrap.Extensions;

namespace Bootstrap.Components.Tasks.Progressor.Abstractions
{
    public abstract class AbstractProgressor<TProgressModel, TStartParamModel> : IProgressor
        where TProgressModel : ProgressorProgress, new() where TStartParamModel : class
    {
        public virtual string Key => GetType().Name;

        private readonly IProgressDispatcher _progressDispatcher;

        private CancellationTokenSource _internalCts;
        private readonly Stopwatch _sw = new();

        /// <summary>
        /// For public usage
        /// </summary>
        public ProgressorState State { get; } = new();

        public object Progress { get; } = new TProgressModel();

        protected AbstractProgressor(IProgressDispatcher progressDispatcher = null)
        {
            _progressDispatcher = progressDispatcher;
        }

        private async Task UpdateState(Action<ProgressorState> update)
        {
            update(State);
            await DispatchState();
        }

        public async Task UpdateProgress(Action<TProgressModel> update)
        {
            update(Progress as TProgressModel);
            await DispatchProgress();
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

        public async Task Start(TStartParamModel @params, CancellationToken ct)
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
                    await StartCore(@params, mixedCt);
                    _sw.Stop();
                    await UpdateState(a => { a.Status = ProgressorStatus.Complete; });
                    await UpdateProgress(_ => { });
                }
                catch (Exception e)
                {
                    _sw.Stop();
                    string message;
                    if (!mixedCt.IsCancellationRequested)
                    {
                        cts.Cancel();
                        message = e.BuildFullInformationText();
                        // message = e.Message;
                    }
                    else
                    {
                        message = e.Message;
                    }

                    await UpdateState(a =>
                    {
                        a.Status = ProgressorStatus.Suspended;
                        a.Message = message;
                    });
                }
            }
        }

        protected abstract Task StartCore(TStartParamModel @params, CancellationToken ct);

        public Task Start(object @params, CancellationToken ct)
        {
            return Start(@params as TStartParamModel, ct);
        }

        public async Task Stop()
        {
            if (State.Status == ProgressorStatus.Running)
            {
                _sw.Stop();
                _internalCts.Cancel();
                await StopCore();
            }
        }

        protected virtual Task StopCore()
        {
            return Task.CompletedTask;
        }

        private async Task DispatchState()
        {
            if (_progressDispatcher != null)
            {
                await _progressDispatcher.Dispatch(Key, ProgressorEvent.StateChanged, State);
            }
        }

        private async Task DispatchProgress()
        {
            if (_progressDispatcher != null)
            {
                await _progressDispatcher.Dispatch(Key, ProgressorEvent.ProgressChanged, Progress);
            }
        }
    }
}