using FishNet.CodeGenerating.Helping.Extension;
using MonoFN.Cecil;

namespace FishNet.CodeGenerating.Extension
{
    internal static class TypeReferenceExtensions
    {
	    /// <summary>
	    ///     Returns the fullname of a TypeReference without <>.
	    /// </summary>
	    /// <param name="tr"></param>
	    /// <returns></returns>
	    public static string GetFullnameWithoutBrackets(this TypeReference tr)
        {
            var str = tr.FullName;
            str = str.Replace("<", "");
            str = str.Replace(">", "");
            return str;
        }

	    /// <summary>
	    ///     Returns a method in the next base class.
	    /// </summary>
	    public static MethodReference GetMethodInBase(this TypeReference tr, CodegenSession session, string methodName)
        {
            return tr.CachedResolve(session).GetMethodInBase(session, methodName);
        }

	    /// <summary>
	    ///     Makes a GenericInstanceType.
	    /// </summary>
	    public static GenericInstanceType MakeGenericInstanceType(this TypeReference self)
        {
            var instance = new GenericInstanceType(self);
            foreach (var argument in self.GenericParameters)
                instance.GenericArguments.Add(argument);

            return instance;
        }
    }
}