using System.Globalization;
using System.Text;

const string OUTPUT_FILE = "output.txt";

Console.Write("Choose a file size unit (B, KB, MB, GB): ");
var allowedUnits = new string[] { "B", "KB", "MB", "GB" };
string? sizeUnit;
var index = -1;

//Repeat input and check it until we get a correct one.
do
{
    sizeUnit = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(sizeUnit))
        Console.WriteLine("You have chosen nothing! Try again or enter Q for exit.");
    else
    {
        sizeUnit = sizeUnit.ToUpper();
        //Exit if an user has printed Q
        if (sizeUnit == "Q") return;
        index = Array.IndexOf(allowedUnits, sizeUnit);
        if (index == -1)
            Console.WriteLine("You have writen wrong option! Try again or enter Q for exit.");
    }
} while (string.IsNullOrWhiteSpace(sizeUnit) || (index == -1));

double fileSize = 0;
var ourSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
var otherSeparator = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;
var repeatIsNeeded = false;

Console.Write("Print a file size in the selected unit: ");
do {
    repeatIsNeeded = true;
    var fileSizeStr = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(fileSizeStr))
    {
        Console.WriteLine("You have writen nothing! Try again or enter Q for exit.");
        continue;
    }

    if (fileSizeStr.ToUpper() == "Q") return;

    if (ourSeparator != otherSeparator)
        fileSizeStr = fileSizeStr.Replace(otherSeparator, ourSeparator);

    if (!double.TryParse(fileSizeStr, out fileSize))
    {
        Console.WriteLine("You must write a correct number! Try again or enter Q for exit.");
        continue;
    }
    if (fileSize <= 0)
    {
        Console.WriteLine("You must write a positive number! Try again or enter Q for exit.");
        continue;
    }

    //If all validations were successful, we can left the loop
    repeatIsNeeded = false;
} while (repeatIsNeeded);

//Calculate file size in bytes
fileSize *= Math.Pow(1024, index);
long currentSize = 0;
long currentIndex = 1;
string prevWord = string.Empty;
var rand = new Random();

//Unfortunately, we cannot use StreamWriter because it doesn't let to get current file size
using (var outputStream = new FileStream(OUTPUT_FILE, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
{
    try
    {
        while (currentSize < fileSize)
        {
            //We want some words to be repeated
            string currentWord = (currentIndex % 5 == 0) ? prevWord : GenerateNewString();
            var numPrefix = rand.Next(1, 1000000);
            var output = string.Join(". ", numPrefix, currentWord);
            var bytes = Encoding.UTF8.GetBytes(output);
            outputStream.Write(bytes);
            outputStream.WriteByte(10); //10 - line feed
            //We want to memorize some word for repeating it later with other number
            if (currentIndex % 3 == 1)
                prevWord = currentWord;
            currentIndex++;
            currentSize = outputStream.Length;
        }
    } catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.ReadKey();
        return;
    }
}

Console.WriteLine("A test file was created successfully! The file name: " + OUTPUT_FILE);
Console.ReadKey();

string GenerateNewString()
{
    var upperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    var lowerLetters = "abcdefghijklmnopqrstuvwxyz";

    var random = new Random();
    var k = random.Next(26);
    var firstLetter = upperLetters[k];

    //Length of new word may be between 3 and 15
    k = random.Next(3, 15);
    var chars = new char[k];
    //The first letter in a word is upper, and the rest are lower
    chars[0] = firstLetter;

    for (int i = 1; i < k; i++)
    {
        var n = random.Next(26);
        chars[i] = lowerLetters[n];
    }

    return new string(chars);
}
