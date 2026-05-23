// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Extensions;

public static class EntityExtensions
{
    extension(Entity entity)
    {
        public object KeySelector(string sAttributeName)
        {
            if (sAttributeName.Contains('.'))
            {
                //Do not lowercase the alias prefix
                var splitted = sAttributeName.Split('.');
                sAttributeName = $"{splitted[0]}.{splitted[1].ToLowerInvariant()}";
            }
            else
            {
                sAttributeName = sAttributeName.ToLowerInvariant();
            }

            if (!entity.Attributes.ContainsKey(sAttributeName))
            {
                //Check if it is the primary key
                if (sAttributeName.Contains("id") && entity.LogicalName.ToLowerInvariant().Equals(sAttributeName[..^2]))
                {
                    return entity.Id;
                }

                return Guid.Empty; //Atrribute is null or doesn´t exists so it can´t be joined
            }

            var keyValue = entity[sAttributeName] is AliasedValue av ? av.Value : entity[sAttributeName];

            return keyValue switch
            {
                EntityReference entityReference => entityReference.Id,
                OptionSetValue optionSetValue => optionSetValue.Value,
                Money money => money.Value,
                _ => keyValue
            };
        }

        public Entity ProjectAttributes(ColumnSet qs, FakeOrganizationService state)
        {
            if (qs.AllColumns)
            {
                return RemoveNullAttributes(entity); //return all the original attributes
            }

            //Return selected list of attributes in a projected entity
            var projected = new Entity(entity.LogicalName) { Id = entity.Id };


            foreach (var attKey in qs.Columns)
            {
                state.ThrowIfNotKnownAttribute(entity.LogicalName, attKey);

                if (!entity.Attributes.ContainsKey(attKey) || entity.Attributes[attKey] == null) continue;

                projected[attKey] = CloneAttribute(entity[attKey]);
                if (entity.FormattedValues.TryGetValue(attKey, out var formattedValue))
                {
                    projected.FormattedValues[attKey] = formattedValue;
                }
            }

            return RemoveNullAttributes(projected);
        }

        public Entity ProjectAttributes(QueryExpression qe, FakeOrganizationService state)
        {
            if (qe.ColumnSet == null || qe.ColumnSet.AllColumns)
            {
                return RemoveNullAttributes(entity); //return all the original attributes
            }

            //Return selected list of attributes in a projected entity
            var projected = new Entity(entity.LogicalName) { Id = entity.Id };

            foreach (var attKey in qe.ColumnSet.Columns)
            {
                state.ThrowIfNotKnownAttribute(entity.LogicalName, attKey);

                if (!entity.Attributes.ContainsKey(attKey) || entity.Attributes[attKey] == null) continue;

                projected[attKey] = CloneAttribute(entity[attKey]);
                if (entity.FormattedValues.TryGetValue(attKey, out var formattedValue))
                {
                    projected.FormattedValues[attKey] = formattedValue;
                }
            }


            //Plus attributes from joins
            foreach (var le in qe.LinkEntities)
            {
                ProjectLinkedEntitiesAttributes(RemoveNullAttributes(entity), projected, le);
            }

            return RemoveNullAttributes(projected);
        }

        public Entity CloneEntity()
        {
            // Intentionally create a plain Entity instance (no deep-clone of the runtime type)
            // so the result is always Entity, never a typed proxy subclass.
            var cloned = new Entity(entity.LogicalName)
            {
                Id = entity.Id,
                EntityState = entity.EntityState,
                RowVersion = entity.RowVersion,
            };

            foreach (var attribute in entity.Attributes)
            {
                cloned.Attributes[attribute.Key] = CloneAttribute(attribute.Value);
            }

            foreach (var formattedValue in entity.FormattedValues)
            {
                cloned.FormattedValues[formattedValue.Key] = formattedValue.Value;
            }

            foreach (var keyAttribute in entity.KeyAttributes)
            {
                cloned.KeyAttributes[keyAttribute.Key] = CloneAttribute(keyAttribute.Value);
            }

            foreach (var relatedEntity in entity.RelatedEntities)
            {
                cloned.RelatedEntities[relatedEntity.Key] =
                    new EntityCollection(relatedEntity.Value.Entities.Select(e => e.CloneEntity()).ToList());
            }

            return cloned;
        }

        public Entity JoinAttributes(Entity? otherEntity, ColumnSet columnSet, string alias)
        {
            if (otherEntity == null)
            {
                return entity;
            }

            otherEntity = otherEntity.CloneEntity(); //To avoid joining entities from/to the same entities, which would cause collection modified exceptions

            if (columnSet.AllColumns)
            {
                foreach (var attKey in otherEntity.Attributes.Keys)
                {
                    entity[alias + "." + attKey] = new AliasedValue(otherEntity.LogicalName, attKey, otherEntity[attKey]);
                }

                foreach (var attKey in otherEntity.FormattedValues.Keys)
                {
                    entity.FormattedValues[alias + "." + attKey] = otherEntity.FormattedValues[attKey];
                }
            }
            else
            {
                //Return selected list of attributes
                foreach (var attKey in columnSet.Columns)
                {
                    if (otherEntity.Attributes.ContainsKey(attKey))
                    {
                        entity[alias + "." + attKey] = new AliasedValue(otherEntity.LogicalName, attKey, otherEntity[attKey]);
                    }
                    else
                    {
                        entity[alias + "." + attKey] = new AliasedValue(otherEntity.LogicalName, attKey, null);
                    }

                    if (otherEntity.FormattedValues.ContainsKey(attKey))
                    {
                        entity.FormattedValues[alias + "." + attKey] = otherEntity.FormattedValues[attKey];
                    }
                }
            }

            return entity;
        }
    }

