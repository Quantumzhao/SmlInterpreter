
# Neo

# What is it

It belongs to the general category of self-modifying languages. However, being different from many other piers, this specific language is not able to have direct control on the RAM.

Certainly, it is a dynamic language which is interpreted-execution.

Also, since the current interpreter is implemented by C#, it gains a huge advantage: GC.
___

# Documentation

## Keywords

Keyword | Restrains | Meaning
--- | --- | ---
`if` | Global | marking the beginning of the if-block
`goto` | Global | jump to a specified code snippet

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
`*` | `int -(int value)`, `double -(double value)` | Multiply | eg. `-1 + 1 == 0`

### Other meta characters

- `~`
- `\`

### Functions

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

### Label

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
codeof(Label1).delete();
```

In this case, this operation deletes the structure of `if` precedure itself, but not the statements inside it.
After the execution, the code should look like this:

``` C++
a := 1;
codeof(Label1).self.delete();
```

> There's something worth noticing: `Label1` does not exist any more, so if this statement is executed again, an error will inevitably occur.
