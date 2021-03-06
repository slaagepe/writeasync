﻿//-----------------------------------------------------------------------
// <copyright file="CleanupTest.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CleanupSample.Test.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class CleanupTest
    {
        public CleanupTest()
        {
        }

        [Fact]
        public void Should_execute_delegate_and_pass_self_as_param()
        {
            CleanupGuard guard = new CleanupGuard();
            bool executed = false;
            Func<CleanupGuard, Task> doAsync = delegate(CleanupGuard g)
            {
                Assert.Same(guard, g);
                executed = true;
                return Task.FromResult(false);
            };

            Task task = guard.RunAsync(doAsync);

            Assert.True(executed);
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
        }

        [Fact]
        public void Should_execute_cleanup_steps_in_opposite_order_after_delegate()
        {
            CleanupGuard guard = new CleanupGuard();
            List<int> steps = new List<int>();
            Func<int, Task> doStepAsync = delegate(int i)
            {
                steps.Add(i);
                return Task.FromResult(false);
            };

            guard.Register(() => doStepAsync(1));
            guard.Register(() => doStepAsync(2));

            bool executed = false;
            Func<CleanupGuard, Task> doAsync = delegate(CleanupGuard g)
            {
                executed = true;
                return Task.FromResult(false);
            };

            Task task = guard.RunAsync(doAsync);

            Assert.True(executed);
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.Equal(new int[] { 2, 1 }, steps.ToArray());
        }

        [Fact]
        public void Should_execute_cleanup_steps_on_sync_exception_in_delegate_and_complete_with_exception()
        {
            CleanupGuard guard = new CleanupGuard();
            List<int> steps = new List<int>();
            Func<int, Task> doStepAsync = delegate(int i)
            {
                steps.Add(i);
                return Task.FromResult(false);
            };

            guard.Register(() => doStepAsync(1));
            guard.Register(() => doStepAsync(2));

            InvalidTimeZoneException expectedException = new InvalidTimeZoneException("Expected.");
            Func<CleanupGuard, Task> doAsync = delegate(CleanupGuard g)
            {
                throw expectedException;
            };

            Task task = guard.RunAsync(doAsync);

            Assert.Equal(TaskStatus.Faulted, task.Status);
            Assert.NotNull(task.Exception);
            AggregateException ae = Assert.IsType<AggregateException>(task.Exception).Flatten();
            Assert.Equal(1, ae.InnerExceptions.Count);
            Assert.Same(expectedException, ae.InnerExceptions[0]);
            Assert.Equal(new int[] { 2, 1 }, steps.ToArray());
        }

        [Fact]
        public void Should_execute_cleanup_steps_on_async_exception_in_delegate_and_complete_with_exception()
        {
            CleanupGuard guard = new CleanupGuard();
            List<int> steps = new List<int>();
            Func<int, Task> doStepAsync = delegate(int i)
            {
                steps.Add(i);
                return Task.FromResult(false);
            };

            guard.Register(() => doStepAsync(1));
            guard.Register(() => doStepAsync(2));

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Task task = guard.RunAsync(g => tcs.Task);

            Assert.False(task.IsCompleted);

            InvalidTimeZoneException expectedException = new InvalidTimeZoneException("Expected.");
            tcs.SetException(expectedException);

            Assert.Equal(TaskStatus.Faulted, task.Status);
            Assert.NotNull(task.Exception);
            AggregateException ae = Assert.IsType<AggregateException>(task.Exception).Flatten();
            Assert.Equal(1, ae.InnerExceptions.Count);
            Assert.Same(expectedException, ae.InnerExceptions[0]);
            Assert.Equal(new int[] { 2, 1 }, steps.ToArray());
        }

        [Fact]
        public void Should_execute_all_cleanup_steps_despite_sync_exception_in_cleanup_and_complete_with_exception()
        {
            CleanupGuard guard = new CleanupGuard();
            List<int> steps = new List<int>();
            Func<int, Exception, Task> doStepAsync = delegate(int i, Exception e)
            {
                steps.Add(i);
                if (e != null)
                {
                    throw e;
                }

                return Task.FromResult(false);
            };

            InvalidTimeZoneException expectedException = new InvalidTimeZoneException("Expected.");

            guard.Register(() => doStepAsync(1, null));
            guard.Register(() => doStepAsync(2, expectedException));

            Task task = guard.RunAsync(g => Task.FromResult(false));

            Assert.Equal(TaskStatus.Faulted, task.Status);
            Assert.NotNull(task.Exception);
            AggregateException ae = Assert.IsType<AggregateException>(task.Exception).Flatten();
            Assert.Equal(1, ae.InnerExceptions.Count);
            Assert.Same(expectedException, ae.InnerExceptions[0]);
            Assert.Equal(new int[] { 2, 1 }, steps.ToArray());
        }

        [Fact]
        public void Should_execute_all_cleanup_steps_despite_async_exception_in_cleanup_and_complete_with_exception()
        {
            CleanupGuard guard = new CleanupGuard();
            List<int> steps = new List<int>();
            Func<int, Task, Task> doStepAsync = delegate(int i, Task t)
            {
                steps.Add(i);
                return t;
            };

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            guard.Register(() => doStepAsync(1, Task.FromResult(false)));
            guard.Register(() => doStepAsync(2, tcs.Task));

            Task task = guard.RunAsync(g => Task.FromResult(false));

            Assert.False(task.IsCompleted);

            InvalidTimeZoneException expectedException = new InvalidTimeZoneException("Expected.");
            tcs.SetException(expectedException);

            Assert.Equal(TaskStatus.Faulted, task.Status);
            Assert.NotNull(task.Exception);
            AggregateException ae = Assert.IsType<AggregateException>(task.Exception).Flatten();
            Assert.Equal(1, ae.InnerExceptions.Count);
            Assert.Same(expectedException, ae.InnerExceptions[0]);
            Assert.Equal(new int[] { 2, 1 }, steps.ToArray());
        }

        [Fact]
        public void Should_execute_all_cleanup_steps_despite_async_exceptions_in_run_and_cleanup_and_complete_with_all_exceptions()
        {
            CleanupGuard guard = new CleanupGuard();
            List<int> steps = new List<int>();
            Func<int, Task, Task> doStepAsync = delegate(int i, Task t)
            {
                steps.Add(i);
                return t;
            };

            TaskCompletionSource<bool> cleanupTcs = new TaskCompletionSource<bool>();
            guard.Register(() => doStepAsync(1, Task.FromResult(false)));
            guard.Register(() => doStepAsync(2, cleanupTcs.Task));

            TaskCompletionSource<bool> runTcs = new TaskCompletionSource<bool>();
            Task task = guard.RunAsync(g => runTcs.Task);

            Assert.False(task.IsCompleted);

            InvalidProgramException expectedRunException = new InvalidProgramException("Expected (run).");
            runTcs.SetException(expectedRunException);

            InvalidTimeZoneException expectedCleanupException = new InvalidTimeZoneException("Expected (cleanup).");
            cleanupTcs.SetException(expectedCleanupException);

            Assert.Equal(TaskStatus.Faulted, task.Status);
            Assert.NotNull(task.Exception);
            AggregateException ae = Assert.IsType<AggregateException>(task.Exception).Flatten();
            Assert.Equal(2, ae.InnerExceptions.Count);
            Assert.Same(expectedRunException, ae.InnerExceptions[0]);
            Assert.Same(expectedCleanupException, ae.InnerExceptions[1]);
            Assert.Equal(new int[] { 2, 1 }, steps.ToArray());
        }
    }
}