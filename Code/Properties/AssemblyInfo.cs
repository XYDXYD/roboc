using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

[assembly: Obfuscation(Feature = "Apply to type *: apply to member * when property and has_attribute('Svelto.IoC.InjectAttribute'): composition pruning")]
[assembly: Obfuscation(Feature = "encrypt symbol names with password B7THQcAecF7dtQaF", Exclude = false)]
[assembly: Obfuscation(Feature = "Apply to type * when inherits('UnityEngine.MonoBehaviour'): renaming", Exclude = true, ApplyToMembers = true)]
[assembly: Obfuscation(Feature = "Apply to type UWK*: renaming", Exclude = true, ApplyToMembers = true)]
[assembly: Obfuscation(Feature = "Apply to type StatsMonitor.*: all", Exclude = true, ApplyToMembers = true)]
[assembly: Obfuscation(Feature = "Apply to type * when inherits ('System.Exception'): renaming", Exclude = true, ApplyToMembers = true)]
[assembly: Obfuscation(Feature = "Apply to type * when inherits ('UnityEngine.ScriptableObject'): renaming", Exclude = true, ApplyToMembers = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion("0.0.0.0")]
[module: UnverifiableCode]
