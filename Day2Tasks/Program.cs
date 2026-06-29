using System.Xml.Linq;
using SchoolManagement;

SchoolManagement.Student S1 = new SchoolManagement.Student();
S1.Name = "Elakkia";
S1.SayHello();

Student S2 = new Student();
S2.Name = "Sri";

ModuleA.Helper.Greet();
ModuleB.Helper.Greet();

// 5 variables — camelCase for locals, PascalCase shown via a method name example
int studentAge = 21;
string studentName = "Elakkia";
double averageScore = 88.5;
bool isPassed = true;
DateTime enrollmentDate = DateTime.Now;
int @class = 5;

Console.WriteLine($"{@class}, age {studentAge}, passed: {isPassed}");




// Without "using", we must fully qualify the class name (SchoolManagement.Student) every time.
// With "using", the compiler already knows where to look, so we can just write "Student".
// Both should run fine together in the same file. That comment at the bottom is your "noted
