using System.ServiceModel;

namespace SharedInterface
{
    [ServiceContract]
    internal interface InspectorInterface
    {
        [OperationContract]
        string GetFullSummary();

        [OperationContract]
        void StartInspection();

        [OperationContract]
        void FinishInspection();
    }
}