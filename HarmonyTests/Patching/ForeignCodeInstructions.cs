#if NET5_0_OR_GREATER
using HarmonyLib;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Loader;

namespace HarmonyLibTests.Patching
{
	[TestFixture]
	public class ForeignCodeInstructions : TestLogger
	{
		[Test]
		public void Conversion_preserves_the_requested_instruction_load_context()
		{
			var context = new AssemblyLoadContext("foreign-instructions", isCollectible: true);
			try
			{
				var assembly = context.LoadFromAssemblyPath(typeof(CodeInstruction).Assembly.Location);
				var elementType = assembly.GetType(typeof(CodeInstruction).FullName, true);
				Assert.AreNotSame(typeof(CodeInstruction), elementType);
				var requestedType = typeof(IEnumerable<>).MakeGenericType(elementType);
				var converted = CodeTranspiler.ConvertInstructionsAndUnassignedValues(requestedType,
					new[] { new CodeInstruction(OpCodes.Ldc_I4, 42) }, out _);
				Assert.IsTrue(requestedType.IsInstanceOfType(converted));
				var instruction = converted.Cast<object>().Single();
				Assert.AreSame(elementType, instruction.GetType());
				Assert.AreEqual(42, elementType.GetField(nameof(CodeInstruction.operand)).GetValue(instruction));
			}
			finally { context.Unload(); }
		}
	}
}
#endif
