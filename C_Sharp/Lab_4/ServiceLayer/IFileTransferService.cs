namespace ServiceLayer
{
    interface IFileTransferService
    {
        void Send(string xmlContent, string xsdContent);
    }
}
