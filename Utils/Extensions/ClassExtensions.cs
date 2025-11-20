using System;

namespace Replay.Utils
{
    public static class ClassExtensions
    {
        public static T GetRetValOrDefaultForStaticMethodName<T>(Type type, string methodName, object[] parameters = null, bool isPolymorphic = true)
        {
            //get method
            var staticMethod = type.GetMethod(methodName);
            if (isPolymorphic)
            {
                while (staticMethod == null)
                {
                    type = type.BaseType;
                    if(type == null)
                        break;
                    staticMethod = type.GetMethod(methodName);
                }
            }
            
            //get retVal
            T retVal = default;
            if (staticMethod != null)
            {
                if (parameters == null)
                    parameters = new object[] { };
                retVal = (T)staticMethod.Invoke(null, parameters);
            }
                
            return retVal;
        }

        public static R GetRetValOrDefaultForStaticMethodName<R, T>(string methodName, object[] parameters = null,
            bool isPolymorphic = true)  
            => GetRetValOrDefaultForStaticMethodName<R>(typeof(T), methodName, parameters, isPolymorphic);
        
       
        public static string GetStringOrNullForStaticMethodName<T>(this T _, string methodName, object[] parameters = null,
            bool isPolymorphic = true)  
            => GetRetValOrDefaultForStaticMethodName<string, T>(methodName, parameters, isPolymorphic);
        
        public static bool GetBoolOrFalseForStaticMethodName<T>(this T _, string methodName, object[] parameters = null,
            bool isPolymorphic = true) where T : class  
            => GetRetValOrDefaultForStaticMethodName<bool, T>(methodName, parameters, isPolymorphic);
        
        public static int GetIntOrZeroForStaticMethodName<T>(this T _, string methodName, object[] parameters = null,
            bool isPolymorphic = true) where T : class  
            => GetRetValOrDefaultForStaticMethodName<int, T>(methodName, parameters, isPolymorphic);
        
        public static float GetFloatOrZeroForStaticMethodName<T>(this T _, string methodName, object[] parameters = null,
            bool isPolymorphic = true) where T : class  
            => GetRetValOrDefaultForStaticMethodName<float, T>(methodName, parameters, isPolymorphic);
        
        public static double GetDoubleOrZeroForStaticMethodName<T>(this T _, string methodName, object[] parameters = null,
            bool isPolymorphic = true) where T : class  
            => GetRetValOrDefaultForStaticMethodName<double, T>(methodName, parameters, isPolymorphic);

        public static string GetClassName<T>() => typeof(T).Name;
        public static string GetClassName<T>(this T _) => GetClassName<T>();
        
    }
    

}

