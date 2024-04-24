//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using System;
using System.Text;
using MonoFN.Collections.Generic;

namespace MonoFN.Cecil
{
    public sealed class CallSite : IMethodSignature
    {
        private readonly MethodReference signature;

        internal CallSite()
        {
            signature = new MethodReference();
            signature.token = new MetadataToken(TokenType.Signature, 0);
        }

        public CallSite(TypeReference returnType)
            : this()
        {
            if (returnType == null)
                throw new ArgumentNullException("returnType");

            signature.ReturnType = returnType;
        }

        public string Name
        {
            get => string.Empty;
            set => throw new InvalidOperationException();
        }

        public string Namespace
        {
            get => string.Empty;
            set => throw new InvalidOperationException();
        }

        public ModuleDefinition Module => ReturnType.Module;

        public IMetadataScope Scope => signature.ReturnType.Scope;

        public string FullName
        {
            get
            {
                var signature = new StringBuilder();
                signature.Append(ReturnType.FullName);
                this.MethodSignatureFullName(signature);
                return signature.ToString();
            }
        }

        public bool HasThis
        {
            get => signature.HasThis;
            set => signature.HasThis = value;
        }

        public bool ExplicitThis
        {
            get => signature.ExplicitThis;
            set => signature.ExplicitThis = value;
        }

        public MethodCallingConvention CallingConvention
        {
            get => signature.CallingConvention;
            set => signature.CallingConvention = value;
        }

        public bool HasParameters => signature.HasParameters;

        public Collection<ParameterDefinition> Parameters => signature.Parameters;

        public TypeReference ReturnType
        {
            get => signature.MethodReturnType.ReturnType;
            set => signature.MethodReturnType.ReturnType = value;
        }

        public MethodReturnType MethodReturnType => signature.MethodReturnType;

        public MetadataToken MetadataToken
        {
            get => signature.token;
            set => signature.token = value;
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}