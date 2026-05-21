// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Dataverse.Testing.Extensions;

internal static class FakeOrganizationServiceStateExtensions
{
    extension(FakeOrganizationServiceState state)
    {
        public OptionSetValue GetDefaultStateCode(string entityLogicalName)
        {
            // Safe universal fallback — correct for most standard entities
            var defaultStateCode = new OptionSetValue(0);

            // no entity metadata available in state
            if (!state.EntityMetadata.TryGetValue(entityLogicalName, out var metadata) || metadata.Attributes == null) return defaultStateCode;

            // we assume that the default state is the first state in the list (typically 0)
            var stateAttributeMetadata = metadata.Attributes.OfType<StateAttributeMetadata>().FirstOrDefault();
            var defaultStateOption = stateAttributeMetadata?.OptionSet?.Options.OfType<StateOptionMetadata>().OrderBy(o => o.Value).FirstOrDefault();

            return defaultStateOption?.Value != null ? new OptionSetValue(defaultStateOption.Value.Value) : defaultStateCode;
        }

        public OptionSetValue GetDefaultStatusCode(string entityLogicalName, int stateCode)
        {
            // Safe universal fallback — correct for most standard entities
            var defaultStatusCode = new OptionSetValue(1);

            // no entity metadata available in state
            if (!state.EntityMetadata.TryGetValue(entityLogicalName, out var metadata) || metadata.Attributes == null) return defaultStatusCode;

            // look for provided statecode in metadata
            var stateAttributeMetadata = metadata.Attributes.OfType<StateAttributeMetadata>().FirstOrDefault();
            var stateOption = stateAttributeMetadata?.OptionSet?.Options.OfType<StateOptionMetadata>().FirstOrDefault(o => o.Value == stateCode);

            // use default statuscode if available
            if (stateOption?.DefaultStatus != null) return new OptionSetValue(stateOption.DefaultStatus.Value);

            var statusAttributeMetadata = metadata.Attributes.OfType<StatusAttributeMetadata>().FirstOrDefault();
            var defaultStatusOption = statusAttributeMetadata?.OptionSet?.Options.OfType<StatusOptionMetadata>().OrderBy(o => o.Value).FirstOrDefault(o => o.State == stateCode);

            return defaultStatusOption?.Value != null ? new OptionSetValue(defaultStatusOption.Value.Value) : defaultStatusCode;
        }
    }
}
