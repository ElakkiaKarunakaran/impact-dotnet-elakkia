
using System.Security.Cryptography;
Console.WriteLine("===== FILE HANDLING =====");

string filePath = "students.txt";
string copyPath = "students-copy.txt";

// 1. Write
FileHandler.WriteFile(
	filePath,
	"Elakkia\nRamya\nArjun"
);

// 2. Read
string content = FileHandler.ReadFile(filePath);

Console.WriteLine("\nFile content:");
Console.WriteLine(content);

// 3. Append
FileHandler.AppendFile(filePath, "Karthik");

// 4. Read again to see appended content
Console.WriteLine("\nAfter append:");
Console.WriteLine(FileHandler.ReadFile(filePath));

// 5. Copy file using streams
FileHandler.CopyFile(filePath, copyPath);

// 6. Read file in 4 KB chunks
Console.WriteLine("\nChunked reading:");
FileHandler.ReadInChunks(filePath);


Console.WriteLine("\n===== CRYPTO BASICS =====");

CryptoBasics.DemonstrateThreeWays("Hello");


Console.WriteLine("\n===== DONE =====");


string originalText = "Hello Elakkia";

using Aes aes = Aes.Create();

aes.KeySize = 256;
aes.GenerateKey();

byte[] key = aes.Key;

byte[] ciphertext = AesEncryption.Encrypt(
	originalText,
	key,
	out byte[] iv
);

string decryptedText = AesEncryption.Decrypt(
	ciphertext,
	key,
	iv
);

Console.WriteLine($"Original:  {originalText}");
Console.WriteLine($"Encrypted: {Convert.ToBase64String(ciphertext)}");
Console.WriteLine($"Decrypted: {decryptedText}");

string text = "Hello Elakkia";

byte[] ciphertext1 = AesEncryption.Encrypt(
	text,
	key,
	out byte[] iv1
);

byte[] ciphertext2 = AesEncryption.Encrypt(
	text,
	key,
	out byte[] iv2
);

Console.WriteLine(
	$"Ciphertext 1: {Convert.ToBase64String(ciphertext1)}"
);

Console.WriteLine(
	$"Ciphertext 2: {Convert.ToBase64String(ciphertext2)}"
);

Console.WriteLine(
	$"IV 1: {Convert.ToBase64String(iv1)}"
);

Console.WriteLine(
	$"IV 2: {Convert.ToBase64String(iv2)}"
);

byte[] wrongKey = RandomNumberGenerator.GetBytes(32);

try
{
	string result = AesEncryption.Decrypt(
		ciphertext1,
		wrongKey,
		iv1
	);

	Console.WriteLine($"Decrypted: {result}");
}
catch (CryptographicException)
{
	Console.WriteLine("Decryption failed: wrong key.");
}


string password = "Hello123";

byte[] salt = RandomNumberGenerator.GetBytes(16);

byte[] key1 = KeyDerivation.DeriveKey(
	password,
	salt
);

byte[] key2 = KeyDerivation.DeriveKey(
	password,
	salt
);
byte[] differentSalt = RandomNumberGenerator.GetBytes(16);

byte[] key3 = KeyDerivation.DeriveKey(
	password,
	differentSalt
);

Console.WriteLine(
	$"Different salt: " +
	CryptographicOperations.FixedTimeEquals(key1, key3)
);

Console.WriteLine(
	$"Same password + same salt: " +
	CryptographicOperations.FixedTimeEquals(key1, key2)
);
AesGcmDemo.Run();
HashDemo.Run();
HashComparisonDemo.Run();
LargeFileDemo.CreateLargeFile("largefile.bin");


// File names
string originalFile = "largefile.bin";
string encryptedFile = "encrypted.bin";
string decryptedFile = "decrypted.bin";

// STEP 1: Create 100 MB file
LargeFileDemo.CreateLargeFile(originalFile);

// STEP 2: Create ONE key and ONE IV
// IMPORTANT: We will use these SAME values
// for both encryption and decryption.
byte[] keys= RandomNumberGenerator.GetBytes(32);
byte[] ivs= RandomNumberGenerator.GetBytes(16);

// STEP 3: Encrypt
LargeFileDemo.EncryptFile(
	originalFile,
	encryptedFile,
	keys,
	ivs
);

Console.WriteLine("Encryption completed.");

// STEP 4: Decrypt using THE SAME key and IV
LargeFileDemo.DecryptFile(
	encryptedFile,
	decryptedFile,
	keys,
	ivs
);

Console.WriteLine("Decryption completed.");

// STEP 5: Compare original and decrypted files
bool identical = LargeFileDemo.FilesAreIdentical(
	originalFile,
	decryptedFile
);

Console.WriteLine(
	$"Files identical: {identical}"
);

RsaDemo.Demonstrate();