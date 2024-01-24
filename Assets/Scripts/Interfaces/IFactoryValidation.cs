using System;

public interface IFactoryValidation
{
    IDeployer[] GetDeployers();

    event EventHandler OnProjectCompleted;
    event EventHandler OnProjectEvaluationCompleted;
}
