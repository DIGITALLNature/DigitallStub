// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Digitall.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Testing.Extensions;

public static class FakedDataverseBuilderExtensions
{
    public static FakedDataverseBuilder AddData(this FakedDataverseBuilder builder, IEnumerable<Entity> records)
    {
        builder.OrganizationService.AddRange(records);
        return builder;
    }

    public static FakedDataverseBuilder AddData(this FakedDataverseBuilder builder, params Entity[] records)
    {
        builder.OrganizationService.AddRange(records);
        return builder;
    }

    public static FakedDataverseBuilder AddOrganizationRequests(this FakedDataverseBuilder builder, IEnumerable<IOrganizationRequestFake> requests)
    {
        builder.OrganizationService.AddRequests(requests);
        return builder;
    }

    public static FakedDataverseBuilder AddOrganizationRequests(this FakedDataverseBuilder builder, params IOrganizationRequestFake[] requests)
    {
        builder.OrganizationService.AddRequests(requests);
        return builder;
    }

    public static FakedDataverseBuilder AddEntityMetadata(this FakedDataverseBuilder builder, IEnumerable<EntityMetadata> metadata)
    {
        builder.OrganizationService.AddMetadata(metadata);
        return builder;
    }

    public static FakedDataverseBuilder AddEntityMetadata(this FakedDataverseBuilder builder, params EntityMetadata[] metadata)
    {
        builder.OrganizationService.AddMetadata(metadata);
        return builder;
    }

    public static FakedDataverseBuilder AddRelationships(this FakedDataverseBuilder builder, IEnumerable<RelationshipMetadataBase> relationships)
    {
        builder.OrganizationService.AddRelationships(relationships);
        return builder;
    }

    public static FakedDataverseBuilder AddRelationships(this FakedDataverseBuilder builder, params RelationshipMetadataBase[] relationships)
    {
        builder.OrganizationService.AddRelationships(relationships);
        return builder;
    }

    /// <summary>
    /// Retrieves the underlying FakedDataverse service from the FakedDataverseBuilder at call time.
    /// </summary>
    /// <param name="builder">The FakedDataverseBuilder instance.</param>
    /// <param name="service">The output parameter for the FakedDataverse service.</param>
    /// <returns>The same FakedDataverseBuilder instance for method chaining.</returns>
    public static FakedDataverseBuilder GetFakedDataverse(this FakedDataverseBuilder builder, out FakedDataverse service)
    {
        // Assign the OrganizationService from the builder to the output parameter
        service = builder.OrganizationService;
        return builder;
    }

    /// <summary>
    /// Loads EntityMetadata from XML files or directory.
    /// </summary>
    /// <param name="builder">builder instance</param>
    /// <param name="path">The path to load from. Can be a file path to the XML file or path to the directory containing the XML files.</param>
    /// <returns>builder instance</returns>
    /// <exception cref="InvalidOperationException">path parameter is not a valid path</exception>
    public static FakedDataverseBuilder LoadMetadata(this FakedDataverseBuilder builder, string path)
    {
        var serializer = new DataContractSerializer(typeof(EntityMetadata));

        if (File.Exists(path))
        {
            var metadata = (EntityMetadata)serializer.ReadObject(File.OpenRead(path));
            builder.OrganizationService.AddMetadata(metadata);
        }
        else if (Directory.Exists(path))
        {
            var metadata = Directory.GetFiles(path, "*.xml").Select(file => (EntityMetadata)serializer.ReadObject(File.OpenRead(file)));
            builder.OrganizationService.AddMetadata(metadata);
        }
        else
        {
            throw new InvalidOperationException($"'{path}' is not a valid path.");
        }

        return builder;
    }
}
