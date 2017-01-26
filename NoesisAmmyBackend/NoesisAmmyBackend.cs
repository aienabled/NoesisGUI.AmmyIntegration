namespace NoesisAmmyBackend
{
	#region

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ammy.BackendCommon;

	#endregion

	public class NoesisAmmyBackend : IAmmyBackend
	{
		public IAmmyCompiler Compiler { get; set; }
		public string[] DefaultNamespaces => new[] { "Noesis" };

		public bool NeedRuntimeUpdate => false;

		public TypeNames TypeNames => NoesisTypeNames.Instance;

		public Type[] ProvideTypes()
		{
			return AppDomain.CurrentDomain
							.GetAssemblies()
							.SelectMany(a => a.GetTypes())
							.ToArray();
		}

		public void TriggerCompilation(IReadOnlyList<string> sourceFilePaths)
		{
			var sources = sourceFilePaths.Select(
											 path => (Source)new FileSource(path))
										 .ToArray();

			var compilationRequest = new CompilationRequest(sources);

			Compiler.Compile(compilationRequest);
		}
	}
}