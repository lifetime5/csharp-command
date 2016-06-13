﻿using NUnit.Framework;
using System;

namespace Haruair.Command.Tests
{
	[TestFixture ()]
	public class RequestResolverTest
	{
		RequestResolver resolver;

		[SetUp ()]
		public void Init()
		{
			resolver = new RequestResolver ();
		}

		[TearDown ()]
		public void Dispose()
		{
			resolver = null;
		}

		[Test ()]
		public void EmptyArgsTestCase ()
		{
			var args = new string[] { };
			var request = resolver.Resolve (args);

			Assert.AreEqual (null, request.Command);
			Assert.AreEqual (null, request.Method);
		}

		[Test ()]
		public void CommandTestCase ()
		{
			var args = new string[] { "hello" };
			var request = resolver.Resolve (args);

			Assert.AreEqual ("hello", request.Command);
			Assert.AreEqual (null, request.Method);
		}

		[Test ()]
		public void CommandAndMethodTestCase ()
		{
			var args = new string[] { "hello", "world" };
			var request = resolver.Resolve (args);

			Assert.AreEqual ("hello", request.Command);
			Assert.AreEqual ("world", request.Method);
		}
	}
}
