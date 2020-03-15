using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Common;
using Data;
using Domain;
using Hangfire;

namespace Services.BackgroundJobs
{
    public interface IBackgroundJobClient
    {
        int Enqueue<TJob>(Expression<Action<TJob>> execute);
    }
    
    public class BackgroundJobClient : IBackgroundJobClient
    {
        public int Enqueue<TJob>(Expression<Action<TJob>> execute)
        {
            return BackgroundJob.Enqueue(execute).AsInt();
        }
    }

    public interface IBackgroundJob
    {
        void Execute();
    }

    public interface IBackgroundJob<in TData>
    {
        void Execute(TData data);
    }
}