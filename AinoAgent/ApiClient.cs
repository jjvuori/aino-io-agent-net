using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aino.Agents.Core
{
    /// <summary>
    /// A delegate for sending data to Aino.io API
    /// </summary>
    public interface IApiClient
    {
        IApiResponse Send(byte[] data);
    }
}
