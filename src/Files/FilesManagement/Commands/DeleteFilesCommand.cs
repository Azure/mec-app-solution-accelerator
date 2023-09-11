﻿using MediatR;

namespace Microsoft.MecSolutionAccelerator.Services.Files.Commands
{
    public class DeleteFilesCommand : IRequest
    {
        public string bucketName { get; set; }
    }
}
