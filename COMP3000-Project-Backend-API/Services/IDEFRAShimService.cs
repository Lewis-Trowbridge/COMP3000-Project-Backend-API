// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using COMP3000_Project_Backend_API.Models.External.Shim;
using COMP3000_Project_Backend_API.Models.MongoDB;

namespace COMP3000_Project_Backend_API.Services
{
    public interface IDEFRAShimService
    {
        Task<ShimResponse?> GetDataFromShim(DEFRAMetadata metadata, DateTime? timestamp);
    }
}