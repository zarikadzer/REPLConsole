# REPL Service
C# REPL Engine and SyntaxAnalyzer features. Projects goal is to create the remote REPL console for debugging purposes.

![logo](https://github.com/zarikadzer/REPLConsole/blob/master/REPL_service_3.png)

## Debug using the REPLConsole
1. Build and Run the *REPLConsole* application in *Debug* mode. 
2. Open the *Output* window at you Visual Studio IDE.
3. Using the REPLConsole, create a new instance of the *A:IDebuggable* type and bind it with the static injector:
```c#
using REPLConsole;
var b = new A { Value = "New message" };
Program.aInjector.Bind(typeof(A), new ClassDebugger(b));
```
4. Look at the *Output* and ensure that object was successfully injected.
5. Now you are able to inject your own types. e.g.:
```c#
public class B : A {
 public B() {
  Value = "New value " + System.Guid.NewGuid().ToString();
  Value2 = " Additional debug info vs code.";
 }
 public override DebugInfo GetDebugInfo() {
  //any custom logic
  return new DebugInfo(Value + Value2, 999);
 }
}
```
6. Bind the  instance of the custom type:
```c#
Program.aInjector.Bind(typeof(A), new ClassDebugger(new B()));
```
7. Open the *Output* and see the debug info.
8. After the debug session you could change the *ContainerMode*:
```c#
Program.aInjector.ContainerMode = REPL.DI.ContainerMode.ReturnsFirst;
```
## Unsolved issues
1. Memory Usage: Minimize a garbage. 
- Collectable assemblies
- CSharpCodeProvider().CreateCompiler().CompileAssemblyFromSource
- Using the MSIL Stream
- Others...
2. Perf/Load testing.
