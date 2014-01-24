﻿//-----------------------------------------------------------------------
// <copyright file="EventWindowTest.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace TraceAnalysisSample.Test.Unit
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class EventWindowTest
    {
        public EventWindowTest()
        {
        }

        [Fact]
        public void Add_adds_operation_to_pending()
        {
            EventWindow window = new EventWindow();
            int eventId = 1;
            Guid instanceId = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

            window.Add(eventId, instanceId);

            Assert.Equal(1, window.GetPendingCount(eventId));
        }

        [Fact]
        public void Add_same_type_twices_adds_2_operations_to_pending()
        {
            EventWindow window = new EventWindow();
            int eventId = 1;
            Guid instanceId1 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            Guid instanceId2 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2);

            window.Add(eventId, instanceId1);
            window.Add(eventId, instanceId2);

            Assert.Equal(2, window.GetPendingCount(eventId));
        }

        [Fact]
        public void Add_different_types_adds_2_separate_operations_to_pending()
        {
            EventWindow window = new EventWindow();
            int eventId1 = 1;
            Guid instanceId1 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            int eventId2 = 2;
            Guid instanceId2 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2);

            window.Add(eventId1, instanceId1);
            window.Add(eventId2, instanceId2);

            Assert.Equal(1, window.GetPendingCount(eventId1));
            Assert.Equal(1, window.GetPendingCount(eventId2));
        }

        [Fact]
        public void Add_many_types_adds_separate_operations_to_pending()
        {
            EventWindow window = new EventWindow();
            int eventIdA = 1;
            Guid instanceIdA1 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            int eventIdB = 2;
            Guid instanceIdB1 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2);
            Guid instanceIdB2 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3);
            int eventIdC = 3;
            Guid instanceIdC1 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4);
            Guid instanceIdC2 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5);
            Guid instanceIdC3 = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6);

            window.Add(eventIdA, instanceIdA1);
            window.Add(eventIdB, instanceIdB1);
            window.Add(eventIdB, instanceIdB2);
            window.Add(eventIdC, instanceIdC1);
            window.Add(eventIdC, instanceIdC2);
            window.Add(eventIdC, instanceIdC3);

            Assert.Equal(1, window.GetPendingCount(eventIdA));
            Assert.Equal(2, window.GetPendingCount(eventIdB));
            Assert.Equal(3, window.GetPendingCount(eventIdC));
        }

        [Fact]
        public void Add_same_type_same_ID_throws_InvalidOperation()
        {
            EventWindow window = new EventWindow();
            int eventId = 1;
            Guid instanceId = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

            window.Add(eventId, instanceId);

            InvalidOperationException ioe = Assert.Throws<InvalidOperationException>(() => window.Add(eventId, instanceId));
            Assert.Contains("00000000-0000-0000-0000-000000000001", ioe.Message, StringComparison.Ordinal);

            Assert.Equal(1, window.GetPendingCount(eventId));
        }

        [Fact]
        public void Get_pending_count_missing_type_returns_0()
        {
            EventWindow window = new EventWindow();

            Assert.Equal(0, window.GetPendingCount(1));
        }

        [Fact]
        public void Complete_no_existing_event_does_nothing()
        {
            EventWindow window = new EventWindow();
            int eventId = 1;
            Guid instanceId = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

            window.Complete(eventId, instanceId);

            Assert.Equal(0, window.GetCompletedCount(eventId));
        }

        [Fact]
        public void Complete_pending_event_with_same_type_and_ID_moves_to_completed()
        {
            EventWindow window = new EventWindow();
            int eventId = 1;
            Guid instanceId = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

            window.Add(eventId, instanceId);
            window.Complete(eventId, instanceId);

            Assert.Equal(0, window.GetPendingCount(eventId));
            Assert.Equal(1, window.GetCompletedCount(eventId));
        }
    }
}