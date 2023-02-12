# wasmer-unity

Aims to provide a C# interface of Wasmer, one of WebAssembly runtimes, integrating as a native plugin for Unity.

## Motivation

There are no suitable libraries to use Wasm on Unity/C#.

- [Wasmtime](https://github.com/bytecodealliance/wasmtime) / [wasmtime-dotnet](https://github.com/bytecodealliance/wasmtime-dotnet)
  - Wasmtime does not support iOS/Android platforms.
  - https://github.com/mochi-neko/wasmtime-dotnet-unity
- [Wasmer](https://github.com/wasmerio/wasmer) / [WasmerSharp](https://github.com/migueldeicaza/WasmerSharp)
  - WasmerSharp embeds old version of Wasmter neaby 0.57.0 (2019).
- [cs-wasm](https://github.com/jonathanvdc/cs-wasm)
  - Original implementation for Wasm by C#, without native plugins.

The following are the best features I think:

- uses a famous Wasm runtime as native plugin,
- maintained to stay close to the latest version of the original,
- supports major platforms: Windows, macOS(Intel/AppleSilicon), Linux, Android(ARM64/ARMv7) and iOS including AoT compilation. 

## Milestones

1. Implement minimal Wasm APIs for hello world.
2. Implement pure Wasm APIs with samples.
3. Make demo on Unity.
4. Prepair Wasmer build environment for Android/iOS.
5. Implement Wasmer APIs.

## Notice

[3rd party notices](https://github.com/mochi-neko/wasmer-unity/tree/main/NOTICE)

## Lisence

[MIT License](https://github.com/mochi-neko/wasmer-unity/blob/main/LICENSE)
