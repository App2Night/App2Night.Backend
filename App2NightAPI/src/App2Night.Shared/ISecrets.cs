using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App2Night.Shared
{
    public interface ISecrets
    {
        string APISecret { get; }
        string ClientSecret { get; }
    }
}
