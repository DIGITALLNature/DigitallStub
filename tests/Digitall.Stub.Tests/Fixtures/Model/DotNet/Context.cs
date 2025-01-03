using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

// ReSharper disable All
namespace Digitall.Stub.Tests.Fixtures
{
   	public partial class DataContext : OrganizationServiceContext
    {
        private readonly bool _noLock;

        public DataContext(IOrganizationService service, bool noLock = true) : base(service)
        {
            _noLock = noLock;
        }

        protected override void OnExecuting(OrganizationRequest request)
        {
            if (_noLock)
            {
                if (request is RetrieveMultipleRequest retrieveMultipleRequest && retrieveMultipleRequest.Query is QueryExpression queryExpression)
                {
                    queryExpression.NoLock = true;
                }
            }

            base.OnExecuting(request);
        }
	}
}
