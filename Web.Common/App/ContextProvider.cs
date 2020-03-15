using System.Collections.Generic;

namespace Web.Common.App
{
    public interface IContextProvider
    {
        ContextType Context { get; }
        IReadOnlyCollection<string> IssuerNames { get; }
    }

    public enum ContextType
    {
        Admin,
        Public
    }
}