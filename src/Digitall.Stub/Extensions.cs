// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub;

public static class Extensions
{

    public static object KeySelector(this Entity e, string sAttributeName)
    {
        if (sAttributeName.Contains("."))
        {
            //Do not lowercase the alias prefix
            var splitted = sAttributeName.Split('.');
            sAttributeName = string.Format("{0}.{1}", splitted[0], splitted[1].ToLower());
        }
        else
        {
            sAttributeName = sAttributeName.ToLower();
        }

        if (!e.Attributes.ContainsKey(sAttributeName))
        {
            //Check if it is the primary key
            if (sAttributeName.Contains("id") &&
                e.LogicalName.ToLower().Equals(sAttributeName.Substring(0, sAttributeName.Length - 2)))
            {
                return e.Id;
            }

            return Guid.Empty; //Atrribute is null or doesn´t exists so it can´t be joined
        }

        object keyValue = null;
        AliasedValue aliasedValue;
        if ((aliasedValue = e[sAttributeName] as AliasedValue) != null)
        {
            keyValue = aliasedValue.Value;
        }
        else
        {
            keyValue = e[sAttributeName];
        }

        var entityReference = keyValue as EntityReference;
        if (entityReference != null)
        {
            return entityReference.Id;
        }

        var optionSetValue = keyValue as OptionSetValue;
        if (optionSetValue != null)
        {
            return optionSetValue.Value;
        }

        var money = keyValue as Money;
        if (money != null)
        {
            return money.Value;
        }

        return keyValue;
    }
    public static Entity ProjectAttributes(this Entity e, ColumnSet qs, DataverseStub state)
    {
        if (qs == null || qs.AllColumns)
        {
            return RemoveNullAttributes(e); //return all the original attributes
        }

        //Return selected list of attributes in a projected entity
        var projected = new Entity(e.LogicalName) { Id = e.Id };


        foreach (var attKey in qs.Columns)
        {
            state.ThrowIfNotKnownAttribute(e.LogicalName, attKey);

            if (e.Attributes.ContainsKey(attKey) && e.Attributes[attKey] != null)
            {
                projected[attKey] = CloneAttribute(e[attKey]);
                if (e.FormattedValues.TryGetValue(attKey, out var formattedValue))
                {
                    projected.FormattedValues[attKey] = formattedValue;
                }
            }
        }

        return RemoveNullAttributes(projected);
    }

    public static Entity ProjectAttributes(this Entity e, QueryExpression qe, DataverseStub state)
    {
        if (qe.ColumnSet == null || qe.ColumnSet.AllColumns)
        {
            return RemoveNullAttributes(e); //return all the original attributes
        }

        //Return selected list of attributes in a projected entity
        var projected = new Entity(e.LogicalName) { Id = e.Id };


        foreach (var attKey in qe.ColumnSet.Columns)
        {
            state.ThrowIfNotKnownAttribute(e.LogicalName, attKey);

            if (e.Attributes.ContainsKey(attKey) && e.Attributes[attKey] != null)
            {
                projected[attKey] = CloneAttribute(e[attKey]);
                if (e.FormattedValues.TryGetValue(attKey, out var formattedValue))
                {
                    projected.FormattedValues[attKey] = formattedValue;
                }
            }
        }


        //Plus attributes from joins
        foreach (var le in qe.LinkEntities)
        {
            ProjectLinkedEntitiesAttributes(RemoveNullAttributes(e), projected, le);
        }

        return RemoveNullAttributes(projected);
    }
    /// <summary>
    ///     Clones an attribute value.
    /// </summary>
    /// <param name="attributeValue">The attribute value to clone.</param>
    /// <returns>The cloned attribute value.</returns>
    internal static object CloneAttribute(object attributeValue)
    {
        // If the attribute value is null, return null.
        if (attributeValue == null)
        {
            return null;
        }

        // If the attribute value is a string, create a new string with the same characters.
        if (attributeValue is string text)
        {
            return new string(text.ToCharArray());
        }

        // If the attribute value is an EntityReference, create a new EntityReference with the same logical name and ID.
        if (attributeValue is EntityReference reference)
        {
            var clonedReference = new EntityReference(reference.LogicalName, reference.Id);
            clonedReference.Name = (string)CloneAttribute(reference.Name);

            // If the reference has key attributes, clone them.
            if (reference.KeyAttributes != null)
            {
                var clonedKeyAttributes = new KeyAttributeCollection();
                clonedKeyAttributes.AddRange(reference.KeyAttributes.Select(kvp => new KeyValuePair<string, object>(
                    (string)CloneAttribute(kvp.Key),
                    kvp.Value
                )).ToArray());

                clonedReference.KeyAttributes = clonedKeyAttributes;
            }

            return clonedReference;
        }

