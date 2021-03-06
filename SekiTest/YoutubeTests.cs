﻿using Xunit;

namespace SekiTest
{
    public class YoutubeTests
    {
        [Fact]
        public void TimeStampTestRegularTime()
        {
            Assert.Equal("12:05:05", SekiDiscord.Commands.YoutubeUseful.ParseDuration("PT12H5M5S"));
        }

        [Fact]
        public void TimeStampTestMidnight()
        {
            Assert.Equal("0:00", SekiDiscord.Commands.YoutubeUseful.ParseDuration("PT00H0M0S"));
        }
    }
}