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

public static class FakeDataverseBuilderExtensions
{
    /// <param name="builder">builder instance</param>
    extension<TBuilder>(TBuilder builder) where TBuilder : IFakeDataverseBuilder<FakeOrganizationService>
    {
        public TBuilder AddData(IEnumerable<Entity> records)
        {
            builder.OrganizationService.AddRange(records);
            return builder;
        }

        public TBuilder AddData(params Entity[] records)
        {
            builder.OrganizationService.AddRange(records);
            return builder;
        }

        public TBuilder AddOrganizationRequests(IEnumerable<IOrganizationRequestFake> requests)
        {
            builder.OrganizationService.AddRequests(requests);
            return builder;
        }

        public TBuilder AddOrganizationRequests(params IOrganizationRequestFake[] requests)
        {
            builder.OrganizationService.AddRequests(requests);
            return builder;
        }

        public TBuilder AddEntityMetadata(IEnumerable<EntityMetadata> metadata)
        {
            builder.OrganizationService.AddMetadata(metadata);
            return builder;
        }

        public TBuilder AddEntityMetadata(params EntityMetadata[] metadata)
        {
            builder.OrganizationService.AddMetadata(metadata);
            return builder;
        }

        public TBuilder AddRelationships(IEnumerable<RelationshipMetadataBase> relationships)
        {
            builder.OrganizationService.AddRelationships(relationships);
            return builder;
        }

        public TBuilder AddRelationships(params RelationshipMetadataBase[] relationships)
        {
            builder.OrganizationService.AddRelationships(relationships);
            return builder;
        }

        /// <summary>
        /// Retrieves the underlying <see cref="FakeOrganizationService"/> from the builder at call time.
        /// </summary>
        public TBuilder GetFakedDataverse(out FakeOrganizationService service)
        {
            service = builder.OrganizationService;
            return builder;
        }

        /// <summary>
        /// Adds an environment variable configuration entry to the builder's organization service.
        /// Optionally adds a value override on top of the default.
        /// </summary>
        public TBuilder AddConfig(string key, string defaultValue, string? value = null)
        {
            var envVarDef = new Entity("environmentvariabledefinition") { Id = Guid.NewGuid(), ["schemaname"] = key, ["defaultvalue"] = defaultValue };
            builder.OrganizationService.Add(envVarDef);

            if (!string.IsNullOrWhiteSpace(value))
            {
                var envVarVal = new Entity("environmentvariablevalue") { Id = Guid.NewGuid(), ["environmentvariabledefinitionid"] = envVarDef.ToEntityReference(), ["value"] = value };
                builder.OrganizationService.Add(envVarVal);
            }

            return builder;
        }

        /// <summary>
        /// Loads EntityMetadata from XML files or a directory.
        /// </summary>
        /// <param name="path">The path to load from. Can be a file path to the XML file or path to the directory containing the XML files.</param>
        /// <returns>builder instance</returns>
        /// <exception cref="InvalidOperationException">path parameter is not a valid path</exception>
        public TBuilder LoadMetadata(string path)
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
}
