﻿// <copyright file="FuncTaskExtensions.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>

namespace TaskSample.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public static class FuncTaskExtensions
    {
        public static async Task<T> FirstAsync<T>(this IEnumerable<Func<CancellationToken, Task<T>>> funcs, Predicate<T> pred)
        {
            var tasks = new List<Task>();
            using (MatchFunc<T> match = new MatchFunc<T>(pred))
            {
                foreach (Func<CancellationToken, Task<T>> func in funcs)
                {
                    if (match.Canceled)
                    {
                        break;
                    }

                    Task task = match.RunAsync(func);
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
                return match.Result;
            }
        }

        private sealed class MatchFunc<T> : IDisposable
        {
            private readonly CancellationTokenSource cts;
            private readonly Predicate<T> pred;

            private Tuple<T> firstResult;

            public MatchFunc(Predicate<T> pred)
            {
                this.cts = new CancellationTokenSource();
                this.pred = pred;
            }

            public bool Canceled => this.cts.IsCancellationRequested;

            public T Result
            {
                get
                {
                    if (this.firstResult == null)
                    {
                        throw new InvalidOperationException("No matching result.");
                    }

                    return this.firstResult.Item1;
                }
            }

            public async Task RunAsync(Func<CancellationToken, Task<T>> func)
            {
                try
                {
                    T result = await func(this.cts.Token);
                    if (this.pred(result))
                    {
                        this.Complete(result);
                    }
                }
                catch (Exception)
                {
                }
            }

            public void Dispose()
            {
                this.cts.Dispose();
            }

            private void Complete(T result)
            {
                if (Interlocked.CompareExchange(ref this.firstResult, Tuple.Create(result), null) == null)
                {
                    this.cts.Cancel();
                }
            }
        }
    }
}
