﻿using Microsoft.AspNetCore.Authorization;
                var searchApprovedBy = Request.Form["columns[3][search][value]"].FirstOrDefault();
                CompData = !string.IsNullOrEmpty(searchCreatedBy) ? CompData.Where(m => m.User.UserName.ToString().ToUpper().Contains(searchCreatedBy.ToUpper())) : CompData;
                CompData = !string.IsNullOrEmpty(searchApprovedBy) ? CompData.Where(m => m.ApprovedBy != null ? _dbContext.Users.FirstOrDefault(x => x.Id == m.ApprovedBy).UserName.ToUpper().Contains(searchApprovedBy.ToUpper()) : false) : CompData;
                var data = CompData.ToList();
                if (pageSize == -1)
                {
                    data = CompData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = CompData.Skip(skip).Take(pageSize).ToList();
                }