// --- Test 1: two ints (value type) ---
int a = 10;
int b = a;   // b gets its OWN copy of the value 10
b = 20;
Console.WriteLine($"a={a}, b={b}");
// Output: a=10, b=20
// Explanation: int is a value type. b got a separate copy of a's value,
// so changing b has no effect on a.

// --- Test 2: int array (reference type) ---
int[] arr1 = { 1, 2, 3 };
int[] arr2 = arr1;   // arr2 does NOT get a new array — it points to the SAME array as arr1
arr2[0] = 99;
Console.WriteLine($"arr1[0]={arr1[0]}, arr2[0]={arr2[0]}");
// Output: arr1[0]=99, arr2[0]=99
// Explanation: arrays are reference types. arr1 and arr2 both point to the
// same block of memory, so changing it through arr2 also shows up via arr1.

CoordinateStruct c1 = new CoordinateStruct { X = 1 , Y = 2};
CoordinateStruct c2 = c1;
c1.X = 80;
Console.WriteLine($"C1.X={c1.X}");

CoordinateClass cc1 = new CoordinateClass { X = 11, Y = 9 };
CoordinateClass cc2 = cc1;
cc2.X = 90;
Console.WriteLine($"CC1.X={cc1.X}");

int daynum = 2;
DaysOfWeek d1 = (DaysOfWeek)daynum;
Console.WriteLine($"The num is {daynum} and day is {d1}");


perm perm = perm.read & perm.write;
Console.WriteLine(perm);

bool istrue = (perm&perm.read) == perm.write;
Console.WriteLine($"istrue : {istrue}");

// Implicit (automatic, safe, widening)
int i = 100;
long l = i;        // int fits safely into long, no cast needed
float f = l;       // long fits into float
double d = f;      // float fits into double
Console.WriteLine($"{i} -> {l} -> {f} -> {d}");

// Explicit (manual cast, may lose data — narrowing)
double price = 19.99;
int wholePrice = (int)price;  // must cast manually
Console.WriteLine($"{price} -> {wholePrice}");
// Output: 19.99 -> 19
// Explanation: casting double to int truncates (chops) the decimal part — precision is lost, 0.99 is gone.
string input = "42";

// 1. Convert.ToInt32 — throws an exception if input isn't a valid number
int viaConvert = Convert.ToInt32(input);
Console.WriteLine(viaConvert);

// 2. int.TryParse — SAFEST for user input; doesn't throw, just returns true/false
bool success = int.TryParse(input, out int viaTryParse);
Console.WriteLine($"Success: {success}, Value: {viaTryParse}");

// 3. "is" — checks if an OBJECT is a certain type (not for converting strings directly,
//    this is more for checking types of objects, e.g. in pattern matching)
object obj = 42;
if (obj is int)
{
	Console.WriteLine("obj is an int");
}

// 4. "as" — tries to convert an object to a type; gives null instead of throwing if it fails
//    (only works with reference types or nullable types, not plain int)
object obj2 = "hello";
string asString = obj2 as string;  // works, obj2 is actually a string
Console.WriteLine(asString);

object obj3 = 42;
string failedCast = obj3 as string;  // obj3 is NOT a string, so this gives null instead of crashing
Console.WriteLine(failedCast ?? "null - cast failed safely");

string ab = "5";
int abc = Convert.ToInt32(ab);
bool isvalid = int.TryParse(ab, out int newnum);
Console.WriteLine(newnum);
Console.WriteLine(isvalid);
Console.WriteLine(abc);

