namespace Mochineko.WasmerUnity.Wasm
{
    public static class ExternalInstanceExtension
    {
        public static ExternalInstance AsExternal(this FunctionInstance instance)
            => ExternalInstance.FromFunction(instance);
        
        public static ExternalInstance AsExternal(this GlobalInstance instance)
            => ExternalInstance.FromGlobal(instance);
    }
}