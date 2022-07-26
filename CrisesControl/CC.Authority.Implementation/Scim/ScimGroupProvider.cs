using System.Linq.Expressions;
using System.Net;
using System.Web.Http;
using CC.Authority.Implementation.Data;
using CC.Authority.Implementation.Helpers;
using CC.Authority.Implementation.Models;
using CC.Authority.SCIM;
using CC.Authority.SCIM.Protocol;
using CC.Authority.SCIM.Schemas;
using CC.Authority.SCIM.Service;
using Microsoft.SCIM.WebHostSample.Provider;
using Microsoft.Win32.SafeHandles;

namespace CC.Authority.Implementation.Scim
{
    public class ScimGroupProvider : ProviderBase
    {
        private readonly CrisesControlAuthContext _authContext;
        private readonly ICurrentUser _currentUser;

        public ScimGroupProvider(CrisesControlAuthContext authContext,
            ICurrentUser currentUser)
        {
            _authContext = authContext;
            _currentUser = currentUser;
        }

        public override async Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier != null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2Group group = resource as Core2Group;

            if (string.IsNullOrWhiteSpace(group.DisplayName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var existingGroups = _authContext.Groups.Where(g => g.GroupName == group.DisplayName).ToList();
            if
            (existingGroups.Any(existingGroup => string.Equals(existingGroup.GroupName, group.DisplayName, StringComparison.Ordinal)))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }
            //Update Metadata
            var created = DateTime.UtcNow;
            group.Metadata.Created = created;
            group.Metadata.LastModified = created;

            var newGroup = new Group
            {
                CompanyId = _currentUser.CompanyId,
                GroupName = group.DisplayName,
                CreatedBy = _currentUser.UserId,
                CreatedOn = created,
                UpdatedBy = _currentUser.UserId,
                UpdatedOn = created
            };

            await _authContext.Groups.AddAsync(newGroup);

            await _authContext.SaveChangesAsync();

            resource.Identifier = newGroup.GroupId.ToString();
           
            return resource;
        }

        public override Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
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

            var results = _authContext.Groups.Select(group => new Core2Group
            {
                DisplayName = group.GroupName,
                Identifier = group.GroupId.ToString(),
                Metadata = new Core2Metadata
                {
                    ResourceType = "Group",
                    Created = group.CreatedOn.DateTime,
                    LastModified = group.UpdatedOn.DateTime
                }
            }).ToArray();

            IFilter queryFilter = parameters.AlternateFilters.SingleOrDefault();

            var predicate = PredicateBuilder.False<Core2Group>();
            Expression<Func<Core2Group, bool>> predicateAnd;
            predicateAnd = PredicateBuilder.True<Core2Group>();

            if (queryFilter == null)
            {
                results = results.ToArray();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(queryFilter.AttributePath))
                {
                    throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
                }

                if (string.IsNullOrWhiteSpace(queryFilter.ComparisonValue))
                {
                    throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
                }

                if (queryFilter.FilterOperator != ComparisonOperator.Equals)
                {
                    throw new NotSupportedException(string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterOperatorNotSupportedTemplate, queryFilter.FilterOperator));
                }


                if (queryFilter.AttributePath.Equals(AttributeNames.DisplayName))
                {

                    string displayName = queryFilter.ComparisonValue;
                    predicateAnd = predicateAnd.And(p => string.Equals(p.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));

                }
                else
                {
                    throw new NotSupportedException(string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterAttributePathNotSupportedTemplate, queryFilter.AttributePath));
                }
            }

            predicate = predicate.Or(predicateAnd);
            results = results.Where(predicate.Compile()).ToArray();

            return Task.FromResult(results.Select(group => group as Resource).ToArray());
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (string.IsNullOrWhiteSpace(resourceIdentifier?.Identifier) 
                && !int.TryParse(resourceIdentifier?.Identifier, out _))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var id = int.Parse(resourceIdentifier?.Identifier);

            var group = _authContext.Groups.FirstOrDefault(g => g.GroupId == id);

            if (group is null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            _authContext.Groups.Remove(group);

            _authContext.SaveChanges();

            return Task.CompletedTask;
        }

        public override Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (string.IsNullOrEmpty(parameters?.ResourceIdentifier?.Identifier))
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            string identifier = parameters.ResourceIdentifier.Identifier;

            var group = _authContext.Groups.FirstOrDefault(g => g.GroupId.ToString() == identifier);

            if (group is not null)
            {
                var groupResult = new Core2Group
                {
                    DisplayName = group.GroupName,
                    Identifier = group.GroupId.ToString(),
                    Metadata = new Core2Metadata
                    {
                        ResourceType = "Group",
                        Created = group.CreatedOn.DateTime,
                        LastModified = group.UpdatedOn.DateTime
                    }
                };
                   
                return Task.FromResult((Resource)groupResult);
                
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        public override Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            if (null == patch)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            if (null == patch.ResourceIdentifier)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidOperation);
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.Identifier))
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidOperation);
            }

            if (null == patch.PatchRequest)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidOperation);
            }

            PatchRequest2 patchRequest =
                patch.PatchRequest as PatchRequest2;

            if (null == patchRequest)
            {
                string unsupportedPatchTypeName = patch.GetType().FullName;
                throw new NotSupportedException(unsupportedPatchTypeName);
            }

            string identifier = patch.ResourceIdentifier.Identifier;

            var group = _authContext.Groups.FirstOrDefault(g => g.GroupId.ToString() == identifier);

            if (group is not null)
            {
                var groupResult = new Core2Group
                {
                    DisplayName = group.GroupName,
                    Identifier = group.GroupId.ToString(),
                    Metadata = new Core2Metadata
                    {
                        ResourceType = "Group",
                        Created = group.CreatedOn.DateTime,
                        LastModified = group.UpdatedOn.DateTime
                    }
                };

                groupResult.Apply(patchRequest);

                groupResult.Metadata.LastModified = DateTime.UtcNow;

                group.GroupName = groupResult.DisplayName;
                group.UpdatedOn = groupResult.Metadata.LastModified;

            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Task.CompletedTask;
        }
    }
}