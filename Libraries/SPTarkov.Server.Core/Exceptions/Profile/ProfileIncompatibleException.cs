using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPTarkov.Server.Core.Exceptions.Profile;

public class ProfileIncompatibleException : Exception
{
    public ProfileIncompatibleException(string message)
        : base(message) { }

    public ProfileIncompatibleException(string message, Exception innerException)
        : base(message, innerException) { }

    public override string? StackTrace
    {
        get { return null; }
    }
}
