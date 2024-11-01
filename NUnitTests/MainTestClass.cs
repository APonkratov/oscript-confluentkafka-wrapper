﻿using NUnit.Framework;
using oscriptcomponent;

// Используется NUnit 3.6

namespace NUnitTests
{
	[TestFixture]
	public class MainTestClass
	{

		private EngineHelpWrapper host;

		[OneTimeSetUp]
		public void Initialize()
		{
			host = new EngineHelpWrapper();
			host.StartEngine();
		}

		[Test]
		public void TestAsInternalObjects()
		{

		}

		[Test]
		public void TestAsExternalObjects()
		{
			host.RunTestScript("NUnitTests.Tests.external.os");
		}
	}
}
