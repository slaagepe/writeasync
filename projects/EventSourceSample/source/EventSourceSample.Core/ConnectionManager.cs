﻿//-----------------------------------------------------------------------
// <copyright file="ConnectionManager.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EventSourceSample
{
    using System;
    using System.Threading.Tasks;

    public class ConnectionManager<TProxy>
    {
        private readonly IFactory<IConnection<TProxy>> factory;

        private IConnection<TProxy> connection;

        public ConnectionManager(IFactory<IConnection<TProxy>> factory)
        {
            this.factory = factory;
        }

        public async Task ConnectAsync()
        {
            if (this.connection == null)
            {
                this.connection = this.factory.Create();
                await this.connection.OpenAsync();
            }
        }
    }
}
