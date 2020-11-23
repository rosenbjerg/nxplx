﻿namespace NxPlx.Models
{
    public interface IBackdropImageOwner : IImageOwner
    {
        public string BackdropPath { get; set; }
        public string BackdropBlurHash { get; set; }
    }
}