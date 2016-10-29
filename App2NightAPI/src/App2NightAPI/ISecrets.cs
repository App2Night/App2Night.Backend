using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI
{
    public interface ISecrets
    {
        string APISecret { get; }
        string ClientSecret { get; }
    }
}
