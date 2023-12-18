﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class MenusController
    {
        [HttpPost, Route(RoutePull)]
        public async Task<ActionResult<WxMenusResult>> Pull([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxMenus))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);

            if (!success)
            {
                return this.Error(errorMessage);
            }

            await _wxManager.PullMenuAsync(token, request.SiteId);

            var wxMenus = await _wxMenuRepository.GetMenusAsync(request.SiteId);

            return new WxMenusResult
            {
                WxMenus = wxMenus
            };
        }
    }
}
