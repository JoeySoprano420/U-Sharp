# U# — The U-Sharp Language

U# (U-Sharp) is a modern, expressive programming language runtime written in C#. It provides a rich set of standard libraries covering the full spectrum of everyday programming needs.

---

## Project Structure

```
U-Sharp/
├── USharp.slnx                      # Solution file
├── src/
│   ├── USharp.Core/                 # Core runtime: types, exceptions, Option/Result
│   ├── USharp.Collections/          # List, Map, Stack, Queue, Set
│   ├── USharp.IO/                   # File, Directory, Console I/O
│   ├── USharp.Math/                 # Math functions, stats, random
│   ├── USharp.Text/                 # String utilities, StringBuilder, Regex patterns
│   ├── USharp.Net/                  # HTTP client, TCP sockets
│   ├── USharp.Concurrency/          # Async helpers, Channel, Lock
│   ├── USharp.Serialization/        # JSON and binary serialization
│   └── USharp.Diagnostics/          # Logging, profiling
└── tests/
    ├── USharp.Core.Tests/
    ├── USharp.Collections.Tests/
    ├── USharp.IO.Tests/
    ├── USharp.Math.Tests/
    └── USharp.Text.Tests/
```

---

## Libraries

### `USharp.Core`
Fundamental building blocks of the U# type system and runtime.

| Class / Type | Description |
|---|---|
| `USharpType` | Represents a runtime type (name, namespace, base type, interfaces) |
| `USharpRuntime` | Manages type registration and resolution |
| `USharpObject` | Abstract base for all U# objects |
| `Option<T>` | Represents a value that may or may not be present (`Some` / `None`) |
| `Result<TValue, TError>` | Represents success (`Ok`) or failure (`Err`) |
| `USharpRange` | Immutable integer range with enumeration and containment checks |
| `WellKnownTypes` | Pre-built type descriptors for `Int`, `Long`, `Float`, `Double`, `Bool`, `String`, `Void`, `Any` |
| `USharpException` and friends | Hierarchy of runtime exceptions (type, null, index, argument) |

### `USharp.Collections`
Generic, functional-style collections.

| Class | Description |
|---|---|
| `USharpList<T>` | Resizable list with `Filter`, `Map`, `Reduce`, `Slice` |
| `USharpMap<TKey, TValue>` | Key-value map with `GetOrAdd`, `Filter`, `Merge` |
| `USharpStack<T>` | LIFO stack |
| `USharpQueue<T>` | FIFO queue |
| `USharpSet<T>` | Unique-element set with `Union`, `Intersect`, `Except` |

### `USharp.IO`
File and console I/O.

| Class | Description |
|---|---|
| `USharpFile` | Read/write text, lines, bytes; copy, delete, exists |
| `USharpDirectory` | Create, delete, list files and sub-directories |
| `USharpConsole` | Console write/read, color support |

### `USharp.Math`
Mathematics and statistics.

| Class | Description |
|---|---|
| `USharpMath` | `Sqrt`, `Pow`, `Log`, trig functions, `Gcd`, `Lcm`, `IsPrime`, `Factorial`, `Lerp` |
| `USharpStats` | `Mean`, `Median`, `Variance`, `StdDev`, `Sum`, `Range` |
| `USharpRandom` | Seeded random numbers, range values, shuffle |

### `USharp.Text`
String and pattern utilities.

| Class | Description |
|---|---|
| `USharpString` | `Reverse`, `Repeat`, `Truncate`, `ToTitleCase`, `ToCamelCase`, `ToSnakeCase`, `PadCenter`, `SplitLines` |
| `USharpStringBuilder` | Fluent mutable string builder |
| `USharpPattern` | Compiled regular expressions; built-in `Email()`, `Url()`, `Integer()`, `Decimal()` patterns |

### `USharp.Net`
Networking.

| Class | Description |
|---|---|
| `USharpHttpClient` | Fluent HTTP client — GET, POST, DELETE with timeout and header support |
| `USharpHttpResponse` | Typed HTTP response wrapper |
| `USharpTcpClient` | Async TCP client for raw socket communication |

### `USharp.Concurrency`
Async and threading.

| Class | Description |
|---|---|
| `USharpAsync` | `RunAsync`, `WhenAll`, `WhenAny`, `RetryAsync` (exponential back-off), `WithTimeout` |
| `USharpChannel<T>` | Thread-safe producer/consumer channel (bounded or unbounded) |
| `USharpLock` | Async-compatible mutual exclusion lock |

### `USharp.Serialization`
Data serialization.

| Class | Description |
|---|---|
| `USharpJson` | JSON serialize/deserialize, `Clone`, `IsValidJson` (via `System.Text.Json`) |
| `USharpBinary` | Byte-array serialization, Base64 encode/decode |

### `USharp.Diagnostics`
Logging and profiling.

| Class | Description |
|---|---|
| `USharpLogger` | Structured logger with levels (Debug/Info/Warning/Error/Fatal) and pluggable sinks |
| `USharpLoggerFactory` | Factory for named loggers |
| `USharpProfiler` | Measure sync/async operations; compute per-label min/max/avg statistics |
| `USharpTimer` | Scoped disposal-based timer |

---

## Building

```bash
dotnet build USharp.slnx -c Release
```

## Testing

```bash
dotnet test USharp.slnx -c Release
```

All library projects target **net9.0**.

---

## License

This project is open source. See the repository for details.