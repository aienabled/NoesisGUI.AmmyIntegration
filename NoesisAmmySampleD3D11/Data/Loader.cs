namespace SampleData
{
	#region

	using Noesis;

	#endregion

	public class Loader : UserControl
	{
		public Loader()
		{
			GUI.LoadComponent(this, "Loader.xaml");
		}
	}
}