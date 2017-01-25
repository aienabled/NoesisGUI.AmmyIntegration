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
		public event EventHandler<CompilationRequest> CompilationRequested;

		public string[] DefaultNamespaces => new[] { "System", "Noesis" };

		public bool NeedRuntimeUpdate => false;

		public TypeNames TypeNames => NoesisTypeNames.Instance;

		public void CompilationFinished(CompilationResult result)
		{
			throw new NotImplementedException();
		}

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
			this.CompilationRequested?.Invoke(this, compilationRequest);
		}
	}
}