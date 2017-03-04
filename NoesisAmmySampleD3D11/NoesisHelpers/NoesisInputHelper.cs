namespace IntegrationSampleDX11.NoesisHelpers
{
	#region

    using System;
    using System.Windows.Forms;
    using Noesis;

    #endregion

	internal static class NoesisInputHelper
	{
		public static Key GetNoesisKey(int key, Keys specialKey)
		{
			switch (specialKey)
			{
				case Keys.F1:
					return Key.F1;
				case Keys.F2:
					return Key.F2;
				case Keys.F3:
					return Key.F3;
				case Keys.F4:
					return Key.F4;
				case Keys.F5:
					return Key.F5;
				case Keys.F6:
					return Key.F6;
				case Keys.F7:
					return Key.F7;
				case Keys.F8:
					return Key.F8;
				case Keys.F9:
					return Key.F9;
				case Keys.F10:
					return Key.F10;
				case Keys.F11:
					return Key.F11;
				case Keys.F12:
					return Key.F12;
				case Keys.PageUp:
					return Key.Prior;
				case Keys.PageDown:
					return Key.Next;
				case Keys.Home:
					return Key.Home;
				case Keys.End:
					return Key.End;
				case Keys.Insert:
					return Key.Insert;
				case Keys.Left:
					return Key.Left;
				case Keys.Right:
					return Key.Right;
				case Keys.Up:
					return Key.Up;
				case Keys.Down:
					return Key.Down;
				default:
					break;
			}

			switch (key)
			{
				case '\n':
				case '\r':
					return Key.Return;
				case '\t':
					return Key.Tab;
				case '\b':
					return Key.Back;
				case ' ':
					return Key.Space;
				case 'A':
				case (char)0x01:
					return Key.A;
				case 'C':
				case (char)0x03:
					return Key.C;
				case 'V':
				case (char)0x16:
					return Key.V;
				case 'X':
				case (char)0x18:
					return Key.X;
				case (char)46:
				case (char)0x7F:
					return Key.Delete;
				default:
					break;
			}

			return Key.None;
		}

		public static MouseButton GetNoesisMouseButton(MouseButtons button)
		{
			switch (button)
			{
				case MouseButtons.Left:
					return MouseButton.Left;
				case MouseButtons.Right:
					return MouseButton.Right;
				case MouseButtons.Middle:
					return MouseButton.Middle;
				case MouseButtons.XButton1:
					return MouseButton.XButton1;
				case MouseButtons.XButton2:
					return MouseButton.XButton2;
				default:
					throw new ArgumentOutOfRangeException(nameof(button), button, null);
			}
		}
	}
}