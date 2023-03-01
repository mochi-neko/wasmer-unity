using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerUnity.Wasm
{
    public static class FunctionInstanceExtension
    {
        public static void Call(this FunctionInstance instance)
            => instance.Call(Array.Empty<ValueInstance>());
        
        public static TResult Call<TResult>(this FunctionInstance instance)
            => instance.Call<TResult>(Array.Empty<ValueInstance>());

        public static void Call<T1>(this FunctionInstance instance, T1 p1)
            => instance.Call(new[]
            {
                ValueInstance.New(p1),
            });
        
        public static TResult Call<TResult, T1>(this FunctionInstance instance, T1 p1)
            => instance.Call<TResult>(new[]
            {
                ValueInstance.New(p1),
            });

        public static void Call<T1, T2>(this FunctionInstance instance, T1 p1, T2 p2)
            => instance.Call(new[]
            {
                ValueInstance.New(p1),
                ValueInstance.New(p2),
            });
        
        public static TResult Call<TResult, T1, T2>(this FunctionInstance instance, T1 p1, T2 p2)
            => instance.Call<TResult>(new[]
            {
                ValueInstance.New(p1),
                ValueInstance.New(p2),
            });

       
    }
}