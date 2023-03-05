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

1. [x] Implement minimal Wasm API for hello world.
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
- [x] ValueType
- [x] FunctionType
- [x] GlobalType
- [ ] TableType
- [ ] MemoryType
- [x] ExternalType
- [x] ImportType
- [x] ExportType
- [x] Trap
- [ ] Foreign
- [x] Module
- [x] ValueInstance
- [x] FunctionInstance
- [x] GlobalInstance
- [ ] TableInstance
- [ ] MemoryInstance
- [x] ExternalInstance
- [x] Instance 

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

### Examples

- Basics
  - [x] Hello World
  - [x] Instanting a module
  - [ ] Handling errors
  - [x] Interacting with memory
- Exports
  - [x] Exproted global
  - [ ] Exproted function
  - [x] Exproted memory
- Imports
  - [ ] Imported global
  - [ ] Imported function
- Externs
  - [ ] Table
  - [ ] Memory
- Tunables
  - [ ] Limit memory
- Engines
  - [ ] Engine
  - [ ] Headless engines
  - [ ] Cross-compilation
  - [ ] Features
- Compilers
  - [ ] Singlepass compiler
  - [ ] Cranelift compiler
  - [ ] LLVM compiler
- Integrations
  - [ ] WASI
  - [ ] WASI Pipes

## Notice

[3rd party notices](https://github.com/mochi-neko/wasmer-unity/tree/main/NOTICE)

## Lisence

[MIT License](https://github.com/mochi-neko/wasmer-unity/blob/main/LICENSE)
