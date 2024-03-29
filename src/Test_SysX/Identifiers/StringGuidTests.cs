﻿namespace Test_SysX.Identifiers;
using Assert = Xunit.Assert;

public class StringGuidTests
{
	[Fact]
	public void Should_Order_Sequentially_Over_Short_Time()
	{
		var time = new DateTime(2020, 1, 1);

		var options = IdentifierOptions.Default with
		{
			GetNow = () =>
			{
				time = time.AddMilliseconds(1);
				return time;
			}
		};

		var guids = Enumerable.Range(0, 1000)
			.Select(x => StringGuid.NewSequentialGuid(options))
			.ToArray();

		var ordered = guids.OrderBy(x => x).ToArray();

		for (var i = 0; i < guids.Length; i++)
			Assert.Equal(guids[i], ordered[i]);
	}

	[Fact]
	public void Should_Order_Sequentially_Over_Long_Time()
	{
		var time = new DateTime(2020, 1, 1);

		var options = IdentifierOptions.Default with
		{
			GetNow = () =>
			{
				time = time.AddDays(1);
				return time;
			}
		};

		var guids = Enumerable.Range(0, 1000)
			.Select(x => StringGuid.NewSequentialGuid(options))
			.ToArray();

		var ordered = guids.OrderBy(x => x).ToArray();

		for (var i = 0; i < guids.Length; i++)
			Assert.Equal(guids[i], ordered[i]);
	}

	[Fact]
	public void Should_Generate_Unique_Values()
	{
		var guids = Enumerable.Range(0, 1000)
			.Select(_ => StringGuid.NewSequentialGuid())
			.ToArray();

		var uniqueGuids = guids.Distinct().ToArray();

		Assert.Equal(guids.Length, uniqueGuids.Length);
	}
}