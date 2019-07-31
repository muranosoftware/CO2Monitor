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
		SqLiteDeviceStateRepository CreateNewRepo() {
			IConfiguration config = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string,string> {
					{ SqLiteDeviceStateRepository.DataSourceConfigurationKey, $"SQLiteDeviceStateRepositoryTests{Guid.NewGuid()}.sqlite"
					}
				})
				.Build();
			var repo = new SqLiteDeviceStateRepository(config);
			repo.EnsureCreated();
			return repo;
		}
		
		[Test]
		public void Add() {
			SqLiteDeviceStateRepository repo = CreateNewRepo();

			DateTime now = DateTime.Now;

			DeviceStateMeasurement[] measurments = Enumerable.Range(0, 5).Select(
				x => new DeviceStateMeasurement {
					Time = now.AddSeconds(x),
					State = x.ToString()
				}).ToArray();

			measurments.ForEach(m => repo.Add(m));

			List<DeviceStateMeasurement> repoList = repo.List().ToList();

			repoList.Count.Should().Be(5);

			repoList.ForEach(x => measurments.Count(y => y.Time == x.Time).Should().Be(1));

			if (File.Exists(repo.DataSource)) {
				File.Delete(repo.DataSource);
			}
		}

		[Test]
		public void Where() {
			SqLiteDeviceStateRepository repo = CreateNewRepo();

			DateTime now = DateTime.Now;

			Enumerable.Range(0, 5).Select(
				x => new DeviceStateMeasurement {
					Time = now.AddSeconds(x),
					State = x.ToString()
				}).ToList().ForEach(x => repo.Add(x));

			DateTime to = now.AddSeconds(4);

			repo.List(x => x.Time > now && x.Time < to).ToArray().Length.Should().Be(3);
		}
	}
}