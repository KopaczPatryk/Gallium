using Gallium;
using Microsoft.ProjectOxford.Face;
using System;

namespace FaceApiClient
{
    public class FaceApiMediator
    {
        IFaceServiceClient faceClient;

        public FaceApiMediator ()
        {
            faceClient = new FaceServiceClient(Constants.APIkey, Constants.APIUri);
        }

    }
}