    /// <summary>
    ///     Clones an attribute value.
    /// </summary>
    /// <param name="attributeValue">The attribute value to clone.</param>
    /// <returns>The cloned attribute value.</returns>
    private static object? CloneAttribute(object? attributeValue)
    {
        switch (attributeValue)
        {
            // If the attribute value is null, return null.
            case null:
                return null;
            // If the attribute value is a string, create a new string with the same characters.
            case string text:
                return new string(text.ToCharArray());
            // If the attribute value is an EntityReference, create a new EntityReference with the same logical name and ID.
            case EntityReference reference:
                {
                    var clonedReference = new EntityReference(reference.LogicalName, reference.Id) { Name = (string?)CloneAttribute(reference.Name) };

                    // If the reference has key attributes, clone them.
                    if (reference.KeyAttributes == null) return clonedReference;

                    var clonedKeyAttributes = new KeyAttributeCollection();
                    clonedKeyAttributes.AddRange(reference.KeyAttributes.Select(kvp => new KeyValuePair<string, object?>(kvp.Key, CloneAttribute(kvp.Value))).ToArray());

                    clonedReference.KeyAttributes = clonedKeyAttributes;

                    return clonedReference;
                }
            // If the attribute value is a BooleanManagedProperty, create a new BooleanManagedProperty with the same value.
            case BooleanManagedProperty booleanManagedProperty:
                return new BooleanManagedProperty(booleanManagedProperty.Value);
            // If the attribute value is an OptionSetValue, create a new OptionSetValue with the same value.
            case OptionSetValue optionSetValue:
                return new OptionSetValue(optionSetValue.Value);
            // If the attribute value is an AliasedValue, create a new AliasedValue with the same entity logical name, attribute logical name, and cloned value.
            case AliasedValue aliasedValue:
                {
                    var clonedAliasedValue = new AliasedValue(aliasedValue.EntityLogicalName, aliasedValue.AttributeLogicalName, CloneAttribute(aliasedValue.Value));

                    return clonedAliasedValue;
                }
            // If the attribute value is a Money, create new Money with the same value.
            case Money money:
                return new Money(money.Value);
            // If the attribute value is an EntityCollection, clone each entity in the collection.
            case EntityCollection collection:
                {
                    var clonedEntities = collection.Entities.Select(e => e.CloneEntity()).ToList();
                    return new EntityCollection(clonedEntities);
                }
            // If the attribute value is an IEnumerable of entities, clone each entity in the collection.
            case IEnumerable<Entity> entities:
                return entities.Select(e => e.CloneEntity()).ToArray();
            // If the attribute value is a byte array, create a new byte array with the same values.
            case byte[] bytes:
                {
                    var clonedBytes = new byte[bytes.Length];
                    bytes.CopyTo(clonedBytes, 0);
                    return clonedBytes;
                }
            // If the attribute value is an OptionSetValueCollection, create a new OptionSetValueCollection
            // with cloned OptionSetValue instances so mutations to either collection are fully independent.
            case OptionSetValueCollection optionSetValues:
                {
                    var clonedOptionSetValues = new OptionSetValueCollection(optionSetValues.Select(v => new OptionSetValue(v.Value)).ToArray());
                    return clonedOptionSetValues;
                }
            default:
                // If the attribute value is none of the above, return the original attribute value.
                return attributeValue;
        }
    }

    private static void ProjectLinkedEntitiesAttributes(Entity e, Entity projected, LinkEntity le)
    {
        var sAlias = string.IsNullOrWhiteSpace(le.EntityAlias) ? le.LinkToEntityName : le.EntityAlias;

        if (le.Columns.AllColumns)
        {
            foreach (var attKey in e.Attributes.Keys)
            {
                if (attKey.StartsWith(sAlias + ".", StringComparison.Ordinal))
                {
                    projected[attKey] = e[attKey];
                }
            }

            foreach (var attKey in e.FormattedValues.Keys)
            {
                if (attKey.StartsWith(sAlias + ".", StringComparison.Ordinal))
                {
                    projected.FormattedValues[attKey] = e.FormattedValues[attKey];
                }
            }
        }
        else
        {
            foreach (var attKey in le.Columns.Columns)
            {
                var linkedAttKey = sAlias + "." + attKey;
                if (e.Attributes.ContainsKey(linkedAttKey))
                {
                    projected[linkedAttKey] = e[linkedAttKey];
                }

                if (e.FormattedValues.ContainsKey(linkedAttKey))
                {
                    projected.FormattedValues[linkedAttKey] = e.FormattedValues[linkedAttKey];
                }
            }
        }

        foreach (var nestedLinkedEntity in le.LinkEntities)
        {
            ProjectLinkedEntitiesAttributes(e, projected, nestedLinkedEntity);
        }
    }

    /// <summary>
    ///     Removes any attributes from the given entity that have a null value.
    /// </summary>
    /// <param name="entity">The entity from which to remove null attributes.</param>
    /// <returns>The entity with null attributes removed.</returns>
    private static Entity RemoveNullAttributes(Entity entity)
    {
        // Find all attributes that have a null value or an AliasedValue with a null value.
        IList<string> nullAttributes = entity.Attributes.Where(attribute => attribute.Value is null or AliasedValue { Value: null }).Select(attribute => attribute.Key).ToList();

        // Remove each null attribute from the entity.
        foreach (var nullAttribute in nullAttributes)
        {
            entity.Attributes.Remove(nullAttribute);
        }

        return entity;
    }
}
