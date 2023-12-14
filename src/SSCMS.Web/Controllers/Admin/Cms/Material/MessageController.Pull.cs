﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class MessageController
    {
        [HttpPost, Route(RoutePull)]
        public async Task<ActionResult<PullResult>> Pull([FromBody] PullRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialMessage))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);

            if (success)
            {
                await _wxManager.PullMaterialAsync(token, MaterialType.Message, request.GroupId);
            }

            return new PullResult
            {
                Success = success,
                ErrorMessage = errorMessage
            };
        }

        
    }
}
