using System;

[ServiceContract]
public class Contract
{
    [OperationContract]
    string GetData(string value);
}
