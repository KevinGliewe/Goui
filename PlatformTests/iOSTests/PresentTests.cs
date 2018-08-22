using System;
using NUnit.Framework;
using Goui;

namespace Tests
{
	[TestFixture]
	public class PresentTests
	{
		[TestCase]
		public void Present ()
		{
			var b = new Button ("Click Me");
			UI.Publish ("/b", b);
			UI.Present ("/b");
		}
	}
}