        // If the attribute value is a BooleanManagedProperty, create a new BooleanManagedProperty with the same value.
        if (attributeValue is BooleanManagedProperty booleanManagedProperty)
        {
            return new BooleanManagedProperty(booleanManagedProperty.Value);
        }

        // If the attribute value is an OptionSetValue, create a new OptionSetValue with the same value.
        if (attributeValue is OptionSetValue optionSetValue)
        {
            return new OptionSetValue(optionSetValue.Value);
        }

        // If the attribute value is an AliasedValue, create a new AliasedValue with the same entity logical name, attribute logical name, and cloned value.
        if (attributeValue is AliasedValue aliasedValue)
        {
            var clonedAliasedValue = new AliasedValue(
                aliasedValue.EntityLogicalName,
                aliasedValue.AttributeLogicalName,
                CloneAttribute(aliasedValue.Value)
            );

            return clonedAliasedValue;
        }

        // If the attribute value is a Money, create a new Money with the same value.
        if (attributeValue is Money money)
        {
            return new Money(money.Value);
        }

        // If the attribute value is an EntityCollection, clone each entity in the collection.
        if (attributeValue is EntityCollection collection)
        {
            var clonedEntities = collection.Entities.Select(e => e.CloneEntity()).ToList();
            return new EntityCollection(clonedEntities);
        }

        // If the attribute value is an IEnumerable of entities, clone each entity in the collection.
        if (attributeValue is IEnumerable<Entity> entities)
        {
            return entities.Select(e => e.CloneEntity()).ToArray();
        }

        // If the attribute value is a byte array, create a new byte array with the same values.
        if (attributeValue is byte[] bytes)
        {
            var clonedBytes = new byte[bytes.Length];
            bytes.CopyTo(clonedBytes, 0);
            return clonedBytes;
        }

        // If the attribute value is an OptionSetValueCollection, create a new OptionSetValueCollection with the same values.
        if (attributeValue is OptionSetValueCollection optionSetValues)
        {
            var clonedOptionSetValues = new OptionSetValueCollection(optionSetValues.ToArray());
            return clonedOptionSetValues;
        }

        // If the attribute value is none of the above, return the original attribute value.
        return attributeValue;
    }

    static void ProjectLinkedEntitiesAttributes(Entity e, Entity projected, LinkEntity le)
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
        IList<string> nullAttributes = entity.Attributes
            .Where(attribute => attribute.Value == null || (attribute.Value is AliasedValue aliasedValue && aliasedValue.Value == null))
            .Select(attribute => attribute.Key).ToList();

        // Remove each null attribute from the entity.
        foreach (var nullAttribute in nullAttributes)
        {
            entity.Attributes.Remove(nullAttribute);
        }

        return entity;
    }

    public static Entity JoinAttributes(this Entity e, IEnumerable<Entity> otherEntities, ColumnSet columnSet, string alias)
    {
        foreach (var otherEntity in otherEntities)
        {
            e.JoinAttributes(otherEntity, columnSet, alias);
        }
        return e;
    }

    public static Entity JoinAttributes(this Entity e, Entity otherEntity, ColumnSet columnSet, string alias)
    {
        if (otherEntity == null) return e; //Left Join where otherEntity was not matched

        otherEntity = otherEntity.CloneEntity(); //To avoid joining entities from/to the same entities, which would cause collection modified exceptions

        if (columnSet.AllColumns)
        {
            foreach (var attKey in otherEntity.Attributes.Keys)
            {
                e[alias + "." + attKey] = new AliasedValue(otherEntity.LogicalName, attKey, otherEntity[attKey]);
            }

            foreach (var attKey in otherEntity.FormattedValues.Keys)
            {
                e.FormattedValues[alias + "." + attKey] = otherEntity.FormattedValues[attKey];
            }
        }
        else
        {
            //Return selected list of attributes
            foreach (var attKey in columnSet.Columns)
            {
                if (otherEntity.Attributes.ContainsKey(attKey))
                {
                    e[alias + "." + attKey] = new AliasedValue(otherEntity.LogicalName, attKey, otherEntity[attKey]);
                }
                else
                {
                    e[alias + "." + attKey] = new AliasedValue(otherEntity.LogicalName, attKey, null);
                }

                if (otherEntity.FormattedValues.ContainsKey(attKey))
                {
                    e.FormattedValues[alias + "." + attKey] = otherEntity.FormattedValues[attKey];
                }
            }
        }
        return e;
    }


}
