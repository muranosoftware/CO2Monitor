using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using MoreLinq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using CO2Monitor.Infrastructure.Data;
using CO2Monitor.Domain.Entities;

namespace CO2Monitor.Infrastructure.Tests.Data {
	[TestFixture]
	public class SqLiteDeviceStateRepositoryTests {


		private string _dBFile;
		private SqLiteDeviceStateRepository _repo;

		[SetUp]
		public void CreateNewRepo() {
			IConfiguration config = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string,string> {
					{ SqLiteDeviceStateRepository.DataSourceConfigurationKey, $"SQLiteDeviceStateRepositoryTests{Guid.NewGuid()}.sqlite"
					}
				})
				.Build();
			_repo = new SqLiteDeviceStateRepository(config);
			_repo.EnsureCreated();
		}

		[TearDown]
		public void DeleteDbFile() {
			if (File.Exists(_repo.DataSource)) {
				File.Delete(_repo.DataSource);
			}
		}
		
		[Test]
		public void AddFiveMeasurements() {
			DateTime now = DateTime.Now;

			DeviceStateMeasurement[] measurments = Enumerable.Range(0, 5).Select(
				x => new DeviceStateMeasurement {
					Time = now.AddSeconds(x),
					State = x.ToString()
				}).ToArray();

			measurments.ForEach(m => _repo.Add(m));

			List<DeviceStateMeasurement> repoList = _repo.List().ToList();

			repoList.Count.Should().Be(5);

			repoList.ForEach(x => measurments.Count(y => y.Time == x.Time).Should().Be(1));
		}

		[Test]
		public void CheckWherePredicate() {
			
			DateTime now = DateTime.Now;

			Enumerable.Range(0, 5).Select(
				x => new DeviceStateMeasurement {
					Time = now.AddSeconds(x),
					State = x.ToString()
				}).ToList().ForEach(x => _repo.Add(x));

			DateTime to = now.AddSeconds(4);

			_repo.List(x => x.Time > now && x.Time < to).ToArray().Length.Should().Be(3);
		}
	}
}