using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace HttpServerLib
{
    public static class Helper
    {
        public static MethodInfo changeTypeMi;

        static Helper()
        {
            Type convertType = typeof(Convert);
            changeTypeMi = convertType.GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });
        }

        public static Func<object[], object> CreateMethodInvokerDelegate(MethodInfo method)
        {
            List<Type> prmList = method.GetParameters().Select(f => f.ParameterType).ToList();
            if (!method.IsStatic)
                prmList.Insert(0, method.DeclaringType);

            var dynamicMethod = new DynamicMethod("DynamicMethod", typeof(object), new Type[] { typeof(object[]) }, method.DeclaringType.GetTypeInfo().Module, skipVisibility: true);

            ILGenerator ilGen = dynamicMethod.GetILGenerator();

            for (int a = 0; a < prmList.Count; a++)
            {
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldc_I4, a);
                ilGen.Emit(OpCodes.Ldelem_Ref);
                ilGen.Emit(OpCodes.Ldtoken, prmList[a]);

                ilGen.Emit(OpCodes.Call, changeTypeMi);

                ilGen.Emit(OpCodes.Unbox_Any, prmList[a]);
            }

            if (method.IsStatic)
                ilGen.Emit(OpCodes.Call, method);
            else
                ilGen.Emit(OpCodes.Callvirt, method);

            if (method.ReturnType == typeof(void))
                ilGen.Emit(OpCodes.Ldnull);
            else
                ilGen.Emit(OpCodes.Box, method.ReturnType);

            ilGen.Emit(OpCodes.Ret);

            return (Func<object[], object>)dynamicMethod.CreateDelegate(typeof(Func<object[], object>));
        }

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}