MathOperation op = Calculator.Add;

op += Calculator.Multiply;

op+= Calculator.Divide;
op(322, 323);


Func<double , double ,double> opfunc = Calculator.Multiply;
opfunc(90, 90);