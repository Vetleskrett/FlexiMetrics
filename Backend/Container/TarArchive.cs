using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;
using System.Text;

namespace Container;

public record TextFile(string FileName, string Contents);

public static class TarArchive
{
    public static Stream CreateAll(params TextFile[] files)
    {
        var outputStream = new MemoryStream();
        using (var writer = WriterFactory.Open(outputStream, ArchiveType.Tar, CompressionType.None))
        {
            foreach (var file in files)
            {
                var bytes = Encoding.UTF8.GetBytes(file.Contents);
                var stream = new MemoryStream(bytes);
                writer.Write(file.FileName, stream, DateTime.UtcNow);
            }
        }
        outputStream.Position = 0;
        return outputStream;
    }

    public static Stream Create(Stream fileStream, string fileName)
    {
        var outputStream = new MemoryStream();
        using (var writer = WriterFactory.Open(outputStream, ArchiveType.Tar, CompressionType.None))
        {
            writer.Write(fileName, fileStream, DateTime.Now);
        }
        outputStream.Position = 0;
        return outputStream;
    }

    public static List<Stream> ExtractAll(Stream tarStream)
    {
        List<Stream> outputStreams = [];
        using (var reader = ReaderFactory.Open(tarStream))
        {
            while (reader.MoveToNextEntry())
            {
                var outputStream = new MemoryStream();
                reader.WriteEntryTo(outputStream);
                outputStream.Position = 0;
                outputStreams.Add(outputStream);
            }
        }
        return outputStreams;
    }

    public static Stream Extract(Stream tarStream)
    {
        var outputStream = new MemoryStream();
        using (var reader = ReaderFactory.Open(tarStream))
        {
            reader.MoveToNextEntry();
            reader.WriteEntryTo(outputStream);
        }
        outputStream.Position = 0;
        return outputStream;
    }
}
