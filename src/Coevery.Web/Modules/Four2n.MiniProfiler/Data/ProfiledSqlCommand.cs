// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfiledSqlCommand.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfiledSqlCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Four2n.Orchard.MiniProfiler.Data
{
    using System.Data.Common;
    using System.Data.SqlClient;
    using StackExchange.Profiling.Data;

    public class ProfiledSqlCommand : ProfiledDbCommand
    {
        private SqlCommand sqlCommand;

        public ProfiledSqlCommand(DbCommand command, IDbProfiler profiler)
            : base(command, null, profiler)
        {
            this.sqlCommand = (SqlCommand)command;
        }

        public SqlCommand SqlCommand
        {
            get { return this.sqlCommand; }
        }
    }
}