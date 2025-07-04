﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Bootstrap.Components.Tasks.Progressor.Abstractions.Models;

namespace Bootstrap.Components.Tasks.Progressor.Abstractions
{
    [Obsolete]
    public interface IProgressor
    {
        string Key { get; }

        /// <summary>
        /// For public usage
        /// </summary>
        ProgressorState State { get; }
        object Progress { get; }
        Task Start(object @params, CancellationToken ct);
        Task Stop();
    }
}