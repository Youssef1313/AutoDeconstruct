﻿using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace AutoDeconstruct.Tests;

public static class AutoDeconstructGeneratorInstanceTests
{
	[Test]
	public static async Task GenerateWhenNoTargetsExistAsync()
	{
		var code =
			"""
			using System;

			namespace TestSpace { }
			""";

		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithPropertyNameThatIsAKeywordAsync()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Namespace { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @namespace)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@namespace = @self.Namespace;
					}
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithReferenceTypeAndOneProperty()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Id { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@id = @self.Id;
					}
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithReferenceTypeAndMultipleProperties()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Name { get; set; }
					public Guid Id { get; set; }
					public int Value { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @name, out global::System.Guid @id, out int @value)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						(@name, @id, @value) =
							(@self.Name, @self.Id, @self.Value);
					}
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithValueTypeAndOneProperty()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public struct Test
				{ 
					public string? Id { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @id)
					{
						@id = @self.Id;
					}
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithValueTypeAndMultipleProperties()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public struct Test
				{ 
					public string? Name { get; set; }
					public Guid Id { get; set; }
					public int Value { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @name, out global::System.Guid @id, out int @value)
					{
						(@name, @id, @value) =
							(@self.Name, @self.Id, @self.Value);
					}
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithRecord()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public record Test()
				{
					public string? Id { get; init; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@id = @self.Id;
					}
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithRecordThatHasDeconstruct()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public record Test(string Id);
			}
			""";

		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithNoAccesibleProperties()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public struct Test
				{ 
					private string? Name { get; set; }
					private Guid Id { get; set; }
					private int Value { get; set; }
				}
			}
			""";

		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithNoDeconstructMatch()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Name { get; set; }
					public Guid Id { get; set; }
					public int Value { get; set; }

					public void Deconstruct(out int value, out string? name) =>
						(value, name) = (this.Value, this.Name);
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @name, out global::System.Guid @id, out int @value)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						(@name, @id, @value) =
							(@self.Name, @self.Id, @self.Value);
					}
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithDeconstructNotReturningVoid()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Name { get; set; }
					public Guid Id { get; set; }
					public int Value { get; set; }

					public int Deconstruct(out int value, out string? name, out Guid id)
					{
						(value, name, id) = (this.Value, this.Name, this.Id);
						return 3;
					}
				}
			}
			""";

		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithExistingDeconstructButWithNonOutParameters()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Name { get; set; }
					public Guid Id { get; set; }
					public int Value { get; set; }

					public void Deconstruct(out int value, out string? name, int[] values) =>
						(value, name) = (this.Value, this.Name);
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @name, out global::System.Guid @id, out int @value)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						(@name, @id, @value) =
							(@self.Name, @self.Id, @self.Value);
					}
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithMatchingDeconstruct()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Name { get; set; }
					public Guid Id { get; set; }
					public int Value { get; set; }

					public void Deconstruct(out int value, out string? name, out Guid id) =>
						(value, name, id) = (this.Value, this.Name, this.Id);
				}
			}
			""";

		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWhenPartialDefinitionsExist()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public partial class Test
				{ 
					public string? Name { get; set; }
				}

				public partial class Test
				{ 
					public Guid Id { get; set; }
					public int Value { get; set; }

					public void Deconstruct(out int value, out string? name, int[] values) =>
						(value, name) = (this.Value, this.Name);
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @name, out global::System.Guid @id, out int @value)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						(@name, @id, @value) =
							(@self.Name, @self.Id, @self.Value);
					}
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWhenPropertiesExistInInheritanceHierarchy()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class BaseTest
				{ 
					public int Id { get; set; }
				}

				public class Test
					: BaseTest
				{ 
					public string? Name { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class BaseTestExtensions
				{
					public static void Deconstruct(this global::TestSpace.BaseTest @self, out int @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@id = @self.Id;
					}
				}
			}
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @name, out int @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						(@name, @id) =
							(@self.Name, @self.Id);
					}
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWhenSelfPropertyExists()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Self { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self1, out string? @self)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self1);
						@self = @self1.Self;
					}
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}
}