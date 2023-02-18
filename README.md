# wasmer-unity

Aims to provide a C# interface of [Wasmer](https://github.com/wasmerio/wasmer), one of WebAssembly runtimes, integrating as a native plugin for Unity.

## Motivation

There is no best library to use Wasm on Unity/C#.

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

1. [ ] Implement minimal Wasm API for hello world.
2. [ ] Implement native Wasm API with samples.
3. [ ] Make demo on Unity.
4. [ ] Implement headless in Wasmer API.
5. [ ] Prepair Wasmer build environment for Android/iOS.
6. [ ] Implement WASI API.
7. [ ] Implement features in Wasmer API.

## Implemntation

### Native Wasm API

- [x] Config
- [x] Engine
- [x] Store
- [x] ByteVector
- [ ] ValueType
- [ ] FunctionType
- [ ] GlobalType
- [ ] TableType
- [ ] MemoryType
- [ ] ExternalType
- [ ] ImportType
- [ ] ExportType
- [ ] Trap
- [ ] Foreign
- [ ] Module
- [ ] FunctionInstance
- [ ] GlobalInstance
- [ ] TableInstance
- [ ] MemoryInstance
- [ ] External
- [ ] ModuleInstance 

### Wasmer API

- [x] Wat2Wasm
- [ ] Headless 
- [ ] WASI
- CPU Features
  - [ ] Bulk Memory?
  - [ ] Memory64?
  - [ ] Module Linking?
  - [ ] Multi Memory?
  - [ ] Multi Value?
- Featues
  - [ ] Reference Type
  - [ ] SIMD
  - [ ] Tail Call
  - [ ] Thread
- [ ] Metering?
- [ ] Middleware?

## Notice

[3rd party notices](https://github.com/mochi-neko/wasmer-unity/tree/main/NOTICE)

## Lisence

[MIT License](https://github.com/mochi-neko/wasmer-unity/blob/main/LICENSE)
