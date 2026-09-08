using HarmonyLib;
using NUnit.Framework;

namespace HarmonyLibTests.Tools
{
	[TestFixture]
	public class Test_InlineSignature : TestLogger
	{
		[TestCase(false), TestCase(true)]
		public void Managed_reference_signatures_preserve_byref_parameters_and_returns(bool refReturn)
		{
			byte[] metadata = refReturn ? [0, 1, 0x10, 8, 0x10, 8] : [0, 1, 8, 0x10, 8];
			var signature = InlineSignatureParser.ImportCallSite(typeof(Test_InlineSignature).Module, metadata);
			Assert.AreEqual(new[] { typeof(int).MakeByRefType() }, signature.Parameters);
			Assert.AreEqual(refReturn ? typeof(int).MakeByRefType() : typeof(int), signature.ReturnType);
		}
	}
}
