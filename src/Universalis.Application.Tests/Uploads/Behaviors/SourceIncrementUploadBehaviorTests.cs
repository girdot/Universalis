﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Universalis.Application.Tests.Mocks.DbAccess.Uploads;
using Universalis.Application.Uploads.Behaviors;
using Universalis.Application.Uploads.Schema;
using Universalis.DbAccess.Queries.Uploads;
using Universalis.Entities.Uploads;
using Xunit;

namespace Universalis.Application.Tests.Uploads.Behaviors
{
    public class SourceIncrementUploadBehaviorTests
    {
        public class PlayerContentUploadBehaviorTests
        {
            [Fact]
            public async Task Behavior_Succeeds()
            {
                var dbAccess = new MockTrustedSourceDbAccess();
                var behavior = new SourceIncrementUploadBehavior(dbAccess);

                const string key = "blah";
                string keyHash;
                using (var sha256 = SHA256.Create())
                {
                    await using var keyStream = new MemoryStream(Encoding.UTF8.GetBytes(key));
                    keyHash = Util.BytesToString(await sha256.ComputeHashAsync(keyStream));
                }

                var source = new TrustedSource
                {
                    ApiKeySha512 = keyHash,
                    Name = "test runner",
                    UploadCount = 0,
                };

                await dbAccess.Create(source);

                var upload = new UploadParameters();
                Assert.True(behavior.ShouldExecute(upload));

                var result = await behavior.Execute(source, upload);
                Assert.Null(result);

                var data = await dbAccess.Retrieve(new TrustedSourceQuery
                {
                    ApiKeySha512 = keyHash,
                });

                Assert.NotNull(data);
                Assert.Equal(1U, data.UploadCount);
            }
        }
    }
}