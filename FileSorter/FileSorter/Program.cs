using FileSorter;
using System.Text;

const string OUTPUT_FILE = "output.txt";
const string TMP_FILE = "temp.txt";
const string TMP_A_FILE = "tempA.txt";
string? inputFile;
bool repeatIsNeeded = true;

Console.Write("Print a path to a file from I'll be reading: ");
do
{
    inputFile = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(inputFile))
        Console.WriteLine("We have written nothing! Try again or print Q for exit.");
    else
    {
        if (inputFile.ToUpper() == "Q") return;
        var fileInfo = new FileInfo(inputFile);
        if (fileInfo.Exists)
            //If such file exists, leave the loop.
            repeatIsNeeded = false;
        else
            Console.WriteLine("This file doesn't exist. Try again or print Q for exit.");
    }
} while (repeatIsNeeded);

repeatIsNeeded = true;
var outputFileInfo = new FileInfo(OUTPUT_FILE);
if (outputFileInfo.Exists)
{
    do
    {
        Console.Write("The file \"output.txt\" already exists. Do you want to replace it by new one? (y/n): ");
        var answer = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(answer))
        {
            if (answer.ToUpper() == "Y") repeatIsNeeded = false;
            else if (answer.ToUpper() == "N") return;
            else Console.WriteLine("Incorrect answer, try again, please.");
        }
    } while (repeatIsNeeded);
}

using (var inputStream = new StreamReader(inputFile))
{
    long p = 0; long count = 0;
    string? line;
    const int MAX_P = 10000;
    var sorteds = new SortedString[MAX_P];

    for (int i = 0; i < MAX_P; i++)
        sorteds[i] = new SortedString();

    using (var tmpStream = new StreamWriter(OUTPUT_FILE, true))
    {
        while (!string.IsNullOrWhiteSpace(line = inputStream.ReadLine()))
        {
            ++count;
            sorteds[p++].Initialize(line);
            if (p % MAX_P == 0)
            {
                Array.Sort(sorteds);
                for (int i = 0; i < MAX_P; i++)
                    tmpStream.WriteLine(sorteds[i].ToString());
                for (int i = 0; i < MAX_P; i++)
                    sorteds[i].Initialize("0. ");
                p = 0;
            }
        }
        if (p % MAX_P > 0)
        {
            Array.Sort(sorteds);
            for (int i = 0; i < MAX_P; i++)
                if (sorteds[i].Prefix > 0)
                    tmpStream.WriteLine(sorteds[i].ToString());
        }
    }

    if (count == 0) return;

    var lastP = p;

    long firstLength = MAX_P;
    SortedString firstLine = new(), lastOfFirstLine = new(), firstOfSecondLine = new(), lastLine = new(), current = new();

    using (var outputStream = new FileStream(OUTPUT_FILE, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
    {
        while (firstLength < count)
        {
            outputStream.Seek(0, SeekOrigin.Begin);
            firstLine.Initialize(Readline(outputStream));
            lastOfFirstLine.Initialize(ReadlineByNum(outputStream, firstLength));
            firstOfSecondLine.Initialize(Readline(outputStream));
            if (firstLength == count - 1)
            {
                if (firstLine >= firstOfSecondLine)
                {
                    outputStream.Seek(0, SeekOrigin.Begin);
                    CopyLinesToTempFile(outputStream, TMP_FILE, firstLength);
                    Writeline(outputStream, firstOfSecondLine.ToString());
                    CopyFromTempFile(outputStream, TMP_FILE);
                }
                else if (lastOfFirstLine > firstOfSecondLine)
                {
                    outputStream.Seek(0, SeekOrigin.Begin);
                    for (long i = 0; i < firstLength; i++)
                    {
                        var prevPos = outputStream.Position;
                        current.Initialize(Readline(outputStream));
                        if (current >= firstOfSecondLine)
                        {
                            outputStream.Seek(prevPos, SeekOrigin.Begin);
                            CopyLinesToTempFile(outputStream, TMP_FILE, firstLength - i);
                            Writeline(outputStream, firstOfSecondLine.ToString());
                            CopyFromTempFile(outputStream, TMP_FILE);
                            break;
                        }
                    }
                }
                break;
            }
            else
            {
                var num = (count - firstLength) < MAX_P ? lastP : MAX_P;
                lastLine.Initialize(ReadlineByNum(outputStream, firstLength + num));
                if (firstLine >= lastLine)
                {
                    outputStream.Seek(0, SeekOrigin.Begin);
                    CopyLinesToTempFile(outputStream, TMP_FILE, firstLength, false);
                    CopyLinesToTempFile(outputStream, TMP_A_FILE, num);
                    outputStream.Seek(0, SeekOrigin.Begin);
                    CopyFromTempFile(outputStream, TMP_A_FILE);
                    CopyFromTempFile(outputStream, TMP_FILE);
                }
                else if (lastOfFirstLine > firstOfSecondLine)
                {
                    long lastLinePos = 0, k = 0;
                    outputStream.Seek(0, SeekOrigin.Begin);
                    for (long j = 0; j < num; j++)
                    {
                        current.Initialize(ReadlineByNum(outputStream, firstLength + j + 1));
                        if (current >= lastOfFirstLine)
                            break;

                        var current2 = new SortedString();
                        outputStream.Seek(lastLinePos, SeekOrigin.Begin);
                        for (long i = 0; i < firstLength + j - k; i++)
                        {
                            var prevPos = outputStream.Position;
                            current2.Initialize(Readline(outputStream));
                            if (current <= current2)
                            {
                                outputStream.Seek(prevPos, SeekOrigin.Begin);
                                CopyLinesToTempFile(outputStream, TMP_FILE, firstLength + j - k - i);
                                Writeline(outputStream, current.ToString());
                                lastLinePos = outputStream.Position; k += i + 1;
                                CopyFromTempFile(outputStream, TMP_FILE);
                                break;
                            }
                        }
                    }
                }
            }
            
            firstLength += MAX_P;
        }
    }
}

string Readline(FileStream stream)
{
    var byt = -1;
    var bytes = new List<byte>();
    while ((byt = stream.ReadByte()) != 10 && byt != 13 && byt != -1)
    {
        bytes.Add((byte)byt);
    }
    if (byt > -1)
        if ((byt = stream.ReadByte()) != 10 && byt != 13)
            stream.Seek(-1, SeekOrigin.Current);
    return Encoding.UTF8.GetString(bytes.ToArray());
}

string ReadlineByNum(FileStream stream, long num)
{
    stream.Seek(0, SeekOrigin.Begin);
    string result = string.Empty;
    for (long i = 0; i < num; ++i)
    {
        var tmp = Readline(stream);
        if (string.IsNullOrWhiteSpace(tmp)) break;
        result = tmp;
    }
    return result;
}

void Writeline(FileStream stream, string line)
{
    var str = line + Environment.NewLine;
    var bytes = Encoding.UTF8.GetBytes(str);
    stream.Write(bytes);
}

void CopyFromTempFile(FileStream stream, string filename)
{
    using (var tmpStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
    {
        tmpStream.CopyTo(stream);
    }
}

void CopyLinesToTempFile(FileStream baseStream, string filename, long lines, bool savePos = true)
{
    var position = baseStream.Position;
    using (var writeStream = new StreamWriter(filename))
    {
        for (long i = 0; i < lines; ++i)
            writeStream.WriteLine(Readline(baseStream));
    }
    if (savePos)
        baseStream.Seek(position, SeekOrigin.Begin);
}
