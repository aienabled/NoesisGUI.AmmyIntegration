namespace IntegrationSampleDX11.SharpDX
{
	#region

	using System;
	using System.Windows.Forms;
	using global::SharpDX;
	using global::SharpDX.Direct3D;
	using global::SharpDX.Direct3D11;
	using global::SharpDX.DXGI;
	using Device = global::SharpDX.Direct3D11.Device;
	using Resource = global::SharpDX.Direct3D11.Resource;

	#endregion

	/// <summary>
	/// Root class for Direct3D10(.1) Demo App
	/// </summary>
	public class Direct3D11DemoApp : DemoApp
	{
		private Texture2D backBuffer;

		private RenderTargetView backBufferView;

		private Device device;

		private SwapChain swapChain;

		/// <summary>
		/// Returns the backbuffer used by the SwapChain
		/// </summary>
		public Texture2D BackBuffer => this.backBuffer;

		/// <summary>
		/// Returns the render target view on the backbuffer used by the SwapChain.
		/// </summary>
		public RenderTargetView BackBufferView => this.backBufferView;

		/// <summary>
		/// Returns the device
		/// </summary>
		public Device Device => this.device;

		public SwapChain SwapChain => this.swapChain;

		protected override void BeginDraw()
		{
			base.BeginDraw();
			this.Device.ImmediateContext.Rasterizer.SetViewport(new Viewport(0, 0, this.Config.Width, this.Config.Height));
			this.Device.ImmediateContext.OutputMerger.SetTargets(this.backBufferView);
		}

		protected override void EndDraw()
		{
			this.swapChain.Present(this.Config.WaitVerticalBlanking ? 1 : 0, PresentFlags.None);
		}

		protected override void Initialize(DemoConfiguration demoConfiguration)
		{
			// SwapChain description
			var desc = new SwapChainDescription()
			{
				BufferCount = 1,
				ModeDescription = new ModeDescription(
					demoConfiguration.Width,
					demoConfiguration.Height,
					new Rational(60, 1),
					Format.R8G8B8A8_UNorm),
				IsWindowed = true,
				OutputHandle = this.DisplayHandle,
				SampleDescription = new SampleDescription(1, 0),
				SwapEffect = SwapEffect.Discard,
				Usage = Usage.RenderTargetOutput
			};

			// Create Device and SwapChain
			FeatureLevel featureLevel = new Device(DriverType.Hardware).FeatureLevel;
			if (featureLevel > FeatureLevel.Level_11_0)
			{
				featureLevel = FeatureLevel.Level_11_0;
			}

			Device.CreateWithSwapChain(
				DriverType.Hardware,
				DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug,
				new[] { featureLevel },
				desc,
				out this.device,
				out this.swapChain);

			// Ignore all windows events
			var factory = this.swapChain.GetParent<Factory>();
			factory.MakeWindowAssociation(this.DisplayHandle, WindowAssociationFlags.IgnoreAll);

			this.CreateBackBuffer();
		}

		protected override void HandleResize(object sender, EventArgs e)
		{
			if (this.form.WindowState == FormWindowState.Minimized)
			{
				return;
			}

			var size = this.RenderingSize;

			if (this.demoConfiguration.Width == size.Width
			    && this.demoConfiguration.Height == size.Height)
			{
				return;
			}

			device.ImmediateContext.ClearState();
			this.DestroyBackBuffer();

			var desc = this.SwapChain.Description;
			this.SwapChain.ResizeBuffers(desc.BufferCount, size.Width, size.Height, Format.Unknown, SwapChainFlags.AllowModeSwitch);

			demoConfiguration.Width = size.Width;
			demoConfiguration.Height = size.Height;

			this.CreateBackBuffer();
		}

		private void DestroyBackBuffer()
		{
			this.backBufferView.Dispose();
			this.backBuffer.Dispose();
		}

		private void CreateBackBuffer()
		{
			this.backBuffer = Resource.FromSwapChain<Texture2D>(this.swapChain, 0);
			this.backBufferView = new RenderTargetView(this.device, this.backBuffer);
		}
	}
}