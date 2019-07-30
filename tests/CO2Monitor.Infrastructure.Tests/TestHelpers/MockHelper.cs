using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq.Language.Flow;
using Moq;

namespace CO2Monitor.Infrastructure.Tests.TestHelpers {
	static class MockHelper {
		public static ISetup<ILogger<T>> MockLog<T>(this Mock<ILogger<T>> logger, LogLevel level) =>
			logger.Setup(x => x.Log(level, It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

		private static Expression<Action<ILogger<T>>> Verify<T>(LogLevel level) =>
			x => x.Log(level, 0, It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>());

		public static void Verify<T>(this Mock<ILogger<T>> mock, LogLevel level, Times times) => 
			mock.Verify(Verify<T>(level), times);
	}
}