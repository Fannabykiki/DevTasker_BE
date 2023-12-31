﻿using Capstone.Common.DTOs.Base;

namespace Capstone.Common.DTOs.BlobAzure
{
	public class BlobResponse
	{
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool? IsSucceed { get; set; }
        public BlobViewModel Blob { get; set; }
    }
}
