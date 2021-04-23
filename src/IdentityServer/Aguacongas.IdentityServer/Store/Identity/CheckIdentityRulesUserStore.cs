﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class CheckIdentityRulesUserStore : IAdminStore<User>
    {
        private readonly IAdminStore<User> _parent;
        private readonly UserManager<ApplicationUser> _manager;

        public CheckIdentityRulesUserStore(IAdminStore<User> parent, UserManager<ApplicationUser> manager)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }
        public async Task<User> CreateAsync(User entity, CancellationToken cancellationToken = default)
        => CheckResult(entity, await _manager.CreateAsync(CreateUser(entity), entity.Password).ConfigureAwait(false));
                
        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        => await CreateAsync(entity as User, cancellationToken).ConfigureAwait(false);
        
        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _manager.FindByIdAsync(id).ConfigureAwait(false);
            if (user == null)
            {
                return;
            }
            CheckResult(null, await _manager.DeleteAsync(user).ConfigureAwait(false));
        }

        public Task<User> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        => _parent.GetAsync(id, request, cancellationToken);

        public Task<PageResponse<User>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        => _parent.GetAsync(request, cancellationToken);

        public async Task<User> UpdateAsync(User entity, CancellationToken cancellationToken = default)
        => CheckResult(entity, await _manager.UpdateAsync(CreateUser(entity)).ConfigureAwait(false));

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        => await UpdateAsync(entity as User, cancellationToken);

        private static User CheckResult(User entity, IdentityResult result)
        {
            if (result.Succeeded)
            {
                return entity;
            }

            throw new IdentityException
            {
                Errors = result.Errors
            };
        }

        private static ApplicationUser CreateUser(User entity)
        => new()
        {
                Id = entity.Id,
                AccessFailedCount = entity.AccessFailedCount,
                ConcurrencyStamp = entity.ConcurrencyStamp,
                Email = entity.Email,
                EmailConfirmed = entity.EmailConfirmed,
                NormalizedEmail = entity.NormalizedEmail,
                NormalizedUserName = entity.NormalizedUserName,
                LockoutEnabled = entity.LockoutEnabled,
                LockoutEnd = entity.LockoutEnd,
                PhoneNumber = entity.PhoneNumber,
                PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
                PasswordHash = entity.PasswordHash,
                SecurityStamp = entity.SecurityStamp,
                TwoFactorEnabled = entity.TwoFactorEnabled,
                UserName = entity.UserName
            };        
    }
}
