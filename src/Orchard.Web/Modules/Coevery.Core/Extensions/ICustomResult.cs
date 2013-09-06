using System;

namespace Coevery.Core
{
    public interface ICustomResult
    {
        bool Success { get; set; }
        string Message { get; set; }
        void Succeed();
        void Fail(string message);
        void Try(Action action);
    }
}
