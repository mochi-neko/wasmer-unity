namespace Mochineko.WasmerUnity.Wasm
{
    public static class ExternalInstanceExtension
    {
        public static ExternalInstance AsExternal(this FunctionInstance functionInstance)
            => ExternalInstance.FromFunction(functionInstance);
    }
}