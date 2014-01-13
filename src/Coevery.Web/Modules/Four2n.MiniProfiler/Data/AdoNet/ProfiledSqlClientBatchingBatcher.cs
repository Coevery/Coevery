// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfiledSqlClientBatchingBatcher.cs" company="Daniel Dabrowski - rod.42n.pl">
//   Copyright (c) 2008 Daniel Dabrowski - 42n. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfiledSqlClientBatchingBatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace Four2n.Orchard.MiniProfiler.Data.AdoNet
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Text;
    using NHibernate;
    using NHibernate.AdoNet;
    using NHibernate.AdoNet.Util;
    using NHibernate.Exceptions;
    using NHibernate.Util;
    using StackExchange.Profiling.Data;

    public class ProfiledSqlClientBatchingBatcher : AbstractBatcher
    {
        private readonly int defaultTimeout;
        private readonly IDbProfiler profiler;
        private int batchSize;
        private int totalExpectedRowsAffected;
        private SqlClientSqlCommandSet currentBatch;
        private StringBuilder currentBatchCommandsLog;

        public ProfiledSqlClientBatchingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
            : base(connectionManager, interceptor)
        {
            this.batchSize = Factory.Settings.AdoBatchSize;
            this.defaultTimeout = PropertiesHelper.GetInt32(global::NHibernate.Cfg.Environment.CommandTimeout, global::NHibernate.Cfg.Environment.Properties, -1);

            this.currentBatch = this.CreateConfiguredBatch();

            // we always create this, because we need to deal with a scenario in which
            // the user change the logging configuration at runtime. Trying to put this
            // behind an if(log.IsDebugEnabled) will cause a null reference exception 
            // at that point.
            this.currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
            this.profiler = StackExchange.Profiling.MiniProfiler.Current as IDbProfiler;
        }

        public override int BatchSize
        {
            get { return this.batchSize; }
            set { this.batchSize = value; }
        }

        protected override int CountOfStatementsInCurrentBatch
        {
            get { return this.currentBatch.CountOfCommands; }
        }

        public override void AddToBatch(IExpectation expectation)
        {
            this.totalExpectedRowsAffected += expectation.ExpectedRowCount;
            IDbCommand batchUpdate = CurrentCommand;
            Driver.AdjustCommand(batchUpdate);
            string lineWithParameters = null;
            var sqlStatementLogger = Factory.Settings.SqlStatementLogger;
            if (sqlStatementLogger.IsDebugEnabled || Log.IsDebugEnabled)
            {
                lineWithParameters = sqlStatementLogger.GetCommandLineWithParameters(batchUpdate);
                var formatStyle = sqlStatementLogger.DetermineActualStyle(FormatStyle.Basic);
                lineWithParameters = formatStyle.Formatter.Format(lineWithParameters);
                this.currentBatchCommandsLog.Append("command ")
                    .Append(this.currentBatch.CountOfCommands)
                    .Append(":")
                    .AppendLine(lineWithParameters);
            }
            if (Log.IsDebugEnabled)
            {
                Log.Debug("Adding to batch:" + lineWithParameters);
            }

            if (batchUpdate is ProfiledSqlCommand)
            {
                var sqlCommand = ((ProfiledSqlCommand)batchUpdate).SqlCommand;
                this.currentBatch.Append(sqlCommand);
                if (this.profiler != null)
                {
                    this.profiler.ExecuteStart(sqlCommand, ExecuteType.NonQuery);
                }
            }
            else
            {
                this.currentBatch.Append((System.Data.SqlClient.SqlCommand)batchUpdate);
            }

            if (this.currentBatch.CountOfCommands >= this.batchSize)
            {
                this.ExecuteBatchWithTiming(batchUpdate);
            }
        }

        protected override void DoExecuteBatch(IDbCommand ps)
        {
            Log.DebugFormat("Executing batch");
            this.CheckReaders();
            this.Prepare(this.currentBatch.BatchCommand);
            if (Factory.Settings.SqlStatementLogger.IsDebugEnabled)
            {
                Factory.Settings.SqlStatementLogger.LogBatchCommand(this.currentBatchCommandsLog.ToString());
                this.currentBatchCommandsLog = new StringBuilder().AppendLine("Batch commands:");
            }

            if (this.profiler != null)
            {
                this.profiler.ExecuteStart(this.currentBatch.BatchCommand, ExecuteType.NonQuery);
            }

            int rowsAffected;
            try
            {
                rowsAffected = this.currentBatch.ExecuteNonQuery();
            }
            catch (DbException e)
            {
                throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, e, "could not execute batch command.");
            }

            if (this.profiler != null)
            {
                this.profiler.ExecuteFinish(this.currentBatch.BatchCommand, ExecuteType.NonQuery, null);
            }

            Expectations.VerifyOutcomeBatched(this.totalExpectedRowsAffected, rowsAffected);

            this.currentBatch.Dispose();
            this.totalExpectedRowsAffected = 0;
            this.currentBatch = this.CreateConfiguredBatch();
        }

        private SqlClientSqlCommandSet CreateConfiguredBatch()
        {
            var result = new SqlClientSqlCommandSet();
            if (this.defaultTimeout > 0)
            {
                try
                {
                    result.CommandTimeout = this.defaultTimeout;
                }
                catch (Exception e)
                {
                    if (Log.IsWarnEnabled)
                    {
                        Log.Warn(e.ToString());
                    }
                }
            }

            return result;
        }
    }
}