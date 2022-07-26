using AutoMapper;
using CC.Authority.Core.UserManagement;
using CC.Authority.Implementation.Data;
using CC.Authority.Implementation.Helpers;
using CC.Authority.SCIM.Protocol;
using CC.Authority.SCIM.Schemas;
using CC.Authority.SCIM.Service;
using Microsoft.SCIM.WebHostSample.Resources;

namespace CC.Authority.Implementation.Scim
{
    public class ScimProvider : ProviderBase
    {
        private readonly ProviderBase _groupProvider;
        private readonly ProviderBase _userProvider;
        private readonly ICurrentUser _currentUser;

        private readonly CrisesControlAuthContext _authContext;

        private static readonly Lazy<IReadOnlyCollection<TypeScheme>> TypeSchema =
            new Lazy<IReadOnlyCollection<TypeScheme>>(
                () =>
                    new TypeScheme[]
                    {
                        SampleTypeScheme.UserTypeScheme,
                        SampleTypeScheme.GroupTypeScheme,
                        SampleTypeScheme.EnterpriseUserTypeScheme,
                        SampleTypeScheme.ResourceTypesTypeScheme,
                        SampleTypeScheme.SchemaTypeScheme,
                        SampleTypeScheme.ServiceProviderConfigTypeScheme
                    });

        private static readonly Lazy<IReadOnlyCollection<Core2ResourceType>> Types =
            new Lazy<IReadOnlyCollection<Core2ResourceType>>(
                () =>
                    new Core2ResourceType[] { SampleResourceTypes.UserResourceType, SampleResourceTypes.GroupResourceType });

        public ScimProvider(CrisesControlAuthContext context, ICurrentUser currentUser, IUserManager userManager, IMapper mapper)
        {
            _authContext = context;
            _currentUser = currentUser;

            this._groupProvider = new ScimGroupProvider(this._authContext, currentUser);
            this._userProvider = new ScimUserProvider(this._authContext, currentUser, userManager, mapper);
        }

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            return resource switch
            {
                Core2EnterpriseUser => _userProvider.CreateAsync(resource, correlationIdentifier),
                Core2Group => _groupProvider.CreateAsync(resource, correlationIdentifier),
                _ => throw new NotImplementedException()
            };
        }

        public override Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            return parameters.SchemaIdentifier switch
            {
                SchemaIdentifiers.Core2EnterpriseUser => _userProvider.QueryAsync(parameters, correlationIdentifier),
                SchemaIdentifiers.Core2Group => _groupProvider.QueryAsync(parameters, correlationIdentifier),
                _ => throw new NotImplementedException()
            };
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            return resourceIdentifier.SchemaIdentifier switch
            {
                SchemaIdentifiers.Core2EnterpriseUser => _userProvider.DeleteAsync(resourceIdentifier,
                    correlationIdentifier),
                SchemaIdentifiers.Core2Group => _groupProvider.DeleteAsync(resourceIdentifier, correlationIdentifier),
                _ => throw new NotImplementedException()
            };
        }

        public override Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            return parameters.SchemaIdentifier switch
            {
                SchemaIdentifiers.Core2EnterpriseUser => _userProvider.RetrieveAsync(parameters, correlationIdentifier),
                SchemaIdentifiers.Core2Group => _groupProvider.RetrieveAsync(parameters, correlationIdentifier),
                _ => throw new NotImplementedException()
            };
        }

        public override Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource is Core2EnterpriseUser)
            {
                return _userProvider.ReplaceAsync(resource, correlationIdentifier);
            }

            if (resource is Core2Group)
            {
                return _groupProvider.ReplaceAsync(resource, correlationIdentifier);
            }

            throw new NotImplementedException();
        }

        public override Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.Identifier))
            {
                throw new ArgumentException(nameof(patch));
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.SchemaIdentifier))
            {
                throw new ArgumentException(nameof(patch));
            }

            return patch.ResourceIdentifier.SchemaIdentifier switch
            {
                SchemaIdentifiers.Core2EnterpriseUser => _userProvider.UpdateAsync(patch, correlationIdentifier),
                SchemaIdentifiers.Core2Group => _groupProvider.UpdateAsync(patch, correlationIdentifier),
                _ => throw new NotImplementedException()
            };
        }
    }
}