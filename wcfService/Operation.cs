using System;
using System.ServiceModel;

[ServiceContract]
public class Contract
{
    [OperationContract]
    string GetData(string value)
    {
        return "test " + value;
    }
}
