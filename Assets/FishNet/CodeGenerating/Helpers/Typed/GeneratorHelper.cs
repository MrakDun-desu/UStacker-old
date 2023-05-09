﻿using System;
using System.Collections.Generic;
using FishNet.CodeGenerating.Helping.Extension;
using FishNet.Object;
using FishNet.Serializing.Helping;
using FishNet.Utility.Performance;
using MonoFN.Cecil;
using UnityEngine;

namespace FishNet.CodeGenerating.Helping
{
    internal class GeneratorHelper : CodegenBase
    {
        /// <summary>
        ///     Gets what objectTypeRef will be serialized as.
        /// </summary>
        public SerializerType GetSerializerType(TypeReference objectTr, bool writer, out TypeDefinition objectTd)
        {
            var errorPrefix = writer ? "CreateWrite: " : "CreateRead: ";
            objectTd = null;

            /* Check if already has a serializer. */
            if (writer)
            {
                if (GetClass<WriterProcessor>().GetWriteMethodReference(objectTr) != null)
                {
                    LogError($"Writer already exist for {objectTr.FullName}.");
                    return SerializerType.Invalid;
                }
            }
            else
            {
                if (GetClass<ReaderProcessor>().GetReadMethodReference(objectTr) != null)
                {
                    LogError($"Reader already exist for {objectTr.FullName}.");
                    return SerializerType.Invalid;
                }
            }

            objectTd = objectTr.CachedResolve(Session);
            //Invalid typeDef.
            if (objectTd == null)
            {
                LogError($"{errorPrefix}{objectTd.FullName} could not be resolved.");
                return SerializerType.Invalid;
            }

            //Intentionally excluded.
            if (objectTd.CustomAttributes.Count > 0)
                foreach (var item in objectTd.CustomAttributes)
                    if (item.AttributeType.Is(typeof(CodegenExcludeAttribute)))
                        return SerializerType.Invalid;

            //By reference.            
            if (objectTr.IsByReference)
            {
                LogError($"{errorPrefix}Cannot pass {objectTr.Name} by reference");
                return SerializerType.Invalid;
            }
            /* Arrays have to be processed first because it's possible for them to meet other conditions
             * below and be processed wrong. */

            if (objectTr.IsArray)
            {
                if (objectTr.IsMultidimensionalArray())
                {
                    LogError(
                        $"{errorPrefix}{objectTr.Name} is an unsupported type. Multidimensional arrays are not supported");
                    return SerializerType.Invalid;
                }

                return SerializerType.Array;
            }
            //Enum.

            if (objectTd.IsEnum) return SerializerType.Enum;

            if (objectTd.Is(typeof(Dictionary<,>))) return SerializerType.Dictionary;

            if (objectTd.Is(typeof(List<>))) return SerializerType.List;

            if (objectTd.Is(typeof(ListCache<>))) return SerializerType.ListCache;

            if (objectTd.InheritsFrom<NetworkBehaviour>(Session)) return SerializerType.NetworkBehaviour;

            if (objectTr.Name == typeof(Nullable<>).Name)
            {
                var git = objectTr as GenericInstanceType;
                if (git == null || git.GenericArguments.Count != 1)
                    return SerializerType.Invalid;
                return SerializerType.Nullable;
            }
            //Invalid type. This must be called after trying to generate everything but class.

            if (!CanGenerateSerializer(objectTd)) return SerializerType.Invalid;
            //If here then the only type left is struct or class.
            if (objectTr.IsClassOrStruct(Session))
            {
                return SerializerType.ClassOrStruct;
            }
            //Unknown type.

            LogError(
                $"{errorPrefix}{objectTr.Name} is an unsupported type. Mostly because we don't know what the heck it is. Please let us know so we can fix this.");
            return SerializerType.Invalid;
        }


        /// <summary>
        ///     Returns if objectTd can have a serializer generated for it.
        /// </summary>
        private bool CanGenerateSerializer(TypeDefinition objectTd)
        {
            var errorText =
                $"{objectTd.Name} is not a supported type. Use a supported type or provide a custom serializer";

            var unityObjectType = typeof(UnityEngine.Object);
            //Unable to determine type, cannot generate for.
            if (objectTd == null)
            {
                LogError(errorText);
                return false;
            }

            //Component.
            if (objectTd.InheritsFrom<UnityEngine.Component>(Session))
            {
                LogError(errorText);
                return false;
            }

            //Unity Object.
            if (objectTd.Is(unityObjectType))
            {
                LogError(errorText);
                return false;
            }

            //ScriptableObject.
            if (objectTd.Is(typeof(ScriptableObject)))
            {
                LogError(errorText);
                return false;
            }

            //Has generic parameters.
            if (objectTd.HasGenericParameters)
            {
                LogError(errorText);
                return false;
            }

            //Is an interface.
            if (objectTd.IsInterface)
            {
                LogError(errorText);
                return false;
            }

            //Is abstract.
            if (objectTd.IsAbstract)
            {
                LogError(errorText);
                return false;
            }

            if (objectTd.InheritsFrom(Session, unityObjectType) &&
                objectTd.IsExcluded(GeneralHelper.UNITYENGINE_ASSEMBLY_PREFIX))
            {
                LogError(errorText);
                return false;
            }

            //If here type is valid.
            return true;
        }
    }
}