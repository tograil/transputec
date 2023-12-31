﻿using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Web.Http;
using AutoMapper;
using CC.Authority.Core.UserManagement;
using CC.Authority.Core.UserManagement.Models;
using CC.Authority.Implementation.Data;
using CC.Authority.Implementation.Helpers;
using CC.Authority.SCIM;
using CC.Authority.SCIM.Protocol;
using CC.Authority.SCIM.Schemas;
using CC.Authority.SCIM.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.SCIM.WebHostSample.Provider;

namespace CC.Authority.Implementation.Scim {
    public class ScimUserProvider : ProviderBase {
        private readonly CrisesControlAuthContext _authContext;
        private readonly ICurrentUser _currentUser;
        private readonly IUserManager _userManager;
        private readonly IMapper _mapper;

        public ScimUserProvider(CrisesControlAuthContext authContext, ICurrentUser currentUser, IUserManager userManager, IMapper mapper) {
            _authContext = authContext;
            _currentUser = currentUser;
            _userManager = userManager;
            _mapper = mapper;
        }

        public override async Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier) {
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier)) {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (null == parameters.AlternateFilters) {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
            }

            if (string.IsNullOrWhiteSpace(parameters.SchemaIdentifier)) {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
            }

            var predicate = PredicateBuilder.False<Core2EnterpriseUser>();
            Expression<Func<Core2EnterpriseUser, bool>> predicateAnd;

            var results = await _authContext.Users.Where(w=>w.Status < 3 && w.CompanyId == _currentUser.CompanyId).Select(user => new Core2EnterpriseUser {
                Identifier = user.UserId.ToString(),
                ExternalIdentifier = user.ExternalScimId,
                Active = user.Status == 1,
                DisplayName = $"{user.FirstName} {user.LastName}",
                UserName = user.PrimaryEmail,
                Nickname = user.FirstName,
                ElectronicMailAddresses = new List<ElectronicMailAddress>() {
                    new ElectronicMailAddress { ItemType=ElectronicMailAddress.Work,Primary=true,Value=user.PrimaryEmail},
                    new ElectronicMailAddress { ItemType=ElectronicMailAddress.Home,Primary=false,Value=user.SecondaryEmail}
                },
                Name = new Name {
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
                Metadata = new Core2Metadata {
                    ResourceType = "User",
                    Created = user.CreatedOn.DateTime,
                    LastModified = user.UpdatedOn.DateTime
                }
            }).ToArrayAsync();

            if (parameters.AlternateFilters.Count <= 0) {
                results = results.ToArray();
            } else {

                foreach (IFilter queryFilter in parameters.AlternateFilters) {
                    predicateAnd = PredicateBuilder.True<Core2EnterpriseUser>();

                    IFilter andFilter = queryFilter;
                    IFilter currentFilter = andFilter;
                    do {
                        if (string.IsNullOrWhiteSpace(andFilter.AttributePath)) {
                            throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
                        } else if (string.IsNullOrWhiteSpace(andFilter.ComparisonValue)) {
                            throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
                        }

                          // UserName filter
                          else if (andFilter.AttributePath.Equals(AttributeNames.UserName, StringComparison.OrdinalIgnoreCase)) {
                            if (andFilter.FilterOperator != ComparisonOperator.Equals) {
                                throw new NotSupportedException(
                                    string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator));
                            }

                            string userName = andFilter.ComparisonValue;
                            predicateAnd = predicateAnd.And(p => string.Equals(p.UserName, userName, StringComparison.OrdinalIgnoreCase));


                        }

                          // ExternalId filter
                          else if (andFilter.AttributePath.Equals(AttributeNames.ExternalIdentifier, StringComparison.OrdinalIgnoreCase)) {
                            if (andFilter.FilterOperator != ComparisonOperator.Equals) {
                                throw new NotSupportedException(
                                    string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator));
                            }

                            string externalIdentifier = andFilter.ComparisonValue;
                            predicateAnd = predicateAnd.And(p => string.Equals(p.ExternalIdentifier, externalIdentifier, StringComparison.OrdinalIgnoreCase));


                        }

                          //Active Filter
                          else if (andFilter.AttributePath.Equals(AttributeNames.Active, StringComparison.OrdinalIgnoreCase)) {
                            if (andFilter.FilterOperator != ComparisonOperator.Equals) {
                                throw new NotSupportedException(
                                    string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator));
                            }

                            bool active = bool.Parse(andFilter.ComparisonValue);
                            predicateAnd = predicateAnd.And(p => p.Active == active);

                        }

                          //LastModified filter
                          else if (andFilter.AttributePath.Equals($"{AttributeNames.Metadata}.{AttributeNames.LastModified}", StringComparison.OrdinalIgnoreCase)) {
                            if (andFilter.FilterOperator == ComparisonOperator.EqualOrGreaterThan) {
                                DateTime comparisonValue = DateTime.Parse(andFilter.ComparisonValue).ToUniversalTime();
                                predicateAnd = predicateAnd.And(p => p.Metadata.LastModified >= comparisonValue);


                            } else if (andFilter.FilterOperator == ComparisonOperator.EqualOrLessThan) {
                                DateTime comparisonValue = DateTime.Parse(andFilter.ComparisonValue).ToUniversalTime();
                                predicateAnd = predicateAnd.And(p => p.Metadata.LastModified <= comparisonValue);


                            } else
                                throw new NotSupportedException(
                                    string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterOperatorNotSupportedTemplate, andFilter.FilterOperator));



                        } else
                            throw new NotSupportedException(
                                string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterAttributePathNotSupportedTemplate, andFilter.AttributePath));

                        currentFilter = andFilter;
                        andFilter = andFilter.AdditionalFilter;

                    } while (currentFilter.AdditionalFilter != null);

                    predicate = predicate.Or(predicateAnd);

                }

                results = results.Where(predicate.Compile()).ToArray();
            }

            if (parameters.PaginationParameters != null) {
                int count = parameters.PaginationParameters.Count.HasValue ? parameters.PaginationParameters.Count.Value : 0;
                return results.Take(count).ToArray();
            } else
                return results.ToArray();
        }

        public override async Task<Resource> CreateAsync(Resource resource, string correlationIdentifier) {
            if (resource.Identifier != null) {
                //throw new HttpResponseException(HttpStatusCode.BadRequest);
                throw new HttpResponseException(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Resource identifier is not null")
                });
            }


            if (resource is not Core2EnterpriseUser user) {
                throw new HttpResponseException(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("Not a valid user object")
                });
                //throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var currentTimeZone = await _authContext.StdTimeZones
                .FirstOrDefaultAsync(x => x.PortalTimeZone == user.TimeZone);

            var primaryEmail = user.ElectronicMailAddresses?.FirstOrDefault(x => x.Primary);
            var secondaryEmail = user.ElectronicMailAddresses?.FirstOrDefault(x => !x.Primary);

            var mobilePhone = user.PhoneNumbers?.FirstOrDefault(x => x.ItemType == "mobile");

            //if (mobilePhone is null) {
            //    throw new HttpResponseException(new HttpResponseMessage {
            //        StatusCode = HttpStatusCode.BadRequest,
            //        Content = new StringContent("Mobile phone is mandatory")
            //    });
            //}

            user.Locale ??= "en-US";

            var (isExists, existingUser) = await _userManager.UserExists(primaryEmail.Value, user.ExternalIdentifier);

            if (isExists && existingUser is not null) {
                user.Identifier = existingUser.UserId.ToString();

                user.Metadata = new Core2Metadata {
                    ResourceType = "User",
                    Created = existingUser.CreatedOn,
                    LastModified = existingUser.UpdatedOn
                };

                existingUser.ExternalScimId = user.ExternalIdentifier;

                await _authContext.SaveChangesAsync();

                //return user;
                throw new HttpResponseException(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.Conflict,
                    Content = new StringContent(user.Identifier)
                });
            }

            var userInput = new UserInput {
                Status = user.Active ? 1 : 0,
                CompanyId = _currentUser.CompanyId,
                FirstName = user.Name.GivenName,
                LastName = user.Name.FamilyName,
                ExternalScimId = user.ExternalIdentifier,
                CurrentUserId = _currentUser.UserId,
                ExpirePassword = false,
                PrimaryEmail = primaryEmail?.Value ?? string.Empty,
                SecondaryEmail = secondaryEmail?.Value ?? string.Empty,
                UserLanguage = new CultureInfo(user.Locale).TwoLetterISOLanguageName,
                MobileISDCode = string.Empty, //Otherwise error
                MobileNo = mobilePhone == null ? "" : mobilePhone.Value,
                UserRole = "USER",
                Password = string.Empty,
                LLISDCode = string.Empty,
                Landline = string.Empty,
                TimezoneId = currentTimeZone?.TimeZoneId ?? 1
            };

            var userResponse = await _userManager.AddUser(userInput);

            if (userResponse.UserId <= 0) {
                throw new HttpResponseException(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("User not created")
                });
            }

            user.Identifier = userResponse.UserId.ToString();

            user.Metadata = new Core2Metadata {
                ResourceType = "User",
                Created = userResponse.CreatedOn,
                LastModified = userResponse.UpdatedOn
            };

            return user;
        }

        public override async Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier) {
            if (string.IsNullOrWhiteSpace(resourceIdentifier.Identifier)) {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var identifier = resourceIdentifier.Identifier;

            var user = await _userManager.GetUser(resourceIdentifier.Identifier);

            if (user == null) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var userInput = _mapper.Map<UserInput>(user);

            userInput.Status = 3;

            await _userManager.UpdateUser(userInput);
        }

        public override async Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier) {
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier)) {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (string.IsNullOrEmpty(parameters.ResourceIdentifier?.Identifier)) {
                throw new ArgumentNullException(nameof(parameters));
            }

            var identifier = parameters.ResourceIdentifier.Identifier;

            var user = await _userManager.GetUser(identifier);

            if (user == null) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return ToCore2EnterpriseUser(user);
        }

        public override async Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier) {
            if (resource.Identifier == null) {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (resource is not Core2EnterpriseUser user) {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(user.UserName)) {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var userToUpdate =
                await _userManager.GetUser(resource.Identifier);

            if (userToUpdate is null) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var userToSend = _mapper.Map<UserInput>(userToUpdate);

            await UpdateUser(user, userToSend);

            user = ToCore2EnterpriseUser(userToUpdate);

            return user;
        }

        private async Task UpdateUser(Core2EnterpriseUser user, UserInput userToUpdate) {
            var primaryEmail = user.ElectronicMailAddresses?.FirstOrDefault(x => x.Primary);
            var secondaryEmail = user.ElectronicMailAddresses?.FirstOrDefault(x => !x.Primary);

            if (user.Name is not null) {
                userToUpdate.FirstName = user.Name.GivenName;
                userToUpdate.LastName = user.Name.FamilyName;
            }

            if (user.ExternalIdentifier is not null) {
                userToUpdate.ExternalScimId = user.ExternalIdentifier;
            }

            if (primaryEmail != null) {
                userToUpdate.PrimaryEmail = primaryEmail?.Value ?? userToUpdate.PrimaryEmail;
            }

            if (secondaryEmail != null) {
                userToUpdate.SecondaryEmail = secondaryEmail?.Value ?? userToUpdate.SecondaryEmail;
            }

            if (user.TimeZone != null) {
                var currentTimeZone = await _authContext.StdTimeZones
                    .FirstOrDefaultAsync(x => x.PortalTimeZone == user.TimeZone);

                userToUpdate.TimezoneId = currentTimeZone?.TimeZoneId ?? 1;
            }

            userToUpdate.Status = user.Active ? 1 : 0;

            await _userManager.UpdateUser(userToUpdate);
        }

        public override async Task UpdateAsync(IPatch patch, string correlationIdentifier) {
            if (null == patch) {
                throw new ArgumentNullException(nameof(patch));
            }

            if (null == patch.ResourceIdentifier) {
                throw new ArgumentException(string.Format(SystemForCrossDomainIdentityManagementServiceResources
                    .ExceptionInvalidOperation));
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.Identifier)) {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources
                    .ExceptionInvalidOperation);
            }

            if (null == patch.PatchRequest) {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources
                    .ExceptionInvalidOperation);
            }

            if (patch.PatchRequest is not PatchRequest2 patchRequest) {
                var unsupportedPatchTypeName = patch.GetType().FullName;
                throw new NotSupportedException(unsupportedPatchTypeName);
            }

            var userToUpdate =
                await _userManager.GetUser(patch.ResourceIdentifier.Identifier);

            if (userToUpdate is null) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var user = ToCore2EnterpriseUser(userToUpdate);

            user.Apply(patchRequest);

            var userToSend = _mapper.Map<UserInput>(userToUpdate);

            await UpdateUser(user, userToSend);
        }

        private static Core2EnterpriseUser ToCore2EnterpriseUser(UserResponse userToUpdate) {

            return new Core2EnterpriseUser {
                Identifier = userToUpdate.UserId.ToString(),
                ExternalIdentifier = userToUpdate.ExternalScimId,
                Active = userToUpdate.Status == 1,
                DisplayName = $"{userToUpdate.FirstName} {userToUpdate.LastName}",
                UserName = userToUpdate.PrimaryEmail,
                Nickname = userToUpdate.FirstName,
                ElectronicMailAddresses = new List<ElectronicMailAddress>() {
                    new ElectronicMailAddress { ItemType=ElectronicMailAddress.Work,Primary=true,Value=userToUpdate.PrimaryEmail},
                    new ElectronicMailAddress { ItemType=ElectronicMailAddress.Home,Primary=false,Value=userToUpdate.SecondaryEmail}
                },
                Name = new Name {
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
                Metadata = new Core2Metadata {
                    ResourceType = "User",
                    Created = userToUpdate.CreatedOn,
                    LastModified = userToUpdate.UpdatedOn
                }
            };
        }
    }
}