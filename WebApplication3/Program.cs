using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace WebApplication3
{
    public class Program
    {
        public class Md5VsSha256
        {
            private const int N = 10000;
            private readonly byte[] data;

            private readonly SHA256 sha256 = SHA256.Create();
            private readonly MD5 md5 = MD5.Create();

            public Md5VsSha256()
            {
                data = new byte[N];
                new Random(42).NextBytes(data);
            }

            [Benchmark]
            public byte[] Sha256() => sha256.ComputeHash(data);

            [Benchmark]
            public byte[] Md5() => md5.ComputeHash(data);
        }


        private class Test
        {
            public int Id;
            public int Secret;

            public Test() { }
        }

        public class TestSpeedOfDictionary {
            private readonly List<Test> _test;
            private Random _rng;
            private List<int> _target;
            private Dictionary<int, Test> _dict;

            public TestSpeedOfDictionary()
            {
                _rng = new Random();
                _target = Enumerable.Range(1, 5).Select(_ => _rng.Next(1, 100)).ToList();
                _test = Enumerable.Range(1, 100).Select(n => new Test 
                { 
                    Id = n,
                    Secret = 100 - n
                }).ToList();
            }

            [Benchmark]
            public int RunDictonary()
            {
                var intToFind = _rng.Next(_target.Count - 1);
                _dict ??= _test.ToDictionary(t => t.Id, t => t);

                if (_dict.ContainsKey(intToFind))
                {
                    return _dict[intToFind].Secret;
                }

                return -1;
            }

            [Benchmark]
            public int RunList()
            {
                var intToFind = _rng.Next(_target.Count - 1);
                return _test.Find(x => x.Id == intToFind)?.Secret ?? -1;
            }
        }

        public static void Main(string[] args)
        {
             var config = new ManualConfig()
                            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
                            .AddValidator(JitOptimizationsValidator.DontFailOnError)
                            .AddLogger(ConsoleLogger.Default)
                            .AddColumnProvider(DefaultColumnProviders.Instance);
            BenchmarkRunner.Run<TestSpeedOfDictionary>(config);        
        }
    }
}
