﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsSiteController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SiteLogPageResult>> Get([FromBody] SearchRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsSite))
            {
                return Unauthorized();
            }

            var siteOptions = await _siteRepository.GetSiteOptionsAsync(0);

            var adminId = 0;
            if (!string.IsNullOrEmpty(request.UserName))
            {
                var admin = await _administratorRepository.GetByUserNameAsync(request.UserName);
                if (admin == null)
                {
                    return new SiteLogPageResult
                    {
                        Items = new List<SiteLogResult>(),
                        Count = 0,
                        SiteOptions = siteOptions
                    };
                }
                adminId = admin.Id;
            }

            var siteIdList = await _siteRepository.GetSiteIdsAsync();
            var siteIds = request.SiteIds;
            if (siteIds == null || siteIds.Count == 0)
            {
                siteIds = siteIdList;
            }

            var count = await _siteLogRepository.GetCountAsync(siteIds, request.LogType, adminId, request.Keyword, request.DateFrom, request.DateTo);
            var siteLogs = await _siteLogRepository.GetAllAsync(siteIds, request.LogType, adminId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);

            var logTasks = siteLogs.Select(async x =>
            {
                var site = await _siteRepository.GetAsync(x.SiteId);
                var administrator = await _administratorRepository.GetByUserIdAsync(x.AdminId);
                var adminName = administrator == null ? string.Empty : administrator.UserName;
                var log = new SiteLogResult
                {
                    Id = x.Id,
                    SiteId = x.SiteId,
                    ChannelId = x.ChannelId,
                    ContentId = x.ContentId,
                    AdminId = x.AdminId,
                    IpAddress = x.IpAddress,
                    Action = x.Action,
                    Summary = x.Summary,
                    SiteName = site.SiteName,
                    CreatedDate = x.CreatedDate,
                    AdminName = adminName,
                    WebUrl = await _pathManager.GetWebUrlAsync(site)
                };
                return log;
            });
            var logs = await Task.WhenAll(logTasks);

            return new SiteLogPageResult
            {
                Items = logs,
                Count = count,
                SiteOptions = siteOptions
            };
        }
    }
}
