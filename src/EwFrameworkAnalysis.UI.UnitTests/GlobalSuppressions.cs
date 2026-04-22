// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test methods use underscores for readability", Scope = "namespaceanddescendants", Target = "~N:EwFrameworkAnalysis.UI.UnitTests")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Test methods don't require Async suffix", Scope = "namespaceanddescendants", Target = "~N:EwFrameworkAnalysis.UI.UnitTests")]
