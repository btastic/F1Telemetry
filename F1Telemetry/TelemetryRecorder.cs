using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using F1Telemetry.Models.Raw.F12018;
using MessagePack;

namespace F1Telemetry
{
    public class TelemetryRecorder
    {
        private readonly string _fileFolder;
        private CancellationTokenSource _cts;

        private ConcurrentBag<BinaryPacket> _packetsToProcess = new ConcurrentBag<BinaryPacket>();
        private ConcurrentQueue<BinaryPacket> _queue = new ConcurrentQueue<BinaryPacket>();

        public TelemetryRecorder(string fileFolder = null)
        {
            if (fileFolder == null)
            {
                fileFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }

            _fileFolder = fileFolder;
        }

        public bool Recording { get; private set; }

        public void RecordPacket(PacketHeader packet, byte[] bytes)
        {
            if (!Recording)
            {
                return;
            }

            if (packet.SessionUId == 0)
            {
                return;
            }

            _queue.Enqueue(new BinaryPacket(packet, bytes, TimeSpan.FromSeconds(packet.SessionTime)));
        }

        public void Start()
        {
            if (Recording)
            {
                return;
            }

            _cts = new CancellationTokenSource();

            Recording = true;

            Task.Factory.StartNew(RecordLoop, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            if (!Recording)
            {
                return;
            }

            Recording = false;
            _cts.Cancel();
        }

        private void RecordLoop()
        {
            CancellationToken cancellation = _cts.Token;
            while (!cancellation.WaitHandle.WaitOne(1))
            {
                while (_queue.TryDequeue(out BinaryPacket binaryPacket))
                {
                    _packetsToProcess.Add(binaryPacket);

                    if (_packetsToProcess.Count > 100)
                    {
                        var packet = _packetsToProcess.First().PacketHeader;

                        var targetFile = Path.Combine(_fileFolder, "telemetry", packet.SessionUId.ToString() + ".f1s");

                        var packets = new List<BinaryPacket>();

                        if (!Directory.Exists(Path.Combine(_fileFolder, "telemetry")))
                        {
                            Directory.CreateDirectory(Path.Combine(_fileFolder, "telemetry"));
                        }

                        if (File.Exists(targetFile))
                        {
                            var previousData = File.ReadAllBytes(targetFile);
                            packets = LZ4MessagePackSerializer.Deserialize<List<BinaryPacket>>(previousData);
                        }

                        packets.AddRange(_packetsToProcess.Reverse());

                        var bin = LZ4MessagePackSerializer.Serialize(packets);

                        File.WriteAllBytes(targetFile, bin);
                        _packetsToProcess = new ConcurrentBag<BinaryPacket>();
                    }
                }
            }
        }
    }
}
