
# Neo

# What is it

It belongs to the general category of self-modifying languages. However, being different from many other piers, this specific language is not able to have direct control on the RAM.

Certainly, it is a dynamic language which is interpreted-execution.

Also, since the current interpreter is implemented by C#, it gains a huge advantage: GC.
___

# Documentation

## Keywords

Keywords | Restrain | Meaning
--- | --- | ---
`if` | Global | marking the beginning of the if-block
`goto` | Global | jump to a specified code snippet
`codeof` | Global | it is actually a special function which returns the specified code snippet
`return` | Global | it returns the specified value
`include` | Global | by using this, the interpreter stores the referrence to the specified sml file
`delete` | Global | deleting the specified code snippet (node)
`null` | Global | null
`readonly` | Global | marks a certain block of code to be read-only
`true` | Global | true
`false` | Global | false
`last` | Regional | it is always used with an array to retrive the last element
`current` | Regional | it is always used with `codeof()` function to retrive the current code snippet
`all` | Regional | it is always used with `codeof()` to retrive **all** of the current **code block**

## Operators and Functions

### Unary operators

Symbol | Signature | Name | Description
--- | --- | --- | ---
`!` | `bool !(bool value)` | Not | eg. `!true == false`
`-` | `int -(int value)`, `double -(double value)` | Negate | eg. `-1 + 1 == 0`

### Binary operators

Symbol | Signature | Name | Description
--- | --- | --- | ---
`%` | `int %(int value1, value2)`, `int %(double value1, int value2)` | Modulus | eg. `5 % 3 == 2`, `5.5 % 3 == 2`
`*` | `double *(double value1, double value2)` | Multiply | eg. `2 * 2 == 4`
`+` | `double +(double value1, double value2)` | Add | eg. `1 + 1 == 2`
`+` | `string +(string s1, string s2)` | Concatenate | eg. `"1" + "1" == "11"`
`-` | `double -(double value1, double value2)` | Subtract | eg. `2 - 1 == 1`
`/` | `int /(int value1, int value2)`, `double /(double value1, double value2)` | Division | eg. `4 / 2 == 2`
`<=` | `bool <=(double value1, double value2)` | Smaller or equal to | eg. `1 <= 2 == true`
`>=` | `bool >=(double value1, double value2)` | Greater or equal to | eg. `2 >= 1 == true`
`==` | `bool ==(bool value1, bool value2)` | Is equal | eg. `true == true`
`&&` | `bool &&(bool value1, bool value2)` | And | eg. `true && true == true`
`||` | `bool ||(bool value1, bool value2)` | Or | eg. `true || false == true`

### Other meta characters

Symbol | Description | Example
--- | --- | ---
`\` | It is used for transcription | `\"`, instead of `"`
`_` | Array marker | `array_0` for the 0th element in an array
`=` | Assignment | `x = 1`
`:` | The label marker | `Fun1: Add(1, 2)` In this case, the function `Add(1, 2)` is labeled `Fun1`
`"` | double quote | `"Hello world"`
`>` & `<` | paren. used in `#include` | `#include <stdlib.sml>`
`,` | seperator | `func(var1, var2) { }`
`.` | accessor | `codeof(label).all`
`;` | statement end marker | `Func();`

- `(` and `)`
- `{` and `}`

### Functions

All of the pre-defined functions are defined in C# inside the `Standard Library` and `Math` classes. These are:

**In Standard Library:**
___

#### print

#### codeof

- It looks like a function, and uses like a function, so just treat it as a normal one, except for its return type is not a value type.

- Actually, it returns the referrence to the specified node in the syntax tree. Inside the interpreter, the referred object derives from `Token`, and it has the following characteristics:
  - **(Unfinished)**

## Grammar & Syntax

### Declaration

#### Variable Declaration

- Conventional notation 1:
  
  ``` C++
  var := 0;
  ```

  **where:**
  - `var` is the name of the variable. It can be any valid combination of characters.
  e.g. `a := 0;` in which case, `a` is the name of the variable.

  - `:=` is the declaration operator.

- Conventional notation 2:

  ``` C++
  var: 0;
  ```

  **where**
  - `var` is the name of the variable.

  - `:` is the declaration operator. In fact, the operator `:=` is absolutely equivalent to `:`. The former notation is just for readability.
  They all create a label pointing to a literal.
  *And hence, a variable in its very nature is a label and a literal.*

#### Method Declaration

#### File Declaraction

Every *.sml* file can be parsed into a `File` object. The source code itself is a declaraction.

> If some fields and methods of a *sml* file needs to be utilized in some code, just add `#include <`*fileName*`>` on top of the code.

### Label

> Labels cannot begin with a number.

The usage of label in variable declaration is just a special case among all of its usages.

In fact a label can also be used in marking all kinds of code snippets, as listed:

- Statement

- Expression

  - And Also values returned by expressions (which is a variable)

- Variable (the usage described above)

- Procedure

  - `if` block

  - Method

    - Function

    - Subprocedure

> An executable script itself is not a procedure, and hence it is ungrammatical to be labeled.

**For example:**

``` C++
Variable1 := 0;
codeof(Label1) = "Variable1 := 1";
```

This code snippet replaces the statement labeled `Label1` with a new statement. Although this replacement doesn't change the definition of variabel a nor its type, the interpreter will replace the old statement (which is a node in the syntax tree) with a new **re-parsed** statement.

**There is another example:**

``` C++
Label1: if (true)
{
    var := 0;
}
delete codeof(Label1);
```

In this case, this operation deletes the structure of `if` precedure itself, but not the statements inside it.
After the execution, the code should look like this:

``` C++
a := 1;
delete codeof(a);
```

> There's something worth noticing: `a` does not exist any more, so if this statement is executed again, an error will inevitably occur.

### Array

#### Array Declaration

Array in Neo is not really an object type. In fact, it exists to provide functionality to manipulate multiple logically correlated variables.

Therefore, unlike many other languages, there is not need to declare an array: an array exists since the creation of its first element.

An element in an array is consisted of three parts: **its name, _underscore character_ and subscript**.

Any integer after `_` is considered the subscript.

> eg. `array_0: 0`

- In this example, it creates a new array named `array`, and initialize its first item to be `0`

Note that Neo arrays have no limitation on length.

When this array needs to have a second element, simply do this: `array_1: 0;` or of course, its equivalent: `array_1 := 0`

Also, the subscription don't need to be successive. For example:

``` C++
array_0: 0;
array_2: 0;
```

#### Manipulation

Any element in an array can be manipulated by doing this:

``` C++
array_0 = 1;
```

Just use it like a normal variable.

Another example:

``` C++
i := 0;
For: if (i < 2){
    array_i = i;
    i++;
    goto codeof(For);
}
```

In this exampe, there is a for loop that sets the *n_th* element to the value of *n*.
