using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace Container;

public static class TarArchive
{
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
