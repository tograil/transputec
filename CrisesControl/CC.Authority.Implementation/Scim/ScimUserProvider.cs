using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Web.Http;
using CC.Authority.Implementation.Data;
using CC.Authority.Implementation.Helpers;
using CC.Authority.SCIM;
using CC.Authority.SCIM.Protocol;
using CC.Authority.SCIM.Schemas;
using CC.Authority.SCIM.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.SCIM.WebHostSample.Provider;
using User = CC.Authority.Implementation.Models.User;

namespace CC.Authority.Implementation.Scim
{
    public class ScimUserProvider : ProviderBase
    {
        private readonly CrisesControlAuthContext _authContext;
        private readonly ICurrentUser _currentUser;

        public ScimUserProvider(CrisesControlAuthContext authContext, ICurrentUser currentUser)
        {
            _authContext = authContext;
            _currentUser = currentUser;
        }

        public override async Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (null == parameters.AlternateFilters)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
            }

            if (string.IsNullOrWhiteSpace(parameters.SchemaIdentifier))
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
            }

            var predicate = PredicateBuilder.False<Core2EnterpriseUser>();
            Expression<Func<Core2EnterpriseUser, bool>> predicateAnd;

            var results = await _authContext.Users.Select(user => new Core2EnterpriseUser
            {
                Identifier = user.UserId.ToString(),
                ExternalIdentifier = user.ExternalScimId,
                Active = user.Status == 1,
                DisplayName = $"{user.FirstName} {user.LastName}",
                Name = new Name
                {
                    FamilyName = user.LastName,
                    GivenName = user.FirstName,
                    Formatted = $"{user.FirstName} {user.LastName}"
                },
                Roles = new[]
                { new Role
                    {
                        Display = user.UserRole!
                    }
                },
                Metadata = new Core2Metadata
                {
                    ResourceType = "User",
                    Created = user.CreatedOn.DateTime,
                    LastModified = user.UpdatedOn.DateTime
                }
            }).ToArrayAsync();

            if (parameters.AlternateFilters.Count <= 0)
            {
                results = results.ToArray();
            }
            else
            {

                foreach (IFilter queryFilter in parameters.AlternateFilters)
                {
                    predicateAnd = PredicateBuilder.True<Core2EnterpriseUser>();

                    IFilter andFilter = queryFilter;
                    IFilter currentFilter = andFilter;
                    do
                    {
                        if (string.IsNullOrWhiteSpace(andFilter.AttributePath))
                        {
                            throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
                        }

                        else if (string.IsNullOrWhiteSpace(andFilter.ComparisonValue))
                        {
                            throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
                        }

                        // UserName filter
                        else if (andFilter.AttributePath.Equals(AttributeNames.UserName, StringComparison.OrdinalIgnoreCase))
                        {
                            if (andFilter.FilterOperator != ComparisonOperator.Equals)
                            {
                                throw new NotSupportedException(
                                    string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator));
                            }

                            string userName = andFilter.ComparisonValue;
                            predicateAnd = predicateAnd.And(p => string.Equals(p.UserName, userName, StringComparison.OrdinalIgnoreCase));


                        }

                        // ExternalId filter
                        else if (andFilter.AttributePath.Equals(AttributeNames.ExternalIdentifier, StringComparison.OrdinalIgnoreCase))
                        {
                            if (andFilter.FilterOperator != ComparisonOperator.Equals)
                            {
                                throw new NotSupportedException(
                                    string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator));
                            }

                            string externalIdentifier = andFilter.ComparisonValue;
                            predicateAnd = predicateAnd.And(p => string.Equals(p.ExternalIdentifier, externalIdentifier, StringComparison.OrdinalIgnoreCase));


                        }

                        //Active Filter
                        else if (andFilter.AttributePath.Equals(AttributeNames.Active, StringComparison.OrdinalIgnoreCase))
                        {
                            if (andFilter.FilterOperator != ComparisonOperator.Equals)
                            {
                                throw new NotSupportedException(
                                    string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator));
                            }

                            bool active = bool.Parse(andFilter.ComparisonValue);
                            predicateAnd = predicateAnd.And(p => p.Active == active);

                        }

                        //LastModified filter
                        else if (andFilter.AttributePath.Equals($"{AttributeNames.Metadata}.{AttributeNames.LastModified}", StringComparison.OrdinalIgnoreCase))
                        {
                            if (andFilter.FilterOperator == ComparisonOperator.EqualOrGreaterThan)
                            {
                                DateTime comparisonValue = DateTime.Parse(andFilter.ComparisonValue).ToUniversalTime();
                                predicateAnd = predicateAnd.And(p => p.Metadata.LastModified >= comparisonValue);


                            }
                            else if (andFilter.FilterOperator == ComparisonOperator.EqualOrLessThan)
                            {
                                DateTime comparisonValue = DateTime.Parse(andFilter.ComparisonValue).ToUniversalTime();
                                predicateAnd = predicateAnd.And(p => p.Metadata.LastModified <= comparisonValue);


                            }
                            else
                                throw new NotSupportedException(
                                    string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator));



                        }
                        else
                            throw new NotSupportedException(
                                string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterAttributePathNotSupportedTemplate, andFilter.AttributePath));

                        currentFilter = andFilter;
                        andFilter = andFilter.AdditionalFilter;

                    } while (currentFilter.AdditionalFilter != null);

                    predicate = predicate.Or(predicateAnd);

                }

                results = results.Where(predicate.Compile()).ToArray();
            }

            if (parameters.PaginationParameters != null)
            {
                int count = parameters.PaginationParameters.Count.HasValue ? parameters.PaginationParameters.Count.Value : 0;
                return results.Take(count).ToArray();
            }
            else
                return results.ToArray();
        }

        public override async Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier != null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }


            if (resource is not Core2EnterpriseUser user)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var currentTimeZone = await _authContext.StdTimeZones
                .FirstOrDefaultAsync(x => x.PortalTimeZone == user.TimeZone);

            var primaryEmail = user.ElectronicMailAddresses?.FirstOrDefault(x => x.Primary);
            var secondaryEmail = user.ElectronicMailAddresses?.FirstOrDefault(x => !x.Primary);

            user.Locale ??= "en-US";

            var existingUser = await _authContext.Users.FirstOrDefaultAsync(x =>
                string.Equals(x.PrimaryEmail, primaryEmail.Value));

            if (existingUser != null)
            {
                existingUser.ExternalScimId = user.ExternalIdentifier;
                existingUser.UpdatedOn = DateTimeOffset.UtcNow;
                existingUser.UpdatedBy = _currentUser.UserId;

                await _authContext.SaveChangesAsync();

                user.Identifier = existingUser.UserId.ToString();

                user.Metadata = new Core2Metadata
                {
                    ResourceType = "User",
                    Created = existingUser.CreatedOn.DateTime,
                    LastModified = existingUser.UpdatedOn.DateTime
                };

                return user;
            }

            var newUser = new Models.User
            {
                ExternalScimId = user.ExternalIdentifier,
                FirstName = user.Name.GivenName,
                LastName = user.Name.FamilyName,
                Status = user.Active ? 1 : 0,
                UserLanguage = new CultureInfo(user.Locale).TwoLetterISOLanguageName,
                TimezoneId = currentTimeZone?.TimeZoneId,
                PrimaryEmail = primaryEmail?.Value ?? string.Empty,
                SecondaryEmail = secondaryEmail?.Value ?? string.Empty,
                CreatedBy = _currentUser.UserId,
                CreatedOn = DateTimeOffset.UtcNow,
                UpdatedBy = _currentUser.UserId,
                UpdatedOn = DateTimeOffset.UtcNow,
                CompanyId = _currentUser.CompanyId,
                Password = string.Empty,
                ExpirePassword = false,
                Otpexpiry = DateTimeOffset.MaxValue,
                Smstrigger = false,
                ActiveOffDuty = 0,
                LastLocationUpdate = DateTimeOffset.UtcNow,
                TrackingStartTime = DateTimeOffset.MinValue,
                TrackingEndTime = DateTimeOffset.MaxValue,
                PasswordChangeDate = DateTimeOffset.UtcNow,
                UniqueGuiId = Guid.NewGuid().ToString()
            };

            await _authContext.Users.AddAsync(newUser);

            await _authContext.SaveChangesAsync();

            user.Identifier = newUser.UserId.ToString();

            user.Metadata = new Core2Metadata
            {
                ResourceType = "User",
                Created = newUser.CreatedOn.DateTime,
                LastModified = newUser.UpdatedOn.DateTime
            };

            return user;
        }

        public override async Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (string.IsNullOrWhiteSpace(resourceIdentifier.Identifier))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var identifier = resourceIdentifier.Identifier;

            var user = await _authContext.Users.FirstOrDefaultAsync(x => x.UserId.ToString() == identifier);

            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _authContext.Remove(user);

            await _authContext.SaveChangesAsync();
        }

        public override async Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (string.IsNullOrEmpty(parameters.ResourceIdentifier?.Identifier))
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var identifier = parameters.ResourceIdentifier.Identifier;

            var user = await _authContext.Users.FirstOrDefaultAsync(x => x.UserId.ToString() == identifier);

            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return ToCore2EnterpriseUser(user);
        }

        public override async Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (resource is not Core2EnterpriseUser user)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var userToUpdate =
                await _authContext.Users.FirstOrDefaultAsync(x => x.UserId.ToString() == resource.Identifier);

            if (userToUpdate is null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            await UpdateUser(user, userToUpdate);

            user = ToCore2EnterpriseUser(userToUpdate);

            return user;
        }

        private async Task UpdateUser(Core2EnterpriseUser user, User userToUpdate)
        {
            var primaryEmail = user.ElectronicMailAddresses?.FirstOrDefault(x => x.Primary);
            var secondaryEmail = user.ElectronicMailAddresses?.FirstOrDefault(x => !x.Primary);

            if (user.Name is not null)
            {
                userToUpdate.FirstName = user.Name.GivenName;
                userToUpdate.LastName = user.Name.FamilyName;
            }

            if (user.ExternalIdentifier is not null)
            {
                userToUpdate.ExternalScimId = user.ExternalIdentifier;
            }

            if (primaryEmail != null)
            {
                userToUpdate.PrimaryEmail = primaryEmail?.Value ?? string.Empty;
            }

            if (secondaryEmail != null)
            {
                userToUpdate.SecondaryEmail = secondaryEmail?.Value ?? string.Empty;
            }

            if (user.TimeZone != null)
            {
                var currentTimeZone = await _authContext.StdTimeZones
                    .FirstOrDefaultAsync(x => x.PortalTimeZone == user.TimeZone);

                userToUpdate.TimezoneId = currentTimeZone?.TimeZoneId;
            }

            userToUpdate.UpdatedOn = DateTime.UtcNow;

            await _authContext.SaveChangesAsync();
        }

        public override async Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            if (null == patch)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            if (null == patch.ResourceIdentifier)
            {
                throw new ArgumentException(string.Format(SystemForCrossDomainIdentityManagementServiceResources
                    .ExceptionInvalidOperation));
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.Identifier))
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources
                    .ExceptionInvalidOperation);
            }

            if (null == patch.PatchRequest)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources
                    .ExceptionInvalidOperation);
            }

            if (patch.PatchRequest is not PatchRequest2 patchRequest)
            {
                var unsupportedPatchTypeName = patch.GetType().FullName;
                throw new NotSupportedException(unsupportedPatchTypeName);
            }

            var userToUpdate =
                await _authContext.Users.FirstOrDefaultAsync(x =>
                    x.UserId.ToString() == patch.ResourceIdentifier.Identifier);

            if (userToUpdate is null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var user = ToCore2EnterpriseUser(userToUpdate);

            user.Apply(patchRequest);

            await UpdateUser(user, userToUpdate);
        }

        private static Core2EnterpriseUser ToCore2EnterpriseUser(User userToUpdate)
        {
            return new Core2EnterpriseUser
            {
                Identifier = userToUpdate.UserId.ToString(),
                ExternalIdentifier = userToUpdate.ExternalScimId,
                Active = userToUpdate.Status == 1,
                DisplayName = $"{userToUpdate.FirstName} {userToUpdate.LastName}",
                Name = new Name
                {
                    FamilyName = userToUpdate.LastName,
                    GivenName = userToUpdate.FirstName,
                    Formatted = $"{userToUpdate.FirstName} {userToUpdate.LastName}"
                },
                Roles = new[]
                {
                    new Role
                    {
                        Display = userToUpdate.UserRole!
                    }
                },
                Metadata = new Core2Metadata
                {
                    ResourceType = "User",
                    Created = userToUpdate.CreatedOn.DateTime,
                    LastModified = userToUpdate.UpdatedOn.DateTime
                }
            };
        }
    }
}