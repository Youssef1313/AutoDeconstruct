﻿using Microsoft.CodeAnalysis.Testing;

namespace AutoDeconstruct.Tests;

using GeneratorTest = CSharpIncrementalSourceGeneratorVerifier<AutoDeconstructGenerator>;

internal static class TestAssistants
{
	internal static async Task RunAsync(string code,
		IEnumerable<(Type, string, string)> generatedSources,
		IEnumerable<DiagnosticResult> expectedDiagnostics)
	{
		var test = new GeneratorTest.Test
		{
			ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
			TestState =
			{
				Sources = { code },
			},
		};

		foreach (var generatedSource in generatedSources)
		{
			test.TestState.GeneratedSources.Add(generatedSource);
		}

		test.TestState.AdditionalReferences.Add(typeof(AutoDeconstructGenerator).Assembly);
		test.TestState.ExpectedDiagnostics.AddRange(expectedDiagnostics);
		await test.RunAsync().ConfigureAwait(false);
	}
}