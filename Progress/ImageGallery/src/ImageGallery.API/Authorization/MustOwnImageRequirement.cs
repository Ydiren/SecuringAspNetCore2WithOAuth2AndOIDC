using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.API.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    public class MustOwnImageRequirement : IAuthorizationRequirement
    {
        public MustOwnImageRequirement()
        {
        }
    }
}
