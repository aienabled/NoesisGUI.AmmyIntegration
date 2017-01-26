namespace IntegrationSampleDX11
{
	#region

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Windows.Forms;
	using IntegrationSampleDX11.NoesisHelpers;
	using IntegrationSampleDX11.SharpDX;
	using Noesis;
	using NoesisAmmyBackend;
	using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
	using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
	using View = Noesis.View;

	#endregion

	public static class Program
	{
		private static readonly DeviceDX11StateHelper deviceState = new DeviceDX11StateHelper();

		private static D3D11AppWrapper dxWrapper;

		private static bool isAltDown;

		private static bool isCtrlDown;

		private static bool isRunning;

		private static bool isShiftDown;

		private static Renderer renderer;

		private static View view;

		private static FileSystemWatcher fileWatcher;

		private static bool isRequiredToReloadNoesisGUI;

		private static string ammyDataPath;

		private static string xamlDataPath;

		private static void LogError(string msg)
		{
			Console.WriteLine("ERROR!");
			Console.WriteLine("--------------------------------------------------------");
			Console.WriteLine(msg);
			Console.WriteLine("--------------------------------------------------------");
			Console.WriteLine("PRESS ANY KEY TO CONTINUE");
			Console.ReadKey();
		}

		private static void Main()
		{
			try
			{
				Run();
			}
			catch (Exception e)
			{
				LogError(e.Message);

				OnClose();
			}
		}

		private static void OnClose()
		{
			// Detach from Application events
			if (!isRunning)
			{
				return;
			}

			isRunning = false;
			dxWrapper.Close -= OnClose;
			dxWrapper.Tick -= OnTick;
			dxWrapper.PreRender -= OnPreRender;
			dxWrapper.PostRender -= OnPostRender;
			dxWrapper.Resize -= OnResize;
			dxWrapper.MouseMove -= OnMouseMove;
			dxWrapper.MouseDown -= OnMouseDown;
			dxWrapper.MouseUp -= OnMouseUp;
			dxWrapper.KeyDownEvent -= OnKeyDown;
			dxWrapper.KeyUpEvent -= OnKeyUp;
			dxWrapper.KeyPressEvent -= OnKeyPress;

			renderer?.Shutdown();
			view = null;

			try
			{
				GUI.Shutdown();
			}
			catch (Exception e)
			{
				LogError(e.Message);
			}
		}

		private static void OnKeyDown(KeyEventArgs keyEventArgs)
		{
			ProcessModifiers(keyEventArgs.Modifiers);

			var noesisKey = NoesisInputHelper.GetNoesisKey(keyEventArgs.KeyValue, keyEventArgs.KeyCode);
			if (noesisKey != Key.None)
			{
				view.KeyDown(noesisKey);
			}
		}

		private static void OnKeyPress(KeyPressEventArgs args)
		{
			view.Char(args.KeyChar);
		}

		private static void OnKeyUp(KeyEventArgs keyEventArgs)
		{
			var noesisKey = NoesisInputHelper.GetNoesisKey(keyEventArgs.KeyValue, keyEventArgs.KeyCode);
			if (noesisKey != Key.None)
			{
				view.KeyUp(noesisKey);
			}

			ProcessModifiers(keyEventArgs.Modifiers);
		}

		private static void OnMouseDown(MouseEventArgs args)
		{
			view.MouseDown(args.X, args.Y, NoesisInputHelper.GetNoesisMouseButton(args.Button));
		}

		private static void OnMouseMove(MouseEventArgs args)
		{
			view.MouseMove(args.X, args.Y);
		}

		private static void OnMouseUp(MouseEventArgs args)
		{
			view.MouseUp(args.X, args.Y, NoesisInputHelper.GetNoesisMouseButton(args.Button));
		}

		private static void OnPostRender()
		{
			deviceState.Save(dxWrapper.Device.ImmediateContext);
			try
			{
				renderer.Render();
			}
			finally
			{
				deviceState.Restore(dxWrapper.Device.ImmediateContext);
			}
		}

		private static void OnPreRender()
		{
			deviceState.Save(dxWrapper.Device.ImmediateContext);

			try
			{
				renderer.UpdateRenderTree();

				if (renderer.NeedsOffscreen())
				{
					renderer.RenderOffscreen();
				}
			}
			finally
			{
				deviceState.Restore(dxWrapper.Device.ImmediateContext);
			}
		}

		private static void OnResize(int width, int height)
		{
			view.SetSize(width, height);
		}

		private static void OnTick(double timeInSeconds)
		{
			if (isRequiredToReloadNoesisGUI)
			{
				ReloadNoesisGUIView();
			}

			view.Update(timeInSeconds);
		}

		private static void ProcessModifier(ref bool modifier, bool isDown, Key modifierKey)
		{
			if (!modifier)
			{
				if (isDown)
				{
					modifier = true;
					view.KeyDown(modifierKey);
				}
			}
			else
			{
				if (!isDown)
				{
					modifier = false;
					view.KeyUp(modifierKey);
				}
			}
		}

		private static void ProcessModifiers(Keys modifiers)
		{
			ProcessModifier(ref isShiftDown, (modifiers & Keys.Shift) != 0, Key.LeftShift);
			ProcessModifier(ref isCtrlDown, (modifiers & Keys.Control) != 0, Key.LeftCtrl);
			ProcessModifier(ref isAltDown, (modifiers & Keys.Alt) != 0, Key.LeftAlt);
		}

		private static void Run()
		{
			ammyDataPath =
				System.IO.Path.GetFullPath(
					System.IO.Path.Combine(
						Environment.CurrentDirectory,
						@"..\..\..\SampleWPFAmmy\Data"));

			xamlDataPath = System.IO.Path.Combine(
				System.IO.Path.GetRandomFileName(),
				System.IO.Path.DirectorySeparatorChar.ToString());

			// Prepare and run D3D11 wrapper
			var configuration = new DemoConfiguration("NoesisGUI Ammy Integration Sample", 800, 600);
			dxWrapper = new D3D11AppWrapper();
			dxWrapper.Run(
				configuration,
				contentLoadCallback:
				() =>
				{
					// Callback for initialization
					GUI.Init();

					GUI.SetResourceProvider(xamlDataPath);

					// Global theme
					{
						// we don't use any theme
						//var theme = (Noesis.ResourceDictionary)Noesis.GUI.LoadXaml("NoesisStyle.xaml");
						//Noesis.GUI.SetTheme(theme);
					}

					// Data loading
					ReloadNoesisGUIView();

					// Attach to Application events
					dxWrapper.Close += OnClose;
					dxWrapper.Tick += OnTick;
					dxWrapper.PreRender += OnPreRender;
					dxWrapper.PostRender += OnPostRender;
					dxWrapper.Resize += OnResize;
					dxWrapper.MouseMove += OnMouseMove;
					dxWrapper.MouseDown += OnMouseDown;
					dxWrapper.MouseUp += OnMouseUp;
					dxWrapper.KeyDownEvent += OnKeyDown;
					dxWrapper.KeyUpEvent += OnKeyUp;
					dxWrapper.KeyPressEvent += OnKeyPress;

					StartFileWatcher();

					isRunning = true;
				});
		}

		private static void StartFileWatcher()
		{
			fileWatcher = new FileSystemWatcher(ammyDataPath) { IncludeSubdirectories = true };

			fileWatcher.Renamed += (s, e) => isRequiredToReloadNoesisGUI = true;
			fileWatcher.Created += (s, e) => isRequiredToReloadNoesisGUI = true;
			fileWatcher.Deleted += (s, e) => isRequiredToReloadNoesisGUI = true;
			fileWatcher.Changed += (s, e) => isRequiredToReloadNoesisGUI = true;
			fileWatcher.Error += (s, e) => LogError("File watcher error: " + e.GetException());
			fileWatcher.EnableRaisingEvents = true;
		}

		private static void ReloadNoesisGUIView()
		{
			// create new ammy backed
			var ammyBackend = new NoesisAmmyBackend();
			// collect file paths for compilation
			var sourceFilePaths = CollectAmmyFilePaths(ammyDataPath);

			try
			{
				// compile .ammy->XAML
				ammyBackend.TriggerCompilation(ammyDataPath, sourceFilePaths, xamlDataPath);
			}
			catch (Exception ex)
			{
				LogError("Error during Ammy compilation: " + ex + Environment.NewLine + ex.StackTrace);
				return;
			}

			// destroy old view renderer
			renderer?.Shutdown();
			
			// create new view
			var loader = new SampleData.Loader();
			var content = loader;
			view = Noesis.GUI.CreateView(content);

			// init new view renderer
			var immediateContext = dxWrapper.Device.ImmediateContext;
			renderer = view.Renderer;
			renderer.InitD3D11(immediateContext.NativePointer, new Noesis.VGOptions());

			isRequiredToReloadNoesisGUI = false;

			OnResize(dxWrapper.Size.Width, dxWrapper.Size.Height);
		}

		private static IReadOnlyList<string> CollectAmmyFilePaths(string rootPath)
		{
			return Directory.GetFiles(rootPath, "*.ammy", SearchOption.AllDirectories);
		}
	}
}