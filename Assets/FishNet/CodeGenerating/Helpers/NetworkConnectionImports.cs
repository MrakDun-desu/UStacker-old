using FishNet.Connection;
using MonoFN.Cecil;

namespace FishNet.CodeGenerating.Helping
{
    internal class NetworkConnectionImports : CodegenBase
    {
        public override bool ImportReferences()
        {
            var type = typeof(NetworkConnection);
            ImportReference(type);

            FullName = type.FullName;

            foreach (var pi in type.GetProperties())
                if (pi.Name == nameof(NetworkConnection.IsLocalClient))
                {
                    IsLocalClient_Get_MethodRef = ImportReference(pi.GetMethod);
                    break;
                }

            return true;
        }

        #region Reflection references.

        //Names.
        internal string FullName;
        public MethodReference IsLocalClient_Get_MethodRef;

        #endregion

        #region Const.

        internal const uint MAX_RPC_ALLOWANCE = ushort.MaxValue;
        internal const string AWAKE_METHOD_NAME = "Awake";

        internal const string DISABLE_LOGGING_TEXT =
            "This message may be disabled by setting the Logging field in your attribute to LoggingType.Off";

        #endregion
    }
}