namespace MAF.Core.Domain.Exceptions;

[Serializable]
public class NameOutOfRangeException : Exception
{
    public NameOutOfRangeException() { }
    public NameOutOfRangeException(string message) : base(message) { }
    public NameOutOfRangeException(string message,Exception inner) : base(message,inner){}

}

public class InvalidEmailException : Exception
{
    public InvalidEmailException() { }
    public InvalidEmailException(string message) : base(message) { }
    public InvalidEmailException(string message,Exception inner) : base(message,inner){}
}

public class PasswordOutOfRangeException : Exception
{
    public PasswordOutOfRangeException() { }
    public PasswordOutOfRangeException(string message) : base(message) { }
    public PasswordOutOfRangeException(string message, Exception inner) : base(message, inner) { }
}

//Tree Exceptions
public class SciNameOutOfRangeException : Exception
{
    public SciNameOutOfRangeException() { }
    public SciNameOutOfRangeException(string message) : base(message) { }
    public SciNameOutOfRangeException(string message, Exception inner) : base(message, inner) { }

}

public class FruitNameOutOfRangeException : Exception
{
    public FruitNameOutOfRangeException() { }
    public FruitNameOutOfRangeException(string message) : base(message) { }
    public FruitNameOutOfRangeException(string message, Exception inner) : base(message, inner) { }

}
public class DescrOutOfRangeException : Exception
{
    public  DescrOutOfRangeException() { }
    public  DescrOutOfRangeException(string message) : base(message) { }
    public  DescrOutOfRangeException(string message, Exception inner) : base(message, inner) { }

}
